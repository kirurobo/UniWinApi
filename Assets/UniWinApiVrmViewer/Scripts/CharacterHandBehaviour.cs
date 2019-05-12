using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class CharacterHandBehaviour : MonoBehaviour {


    public float aimSpeed = 10.0f;   // 頭の追従速度係数 [1/s]

    [SerializeField]
    private GameObject targetObject;        // 視線目標オブジェクト
    [SerializeField]
    private Transform leftHandTransform;    // Head transform
    [SerializeField]
    private Transform rightHandTransform;   // Head transform
    private bool hasNewTargetObject = false;    // 新規に目標オブジェクトを作成したらtrue

    private float cursorGrabingSqrMagnitude = 0.81f;    // 手の届く距離の2乗（これ以上離れると手を伸ばすことをやめる）

    private Animator animator;

    private Camera currentCamera;

    private float lastRightHandWait = 0f;
    private float lastLeftHandWait = 0f;

    // Use this for initialization
    void Start()
    {
        if (!targetObject)
        {
            targetObject = new GameObject("LookAtTarget");
            hasNewTargetObject = true;
        }

        if (!leftHandTransform)
        {
            leftHandTransform = transform.Find("LeftHand");
        }
        if (!rightHandTransform)
        {
            rightHandTransform = transform.Find("RightHand");
        }

        // マウスカーソルとの奥行きに反映させるためカメラを取得
        if (!currentCamera)
        {
            currentCamera = Camera.main;
        }

        animator = GetComponent<Animator>();
        if (animator)
        {
            rightHandTransform = animator.GetBoneTransform(HumanBodyBones.RightHand);
            leftHandTransform = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        }
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

    /// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
    void Update()
    {
        UpdateTarget();
    }

    void OnAnimatorIK(int layerIndex)
    {
        Debug.Log(layerIndex);
        UpdateHand();
    }

    /// <summary>
    /// Update()より後で呼ばれる
    /// </summary>
    void LateUpdate()
    {
    }

    /// <summary>
    /// 目線目標座標を更新
    /// </summary>
    private void UpdateTarget()
    {
    }

    /// <summary>
    /// マウスカーソルの方を見る動作
    /// </summary>
    private void UpdateHand()
    {
        if (!animator || !rightHandTransform || !leftHandTransform) return;

        const float cursorOffsetZ = 1.2f;   // カーソルZ座標 [Unit]

        bool isRightHandMoved = false;
        bool isLeftHandMoved = false;

        // カーソル座標をUnityのワールド座標に変換
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, cursorOffsetZ)
        );

        // モデル基準座標に対するカーソルの横位置
        float relativeX = cursorPosition.x - this.transform.position.x;

        if (relativeX < -0.1f)
        {  // カーソルが右手側にある場合
           // 右手とカーソルの距離の2乗
            float sqrDistance = (cursorPosition - rightHandTransform.position).sqrMagnitude;

            // モデルやアニメーションの状態によるが、位置調整
            cursorPosition.y -= 0.05f;

            // 右手からの距離が近ければ追従させる
            if ((sqrDistance < cursorGrabingSqrMagnitude))
            {
                lastRightHandWait = Mathf.Lerp(lastRightHandWait, 0.7f, 0.1f);

                Quaternion handRotation = Quaternion.Euler(-90f, 180f, 0f);

                animator.SetIKPosition(AvatarIKGoal.RightHand, cursorPosition);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, lastRightHandWait);

                animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, lastRightHandWait);

                isRightHandMoved = true;
            }
        }
        else if (relativeX > 0.1f)
        {   // カーソルが左手側にある場合
            // 左とカーソルの距離の2乗
            float sqrDistance = (cursorPosition - leftHandTransform.position).sqrMagnitude;

            // モデルやアニメーションの状態によるが、位置調整
            cursorPosition.x += 0.05f;

            // 左手からの距離が近ければ追従させる
            if ((sqrDistance < cursorGrabingSqrMagnitude))
            {
                lastLeftHandWait = Mathf.Lerp(lastLeftHandWait, 0.7f, 0.1f);

                Quaternion handRotation = Quaternion.Euler(-90f, 180f, 0f);

                animator.SetIKPosition(AvatarIKGoal.LeftHand, cursorPosition);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lastLeftHandWait);

                animator.SetIKRotation(AvatarIKGoal.LeftHand, handRotation);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lastLeftHandWait);

                isLeftHandMoved = true;
            }
        }

        if (!isRightHandMoved)
        {
            // 右手を戻す
            lastRightHandWait = Mathf.Lerp(lastRightHandWait, 0.0f, 0.1f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, cursorPosition);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, lastRightHandWait);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, lastRightHandWait);
        }

        if (!isLeftHandMoved)
        {
            // 左手を戻す
            lastLeftHandWait = Mathf.Lerp(lastLeftHandWait, 0.0f, 0.1f);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, cursorPosition);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lastLeftHandWait);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lastLeftHandWait);

        }
    }
}
