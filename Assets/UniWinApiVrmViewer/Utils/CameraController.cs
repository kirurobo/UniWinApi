/**
 * CameraController
 * 
 * Author: Kirurobo http://twitter.com/kirurobo
 * License: CC0 https://creativecommons.org/publicdomain/zero/1.0/
 */

using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [Flags]
    public enum RotationAxes
    {
        None = 0,
        Pitch = 1,
        Yaw = 2,
        PitchAndYaw = 3
    }

    [Flags]
    public enum ZoomType
    {
        None = 0,
        Dolly = 1,	// Dolly in/out
        Zoom = 2,	// Zoom in/out
    }

    public RotationAxes axes = RotationAxes.PitchAndYaw;
    [FormerlySerializedAs("zoomMode")] public ZoomType zoomType = ZoomType.Dolly;
    public float sensitivityX = 15f;
    public float sensitivityY = 15f;
    public float dragSensitivity = 0.1f;
    public float wheelSensitivity = 0.5f;

    public Vector2 minimumAngles = new Vector2(-90f, -360f);
    public Vector2 maximumAngles = new Vector2(90f, 360f);

    [Tooltip("None means to set the parent transform")]
    public Transform centerTransform;   // 回転中心

    private GameObject centerObject = null; // 回転中心Transformが指定されなかった場合に作成される

    internal Vector3 rotation;
    internal Vector3 translation;

    [SerializeField]
    internal float distance;

    internal Vector3 relativePosition;
    internal Quaternion relativeRotation;
    internal float originalDistance;
    internal float originalFieldOfView;
    internal float dolly;
    internal float zoom;

    internal Camera currentCamera;

    void Start()
    {
        Initialize();
        SetupTransform();
    }

    void OnDestroy()
    {
        // 回転中心を独自に作成していれば、削除
        if (centerObject) GameObject.Destroy(centerObject);
    }

    void Update()
    {
        if (!currentCamera.isActiveAndEnabled) return;
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            HandleMouse();
        }
    }

    /// <summary>
    /// 必要なオブジェクトを取得・準備
    /// </summary>
    internal void Initialize()
    {
        if (!centerTransform)
        {
            centerTransform = this.transform.parent;
            if (!centerTransform || centerTransform == this.transform)
            {
                centerObject = new GameObject();
                centerTransform.position = Vector3.zero;
                centerTransform = centerObject.transform;
            }
        }

        if (!currentCamera)
        {
            currentCamera = GetComponent<Camera>();
        }
        if (!currentCamera)
        {
            currentCamera = Camera.main;
        }
    }

    /// <summary>
    /// 初期位置・姿勢の設定
    /// 対象となるオブジェクトがそろった後で実行する
    /// </summary>
    internal void SetupTransform()
    {
        relativePosition = centerTransform.position - transform.position;	// カメラから中心座標へのベクトル
        relativeRotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        originalDistance = relativePosition.magnitude;

        originalFieldOfView = currentCamera.fieldOfView;

        ResetTransform();
    }

    /// <summary>
    /// Reset rotation and translation.
    /// </summary>
    public void ResetTransform()
    {
        rotation = relativeRotation.eulerAngles;
        translation = Vector3.zero;
        distance = originalDistance;
        dolly = 0f;
        zoom = Mathf.Log10(originalFieldOfView);

        UpdateTransform();
    }

    /// <summary>
    /// Apply rotation and translation
    /// </summary>
    internal void UpdateTransform()
    {
        Quaternion rot = Quaternion.Euler(rotation);
        transform.rotation = rot;
        transform.position = centerTransform.position + transform.rotation * Vector3.back * distance + transform.rotation * translation;

        currentCamera.fieldOfView = Mathf.Pow(10f, zoom);
    }

    internal virtual void HandleMouse()
    {
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
            // 中ボタンドラッグで並進移動
            Vector3 screenVector = new Vector3(
                Input.GetAxis("Mouse X") * dragSensitivity,
                Input.GetAxis("Mouse Y") * dragSensitivity,
                0f
                );
            //translation -= transform.rotation * screenVector;
            translation -= screenVector;
            UpdateTransform();
        }
        else
        {
            // ホイールで接近・離脱
            float wheelDelta = Input.GetAxis("Mouse ScrollWheel") * wheelSensitivity;

            ZoomType type = zoomType;

            // Shiftキーが押されていて、かつZoomModeがZoomかDollyならば、モードを入れ替える
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (type == ZoomType.Dolly)
                {
                    type = ZoomType.Zoom;
                }
                else if (type == ZoomType.Zoom)
                {
                    type = ZoomType.Dolly;
                }
            }

            if (wheelDelta != 0f)
            {
                if ((type & ZoomType.Dolly) != ZoomType.None)
                {
                    // ドリーの場合。カメラを近づけたり遠ざけたり。
                    dolly += wheelDelta;
                    dolly = Mathf.Clamp(dolly, -2f, 5f);	// Logarithm of distance [m] range

                    distance = originalDistance * Mathf.Pow(10f, -dolly);

                    UpdateTransform();
                }
                else if ((type & ZoomType.Zoom) != ZoomType.None)
                {
                    // ズームの場合。カメラのFOVを変更
                    zoom -= wheelDelta;
                    zoom = Mathf.Clamp(zoom, -1f, 2f);	// Logarithm of field-of-view [deg] range

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
