/**
 * VrmCharacterBehaviour
 * 
 * キャラクターのまばたきや表情、動作を制御します
 * 
 * Author: Kirurobo http://twitter.com/kirurobo
 * License: MIT
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class VrmCharacterBehaviour : MonoBehaviour
{

    public float LookAtSpeed = 10.0f;   // 頭の追従速度係数 [1/s]
    private float BlinkTime = 0.1f; // まばたきで閉じるまたは開く時間 [s]

    private float lastBlinkTime = 0f;
    private float nextBlinkTime = 0f;
    private BlinkState blinkState = BlinkState.None; // まばたきの状態管理。 0:なし, 1:閉じ中, 2:開き中

    public enum BlinkState
    {
        None = 0,       // 瞬き無効
        Closing = 1,
        Opening = 2,
    }

    public enum MotionMode
    {
        Default = 0,
        Random = 1,
        Bvh = 2,
    }

    /// <summary>
    /// 表情のプリセット
    /// </summary>
    public static BlendShapePreset[] EmotionPresets =
    {
        BlendShapePreset.Neutral,
        BlendShapePreset.Joy,
        BlendShapePreset.Sorrow,
        BlendShapePreset.Angry,
        BlendShapePreset.Fun,
        BlendShapePreset.Unknown,
    };

    internal int emotionIndex = 0;  // 表情の状態
    internal float emotionRate = 0f; // その表情になっている程度 0～1
    private float emotionSpeed = 0f;  // 表情を発生させる方向なら 1、戻す方向なら -1、維持なら 0
    private float nextEmotionTime = 0f;   // 次に表情を変化させる時刻

    public float emotionInterval = 2f;     // 表情を変化させる間隔
    public float emotionIntervalRandamRange = 5f;  // 表情変化間隔のランダム要素

    private float emotionPromoteTime = 0.5f;  // 表情が変化しきるまでの時間 [s]

    private VRMLookAtHead lookAtHead;
    private VRMBlendShapeProxy blendShapeProxy;

    private GameObject targetObject;    // 視線目標オブジェクト
    private Transform headTransform;    // Head transform
    private bool hasNewTargetObject = false;	// 新規に目標オブジェクトを作成したらtrue
    private Transform leftHandTransform;    // Head transform
    private Transform rightHandTransform;   // Head transform

    private Animator animator;
    private AnimatorStateInfo currentState;     // 現在のステート状態を保存する参照
    private AnimatorStateInfo previousState;    // ひとつ前のステート状態を保存する参照

    public MotionMode motionMode = MotionMode.Default;

    //private float cursorGrabingSqrMagnitude = 0.81f;    // 手の届く距離の2乗（これ以上離れると手を伸ばすことをやめる）
    private float cursorGrabingSqrMagnitude = 10000f;    // 手の届く距離の2乗（これ以上離れると手を伸ばすことをやめる）
    private float lastRightHandWait = 0f;
    private float lastLeftHandWait = 0f;

    public bool randomMotion = false;   // モーションをランダムにするか
    public bool randomEmotion = true;

    private Camera currentCamera;
    private VrmUiController uiController;


    // Use this for initialization
    void Start()
    {
        if (!targetObject)
        {
            targetObject = new GameObject("LookAtTarget");
            hasNewTargetObject = true;
        }

        lookAtHead = GetComponent<VRMLookAtHead>();
        blendShapeProxy = GetComponent<VRMBlendShapeProxy>();

        if (lookAtHead)
        {
            lookAtHead.Target = targetObject.transform;
            lookAtHead.UpdateType = UpdateType.LateUpdate;

            headTransform = lookAtHead.Head;
        }
        if (!headTransform)
        {
            headTransform = this.transform;
        }

        // マウスカーソルとの奥行きに反映させるためカメラを取得
        if (!currentCamera)
        {
            currentCamera = Camera.main;
        }

        // UIコントローラーを取得
        if (!uiController)
        {
            uiController = FindObjectOfType<VrmUiController>();
        }

        SetAnimator(GetComponent<Animator>());

        currentState = animator.GetCurrentAnimatorStateInfo(0);
        previousState = currentState;
    }

    /// <summary>
    /// Destroy created target object
    /// </summary>
    void OnDestroy()
    {
        if (hasNewTargetObject)
        {
            GameObject.Destroy(targetObject);
        }
    }

    public void SetAnimator(Animator anim)
    {
        if (!anim)
        {
            rightHandTransform = null;
            leftHandTransform = null;
        }
        else if (anim != animator)
        {
            rightHandTransform = anim.GetBoneTransform(HumanBodyBones.RightHand);
            leftHandTransform = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        }
        animator = anim;
        animator.applyRootMotion = true;
    }

    public void SetMotionMode(MotionMode mode)
    {
        motionMode = mode;
    }

    /// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
    void Update()
    {
        UpdateLookAtTarget();
        Blink();
        RandomFace();
        UpdateMotion();
    }

    /// <summary>
    /// Update()より後で呼ばれる
    /// </summary>
    void LateUpdate()
    {
        UpdateHead();
    }

    /// <summary>
    /// 目線目標座標を更新
    /// </summary>
    private void UpdateLookAtTarget()
    {
        Vector3 mousePos = Input.mousePosition;
        //// 奥行きはモデル座標から 1[m] 手前に設定
        //mousePos.z = (currentCamera.transform.position - headTransform.position).magnitude - 1f;
        // 奥行きはモデル座標とカメラ間の90%と設定
        mousePos.z = (currentCamera.transform.position - headTransform.position).magnitude * 0.90f;
        Vector3 pos = currentCamera.ScreenToWorldPoint(mousePos);
        targetObject.transform.position = pos;
    }

    /// <summary>
    /// マウスカーソルの方を見る動作
    /// </summary>
    private void UpdateHead()
    {
        Quaternion rot = Quaternion.Euler(-lookAtHead.Pitch, lookAtHead.Yaw, 0f);
        headTransform.rotation = Quaternion.Slerp(headTransform.rotation, rot, 0.2f);

    }

    /// <summary>
    /// まばたき
    /// </summary>
    private void Blink()
    {
        if (!blendShapeProxy) return;

        float now = Time.timeSinceLevelLoad;
        float span;

        BlendShapeKey blinkShapeKey = BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink);

        // 表情が笑顔の時は目が閉じられるため、まばたきは無効とする
        if (EmotionPresets[emotionIndex] == BlendShapePreset.Joy)
        {
            blinkState = BlinkState.None;
            blendShapeProxy.ImmediatelySetValue(blinkShapeKey, 0f);
        }

        // まばたきの状態遷移
        switch (blinkState)
        {
            case BlinkState.Closing:
                span = now - lastBlinkTime;
                if (span > BlinkTime)
                {
                    blinkState = BlinkState.Opening;
                    blendShapeProxy.ImmediatelySetValue(blinkShapeKey, 1f);
                }
                else
                {
                    blendShapeProxy.ImmediatelySetValue(blinkShapeKey, (span / BlinkTime));
                }
                break;
            case BlinkState.Opening:
                span = now - lastBlinkTime - BlinkTime;
                if (span > BlinkTime)
                {
                    blinkState = BlinkState.None;
                    blendShapeProxy.ImmediatelySetValue(blinkShapeKey, 0f);
                }
                else
                {
                    blendShapeProxy.ImmediatelySetValue(blinkShapeKey, (1f - span) / BlinkTime);
                }
                break;
            default:
                if (now >= nextBlinkTime)
                {
                    lastBlinkTime = now;
                    if (Random.value < 0.2f)
                    {
                        nextBlinkTime = now;    // 20%の確率で連続まばたき
                    }
                    else
                    {
                        nextBlinkTime = now + Random.Range(1f, 10f);
                    }
                    blinkState = BlinkState.Closing;
                }
                break;
        }
    }

    /// <summary>
    /// 表情をランダムに変更
    /// </summary>
    public void RandomFace()
    {
        float now = Time.timeSinceLevelLoad;

        if (now >= nextEmotionTime)
        {
            // 待ち時間を越えた場合の処理
            nextEmotionTime = now + emotionInterval + Random.value * emotionIntervalRandamRange;
            emotionSpeed = (emotionSpeed > 0 ? -1f : emotionSpeed < 0 ? 0 : 1f);    // 表情を与えるか戻すか、次の方向を決定

            // 表情を与えるなら、ランダムで次の表情を決定
            if (emotionSpeed > 0)
            {
                emotionIndex = Random.Range(0, EmotionPresets.Length - 1);
            }
        }
        else
        {
            // 待ち時間に達していなければ、変化を処理
            float dt = Time.deltaTime;
            emotionRate = Mathf.Min(1f, Mathf.Max(0f, emotionRate + emotionSpeed * (dt / emotionPromoteTime)));

            UpdateEmotion();
        }

    }

    /// <summary>
    /// 現在の表情を適用
    /// </summary>
    private void UpdateEmotion()
    {
        if (!blendShapeProxy) return;

        if (!randomEmotion) return;     // 現状、ランダムが解除されていたら何もしない（戻さない）

        var blendShapes = new List<KeyValuePair<BlendShapeKey, float>>();

        int index = 0;
        foreach (var shape in EmotionPresets)
        {
            float val = 0f;
            // 現在選ばれている表情のみ値を入れ、他はゼロとする
            if (index == emotionIndex) val = emotionRate;
            blendShapes.Add(new KeyValuePair<BlendShapeKey, float>(BlendShapeKey.CreateFromPreset(shape), val));
            index++;
        }
        blendShapeProxy.SetValues(blendShapes);

        UpdateUI();
    }

    /// <summary>
    /// UIのブレンドシェイプ表示を更新
    /// </summary>
    private void UpdateUI()
    {
        if (!uiController) return;

        if (uiController.enableRandomEmotion)
        {
            uiController.SetBlendShape(emotionIndex);
            uiController.SetBlendShapeValue(emotionRate);
        }
    }

    /// <summary>
    /// モーション変更に関する処理をここに書く（現在はオミット）
    /// </summary>
    private void UpdateMotion()
    {
    }

    /// <summary>
    /// IK処理時に手をマウスカーソルに伸ばす
    /// </summary>
    void OnAnimatorIK()
    {
        UpdateHand();
    }

    /// <summary>
    /// マウスカーソルの方を見る動作
    /// </summary>
    private void UpdateHand()
    {
        if (!animator || !rightHandTransform || !leftHandTransform) return;

        // BVH再生中はスキップ
        if (motionMode == MotionMode.Bvh) return;

        bool isRightHandMoved = false;
        bool isLeftHandMoved = false;

        Vector3 cursorPosition = targetObject.transform.position;

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

        // IK_HANDというアニメーションのときのみ、手を伸ばす
        if (animState.IsName("IK_HAND") || animState.IsName("IK_HAND_REVERSE"))
        {

            float sqrDistanceRight = (cursorPosition - rightHandTransform.position).sqrMagnitude;
            float sqrDistanceLeft = (cursorPosition - leftHandTransform.position).sqrMagnitude;

            if (sqrDistanceRight < sqrDistanceLeft)
            {  // カーソルが右手側にある場合
               // 右手とカーソルの距離の2乗
                float sqrDistance = sqrDistanceRight;

                // モデルやアニメーションの状態によるが、位置調整
                //cursorPosition.y -= 0.05f;

                // 右手からの距離が近ければ追従させる
                if ((sqrDistance < cursorGrabingSqrMagnitude))
                {
                    lastRightHandWait = Mathf.Lerp(lastRightHandWait, 0.7f, 0.1f);

                    Quaternion handRotation = Quaternion.Euler(-90f, 180f, 0f);

                    animator.SetIKPosition(AvatarIKGoal.RightHand, cursorPosition);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, lastRightHandWait);

                    //animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
                    //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, lastRightHandWait);

                    isRightHandMoved = true;
                }
            }
            else
            {   // カーソルが左手側にある場合
                // 左とカーソルの距離の2乗
                float sqrDistance = sqrDistanceLeft;

                // モデルやアニメーションの状態によるが、位置調整
                //cursorPosition.x += 0.05f;

                // 左手からの距離が近ければ追従させる
                if ((sqrDistance < cursorGrabingSqrMagnitude))
                {
                    lastLeftHandWait = Mathf.Lerp(lastLeftHandWait, 0.7f, 0.1f);

                    //Quaternion handRotation = Quaternion.Euler(-90f, 180f, 0f);

                    animator.SetIKPosition(AvatarIKGoal.LeftHand, cursorPosition);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lastLeftHandWait);

                    //animator.SetIKRotation(AvatarIKGoal.LeftHand, handRotation);
                    //animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lastLeftHandWait);

                    isLeftHandMoved = true;
                }
            }

        }

        if (!isRightHandMoved)
        {
            // 右手を戻す
            lastRightHandWait = Mathf.Lerp(lastRightHandWait, 0.0f, 0.1f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, cursorPosition);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, lastRightHandWait);
            //animator.SetIKRotationWeight(AvatarIKGoal.RightHand, lastRightHandWait);
        }

        if (!isLeftHandMoved)
        {
            // 左手を戻す
            lastLeftHandWait = Mathf.Lerp(lastLeftHandWait, 0.0f, 0.1f);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, cursorPosition);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lastLeftHandWait);
            //animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lastLeftHandWait);

        }
    }
}
