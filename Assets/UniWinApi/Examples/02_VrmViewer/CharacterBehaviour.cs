using UnityEngine;
using System.Collections;
using VRM;

public class CharacterBehaviour : MonoBehaviour {

	public float LookAtSpeed = 10.0f;	// 頭の追従速度係数 [1/s]
    private float CursorGrabingSqrMagnitude = 0.81f;    // 手の届く距離の2乗（これ以上離れると手を伸ばすことをやめる）
    private float BlinkTime = 0.1f; // まばたきで閉じるまたは開く時間 [s]

    private Animator animator = null;
    private SkinnedMeshRenderer skinnedMeshRenderer = null;
    private Transform rightHandTransform = null;
    private Transform leftHandTransform = null;
	public Transform HeadTransform = null;

    private float lastRightHandWait = 0f;
    private float lastLeftHandWait = 0f;

    private float lastBlinkTime = 0f;
    private float nextBlinkTime = 0f;
    private int blinkBlendShapeIndex = -1;
    private int blinkState = 0; // まばたきの状態管理。 0:なし, 1:閉じ中, 2:開き中

	private VRMLookAtHead lookAtHead;
	private VRMBlendShapeProxy blendShapeProxy;


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
        if (animator != null)
        {
            rightHandTransform = animator.GetBoneTransform(HumanBodyBones.RightHand);
            leftHandTransform = animator.GetBoneTransform(HumanBodyBones.LeftHand);

            // Humanoidならばここで探すのでエディタ上でHeadTransformは指定不要
            if (HeadTransform == null)
            {
                HeadTransform = animator.GetBoneTransform(HumanBodyBones.Head);
            }
        }

		lookAtHead = GetComponent<VRMLookAtHead>();
		blendShapeProxy = GetComponent<VRMBlendShapeProxy>();
	}
	
	/// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
	void Update () {
        Blink();
    }

    /// <summary>
    /// IK評価前に呼ばれる
    /// </summary>
    void OnAnimatorIK()
    {
        LookAtCursor();
        TraceCursor();
    }

    /// <summary>
    /// 
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
					blendShapeProxy.SetValue(BlendShapePreset.Blink, 1f);
                }
                else
                {
					blendShapeProxy.SetValue(BlendShapePreset.Blink, (span / BlinkTime));
                }
                break;
            case 2:
                span = now - lastBlinkTime - BlinkTime;
                if (span > BlinkTime)
                {
                    blinkState = 0;
					blendShapeProxy.SetValue(BlendShapePreset.Blink, 0f);
                }
                else
                {
					blendShapeProxy.SetValue(BlendShapePreset.Blink, (1f - span) / BlinkTime);
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

    /// <summary>
    /// マウスカーソルの方を見る動作
    /// </summary>
    private void LookAtCursor()
    {
    }

    /// <summary>
    /// マウスカーソルを手で追う動作
    /// </summary>
    private void TraceCursor()
    {
        if (animator != null && rightHandTransform != null && leftHandTransform != null)
        {
            const float cursorOffsetZ = 1.2f;   // カーソルZ座標 [Unit]

            bool isRightHandMoved = false;
            bool isLeftHandMoved = false;

            // カーソル座標をUnityのワールド座標に変換
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, cursorOffsetZ)
            );

            // モデル基準座標に対するカーソルの横位置
            float relativeX = cursorPosition.x - this.transform.position.x;

            if (relativeX < -0.1f) {  // カーソルが右手側にある場合
                // 右手とカーソルの距離の2乗
                float sqrDistance = (cursorPosition - rightHandTransform.position).sqrMagnitude;

                // モデルやアニメーションの状態によるが、位置調整
                cursorPosition.y -= 0.05f;

                // 右手からの距離が近ければ追従させる
                if ((sqrDistance < CursorGrabingSqrMagnitude))
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
                if ((sqrDistance < CursorGrabingSqrMagnitude))
                {
                    lastLeftHandWait = Mathf.Lerp(lastLeftHandWait, 0.7f, 0.1f);

                    Quaternion handRotation = Quaternion.Euler(-90f, 180f, 0f);

                    animator.SetIKPosition(AvatarIKGoal.LeftHand, cursorPosition);
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lastLeftHandWait);

                    animator.SetIKRotation(AvatarIKGoal.LeftHand, handRotation);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lastLeftHandWait);

                    isRightHandMoved = true;
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
}
