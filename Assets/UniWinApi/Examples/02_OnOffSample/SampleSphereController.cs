using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSphereController : MonoBehaviour {
	/// <summary>
	/// 
	/// </summary>
	Vector3 initialPosition;

	Vector3 rotationAxis = new Vector3(1f, 1f, 0f).normalized;
	float rotationAngle = 0f;
	float distance = 1.5f;
	float speed = 90f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		// ぐるぐる回す
		rotationAngle = Time.time * speed;
		transform.position = Quaternion.AngleAxis(rotationAngle, rotationAxis) * Vector3.forward * distance;
	}
}
