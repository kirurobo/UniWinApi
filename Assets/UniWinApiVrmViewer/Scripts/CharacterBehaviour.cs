using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class CharacterBehaviour : MonoBehaviour
{

    public float LookAtSpeed = 10.0f;   // 頭の追従速度係数 [1/s]
    private float BlinkTime = 0.1f; // まばたきで閉じるまたは開く時間 [s]

    private float lastBlinkTime = 0f;
    private float nextBlinkTime = 0f;
    private int blinkState = 0; // まばたきの状態管理。 0:なし, 1:閉じ中, 2:開き中

    private BlendShapePreset[] emotionPresets =
    {
        BlendShapePreset.Neutral,
        BlendShapePreset.Joy,
        BlendShapePreset.Sorrow,
        BlendShapePreset.Angry,
        BlendShapePreset.Fun,
        BlendShapePreset.Unknown,
    };

    private int emotionIndex = 0;  // 表情の状態
    private float emotionRate = 0f; // その表情になっている程度 0～1
    private float emotionSpeed = 0f;  // 表情を発生させる方向なら 1、戻す方向なら -1、維持なら 0
    private float lastEmotionTime = 0f;   // 前回表情を変化させた時刻
    private float nextEmotionTime = 0f;   // 次に表情を変化させる時刻
    public float emotionInterval = 2f;     // 表情を変化させる間隔
    public float emotionIntervalRandamRange = 5f;  // 表情変化間隔のランダム要素
    private float emotionPromoteTime = 0.5f;  // 表情が変化しきるまでの時間 [s]
 
    private VRMLookAtHead lookAtHead;
    private VRMBlendShapeProxy blendShapeProxy;

    private GameObject targetObject;    // 視線目標オブジェクト
    private Transform headTransform;    // Head transform
    private bool isNewTargetObject = false;	// 新規に目標オブジェクトを作成したらtrue

    private Animator animator;
    private AnimatorStateInfo currentState;     // 現在のステート状態を保存する参照
    private AnimatorStateInfo previousState;    // ひとつ前のステート状態を保存する参照
    public bool randomMotion = true;                // ランダム判定スタートスイッチ
    private float randomMotionThreshold = 0.5f;             // ランダム判定の閾値
    private float randomMotionInterval = 5f;				// ランダム判定のインターバル

    public bool randomEmotion = true;

    private Camera currentCamera;

    // Use this for initialization
    void Start()
    {
        if (!targetObject)
        {
            targetObject = new GameObject("LookAtTarget");
            isNewTargetObject = true;
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

        animator = GetComponent<Animator>();
        currentState = animator.GetCurrentAnimatorStateInfo(0);
        previousState = currentState;
        // ランダム判定用関数をスタートする
        StartCoroutine("RandomChange");
    }

    /// <summary>
    /// Destroy created target object
    /// </summary>
    void OnDestroy()
    {
        if (isNewTargetObject)
        {
            GameObject.Destroy(targetObject);
        }
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

        switch (blinkState)
        {
            case 1:
                span = now - lastBlinkTime;
                if (span > BlinkTime)
                {
                    blinkState = 2;
                    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Blink, 1f);
                }
                else
                {
                    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Blink, (span / BlinkTime));
                }
                break;
            case 2:
                span = now - lastBlinkTime - BlinkTime;
                if (span > BlinkTime)
                {
                    blinkState = 0;
                    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Blink, 0f);
                }
                else
                {
                    blendShapeProxy.ImmediatelySetValue(BlendShapePreset.Blink, (1f - span) / BlinkTime);
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
                    blinkState = 1;
                }
                break;
        }
    }

    public void RandomFace()
    {
        float now = Time.timeSinceLevelLoad;

        if (now >= nextEmotionTime)
        {
            // 待ち時間を越えた場合の処理
            nextEmotionTime = now + emotionInterval + Random.value * emotionIntervalRandamRange;
            emotionSpeed = (emotionSpeed > 0 ? -1f : emotionSpeed < 0 ? 0 : 1f);    // 表情を与えるか戻すか、次の方向を決定
            lastEmotionTime = now;

            // 表情を与えるなら、ランダムで次の表情を決定
            if (emotionSpeed > 0)
            {
                emotionIndex = Random.Range(0, emotionPresets.Length - 1);
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
        foreach (var shape in emotionPresets)
        {
            float val = 0f;
            // 現在選ばれている表情のみ値を入れ、他はゼロとする
            if (index == emotionIndex) val = emotionRate;
            blendShapes.Add(new KeyValuePair<BlendShapeKey, float>(new BlendShapeKey(shape), val));
            index++;
        }
        blendShapeProxy.SetValues(blendShapes);
    }

    public void ForwardMotion()
    {
            // ブーリアンNextをtrueにする
            animator.SetBool("Next", true);
    }

    public void BackwardMotion()
    {
            // ブーリアンBackをtrueにする
            animator.SetBool("Back", true);
    }

    private void UpdateMotion()
    {
        if (randomMotion)
        {
            // ↑キー/スペースが押されたら、ステートを次に送る処理
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ForwardMotion();
            }

            // ↓キーが押されたら、ステートを前に戻す処理
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                BackwardMotion();
            }

            // "Next"フラグがtrueの時の処理
            if (animator.GetBool("Next"))
            {
                // 現在のステートをチェックし、ステート名が違っていたらブーリアンをfalseに戻す
                currentState = animator.GetCurrentAnimatorStateInfo(0);
                if (previousState.fullPathHash != currentState.fullPathHash)
                {
                    animator.SetBool("Next", false);
                    previousState = currentState;
                }
            }

            // "Back"フラグがtrueの時の処理
            if (animator.GetBool("Back"))
            {
                // 現在のステートをチェックし、ステート名が違っていたらブーリアンをfalseに戻す
                currentState = animator.GetCurrentAnimatorStateInfo(0);
                if (previousState.fullPathHash != currentState.fullPathHash)
                {
                    animator.SetBool("Back", false);
                    previousState = currentState;
                }
            }
        }
    }

    // ランダム判定用関数
    IEnumerator RandomChange()
    {
        // 無限ループ開始
        while (true)
        {
            //ランダム判定スイッチオンの場合
            if (randomMotion)
            {
                // ランダムシードを取り出し、その大きさによってフラグ設定をする
                float _seed = Random.Range(0.0f, 1.0f);
                if (_seed < randomMotionThreshold)
                {
                    animator.SetBool("Back", true);
                }
                else if (_seed >= randomMotionThreshold)
                {
                    animator.SetBool("Next", true);
                }
            }
            // 次の判定までインターバルを置く
            yield return new WaitForSeconds(randomMotionInterval);
        }

    }

}
