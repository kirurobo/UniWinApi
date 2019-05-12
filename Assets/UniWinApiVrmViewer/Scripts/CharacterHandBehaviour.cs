using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

public class CharacterHandBehaviour : MonoBehaviour {


    public float aimSpeed = 10.0f;   // 頭の追従速度係数 [1/s]

    private VRMBlendShapeProxy blendShapeProxy;

    private GameObject targetObject;        // 視線目標オブジェクト
    private Transform leftHandTransform;    // Head transform
    private Transform rightHandTransform;   // Head transform
    private bool hasNewTargetObject = false;	// 新規に目標オブジェクトを作成したらtrue

    private Animator animator;

    private Camera currentCamera;

    // Use this for initialization
    void Start()
    {
        if (!targetObject)
        {
            targetObject = new GameObject("LookAtTarget");
            hasNewTargetObject = true;
        }

        blendShapeProxy = GetComponent<VRMBlendShapeProxy>();

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

    /// <summary>
    /// Update()より後で呼ばれる
    /// </summary>
    void LateUpdate()
    {
        UpdateHand();
    }

    /// <summary>
    /// 目線目標座標を更新
    /// </summary>
    private void UpdateTarget()
    {
        if (hasNewTargetObject)
        {
            Vector3 mousePos = Input.mousePosition;
            //// 奥行きはモデル座標から 1[m] 手前に設定
            //mousePos.z = (currentCamera.transform.position - headTransform.position).magnitude - 1f;
            // 奥行きはモデル座標とカメラ間の90%と設定
            mousePos.z = (currentCamera.transform.position - leftHandTransform.position).magnitude * 0.90f;
            Vector3 pos = currentCamera.ScreenToWorldPoint(mousePos);
            targetObject.transform.position = pos;
        }
    }

    /// <summary>
    /// マウスカーソルの方を見る動作
    /// </summary>
    private void UpdateHand()
    {
    }
}
