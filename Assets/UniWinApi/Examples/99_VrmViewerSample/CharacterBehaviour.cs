using UnityEngine;
using VRM;

public class CharacterBehaviour : MonoBehaviour
{

	public float LookAtSpeed = 10.0f;   // 頭の追従速度係数 [1/s]
	private float BlinkTime = 0.1f; // まばたきで閉じるまたは開く時間 [s]

	private float lastBlinkTime = 0f;
	private float nextBlinkTime = 0f;
	private int blinkBlendShapeIndex = -1;
	private int blinkState = 0; // まばたきの状態管理。 0:なし, 1:閉じ中, 2:開き中

	private VRMLookAtHead lookAtHead;
	private VRMBlendShapeProxy blendShapeProxy;

	private GameObject targetObject;    // 視線目標オブジェクト
	private Transform headTransform;    // Head transform
	private bool isNewTargetObject = false;	// 新規に目標オブジェクトを作成したらtrue

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

			headTransform = lookAtHead.Head.Transform;
		}
		if (!headTransform)
		{
			headTransform = this.transform;
		}
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
		// モデル座標から 1[m] 手前に設定
		mousePos.z = (Camera.main.transform.position - headTransform.position).magnitude - 1f;
		Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
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

}
