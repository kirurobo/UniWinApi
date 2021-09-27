/**
 * LightController
 * 
 * Author: Kirurobo http://twitter.com/kirurobo
 * License: CC0 https://creativecommons.org/publicdomain/zero/1.0/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : CameraController {

	// Update is called once per frame
	void Update () {
		// [Shift]が押されていれば、照明の回転を行う
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			HandleMouse();
		}
	}

	internal override void HandleMouse()
	{
		if (Input.GetMouseButton(1))
		{
			// 右ボタンドラッグで照明を回転
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
		else
		{
			// ホイールで環境光の明るさを増減
			float wheelDelta = Input.GetAxis("Mouse ScrollWheel") * wheelSensitivity;

			if (wheelDelta != 0f)
			{
				//float intensity = RenderSettings.ambientIntensity;
				//Mathf.Clamp(intensity - wheelDelta, 0f, 8f);
				//RenderSettings.ambientIntensity = intensity;
				Color color = RenderSettings.ambientLight;
				float v = Mathf.Clamp(color.r + wheelDelta, 0f, 1f);
				color.r = color.g = color.b = v;
				RenderSettings.ambientLight = color;
			}
		}
	}
}
