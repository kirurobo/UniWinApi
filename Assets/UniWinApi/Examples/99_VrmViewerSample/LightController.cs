using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : CameraController {
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			HandleMouse();
		}
	}
}
