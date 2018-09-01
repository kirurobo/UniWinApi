/**
 * CameraController
 * 
 * Author: Kirurobo http://twitter.com/kirurobo
 * License: CC0 https://creativecommons.org/publicdomain/zero/1.0/
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [Flags]
    public enum RotationAxes {
        None = 0,
        Pitch = 1,
        Yaw = 2,
        PitchAndYaw = 3
    }

	[Flags]
	public enum WheelMode
	{
		None = 0,
		Dolly = 1,	// Dolly in/out
		//Zoom = 2,		// Not implemented
	}

    public RotationAxes axes = RotationAxes.PitchAndYaw;
	public WheelMode wheelMode = WheelMode.Dolly;
    public float sensitivityX = 15f;
    public float sensitivityY = 15f;
    public float dragSensitivity = 1f;
    public float wheelSensitivity = 0.2f;

    public Vector2 minimumAngles = new Vector2(-90f, -360f);
    public Vector2 maximumAngles = new Vector2( 90f,  360f);

    public Transform centerTransform;   // 回転中心

    Vector3 rotation;
    Vector3 translation;
    float distance;

    Vector3 relativePosition;
    Quaternion relativeRotation;
    float originalDistance;
    float wheel;

    Camera currentCamera;

    void Start()
    {
        if (!centerTransform)
        {
            centerTransform = this.transform.parent;
            if (!centerTransform || centerTransform == this.transform)
            {
                centerTransform = new GameObject().transform;
                centerTransform.position = Vector3.zero;
            }
        }

		relativePosition = centerTransform.position - transform.position;	// カメラから中心座標へのベクトル
        relativeRotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        originalDistance = relativePosition.magnitude;

		ResetTransform();

        currentCamera = GetComponent<Camera>();
    }

	/// <summary>
	/// Reset rotation and translation.
	/// </summary>
    public void ResetTransform()
    {
		rotation = relativeRotation.eulerAngles;
        distance = originalDistance;
        wheel = 0f;

		UpdateTransform();
    }

	/// <summary>
	/// Apply rotation and translation
	/// </summary>
	private void UpdateTransform()
	{
		Quaternion rot = Quaternion.Euler(rotation);
		transform.rotation = rot;
		transform.position = centerTransform.position + transform.rotation * Vector3.back * distance;
	}

	void Update()
    {
        if (!currentCamera.isActiveAndEnabled) return;

        if (Input.GetMouseButton(1))
        {
            // 右ボタンドラッグで回転
            if ((axes & RotationAxes.Yaw) > RotationAxes.None)
            {
                rotation.y += Input.GetAxis("Mouse X") * sensitivityX;
                rotation.y = ClampAngle(rotation.y, minimumAngles.y, maximumAngles.y);
			}
			if ((axes & RotationAxes.Pitch) > RotationAxes.None)
            {
                rotation.x -= Input.GetAxis("Mouse Y") * sensitivityY;
                rotation.x = ClampAngle(rotation.x, minimumAngles.x, maximumAngles.x);
			}
			UpdateTransform();
        }
        else if (Input.GetMouseButton(2))
        {
            //// 中ボタンドラッグで並進移動
            //Vector3 screenVector = new Vector3(
            //    Input.GetAxis("Mouse X") * dragSensitivity,
            //    Input.GetAxis("Mouse Y") * dragSensitivity,
            //    0f
            //    );
            //translation -= transform.rotation * (screenVector * translationCoef);
            //transform.localPosition = translation + rotationCenter - (transform.rotation * rotationCenter);
        }
        else
        {
			// ホイールで接近・離脱
			float wheelDelta = Input.GetAxis("Mouse ScrollWheel") * wheelSensitivity;

			if (wheelDelta != 0f)
			{
				if (wheelMode == WheelMode.Dolly)
				{
					// ドリーの場合。カメラを近づけたり遠ざけたり。
					wheel += wheelDelta;
					if (wheel > 5f) wheel = 5f;     // 上限
					if (wheel < -2f) wheel = -2f;   // 下限

					distance = originalDistance * Mathf.Pow(10f, -wheel);

					UpdateTransform();
				}
			}
		}
    }

    /// <summary>
    /// 指定範囲から外れる角度の場合、補正する
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -min) angle = -((-angle) % 360f);
        if (angle > max) angle = angle % 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
