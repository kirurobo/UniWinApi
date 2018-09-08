/**
 * VrmSample
 * 
 * Author: Kirurobo http://twitter.com/kirurobo
 * License: CC0 https://creativecommons.org/publicdomain/zero/1.0/
 */

using System;
using System.IO;
using UniHumanoid;
using UnityEngine;
using VRM;

public class VrmSample : MonoBehaviour {

	private WindowController windowController;

	private VRMImporterContext context;
	private HumanPoseTransfer model;
	private HumanPoseTransfer motion;
	private VRMMetaObject meta;

	public VrmUiController uiController;
	public CameraController cameraController;
	public Transform cameraTransform;

	private CameraController.WheelMode originalWheelMode;


	// Use this for initialization
	void Start () {
		if (!uiController)
		{
			uiController = FindObjectOfType<VrmUiController>();
		}

		if (!cameraController)
		{
			cameraController = FindObjectOfType<CameraController>();
			if (cameraController)
			{
				originalWheelMode = cameraController.wheelMode;
			}
		}

		// Load the initial motion.
		LoadMotion(Application.streamingAssetsPath + "/default_bvh.txt");

		// Load the initial model.
		string[] cmdArgs = System.Environment.GetCommandLineArgs();
		if (cmdArgs.Length > 1)
		{
			LoadModel(cmdArgs[1]);
		} else
		{
			LoadModel(Application.streamingAssetsPath + "/default_vrm.vrm");
		}

		// Initialize window manager
		windowController =FindObjectOfType<WindowController>();
		if (windowController)
		{
			// Add a file drop handler.
			windowController.OnFilesDropped += Window_OnFilesDropped;
		}
	}

	void Update()
	{
		// 透明なところではホイール操作は受け付けなくする
		if (windowController && cameraController)
		{
			Vector2 pos = Input.mousePosition;
			bool inScreen = (pos.x >= 0 && pos.x < Screen.width && pos.y >= 0 && pos.y < Screen.height);
			if (!windowController.isClickThrough && inScreen)
			{
				cameraController.wheelMode = originalWheelMode;
			} else
			{
				cameraController.wheelMode = CameraController.WheelMode.None;
			}
		}

		// End を押すとウィンドウ透過切替
		if (Input.GetKeyDown(KeyCode.End))
		{
			windowController.SetTransparent(!windowController.isTransparent);
		}

		// Home を押すと最前面切替
		if (Input.GetKeyDown(KeyCode.Home))
		{
			windowController.SetTopmost(!windowController.isTopmost);
		}
		// F11 を押すと最大化切替
		if (Input.GetKeyDown(KeyCode.F11))
		{
			windowController.SetMaximized(!windowController.isMaximized);
		}

		// Insert を押すと最小化切替
		if (Input.GetKeyDown(KeyCode.Insert))
		{
			windowController.SetMinimized(!windowController.isMinimized);
		}
	}

	/// <summary>
	/// A handler for file dropping.
	/// </summary>
	/// <param name="files"></param>
	private void Window_OnFilesDropped(string[] files)
	{
		foreach (string path in files)
		{
			string ext = path.Substring(path.Length - 4).ToLower();

			// Open the VRM file if its extension is ".vrm".
			if (ext == ".vrm")
			{
				LoadModel(path);
				break;
			}

			// Open the motion file if its extension is ".bvh" or ".txt".
			if (ext == ".bvh" || ext == ".txt")
			{
				LoadMotion(path);
				break;
			}
		}
	}

	/// <summary>
	/// Apply the motion to the model.
	/// </summary>
	/// <param name="motion"></param>
	/// <param name="model"></param>
	/// <param name="meta"></param>
	private void SetMotion(HumanPoseTransfer motion, HumanPoseTransfer model, VRMMetaObject meta)
	{
		if (!model || !motion || !meta) return;

		// Apply the motion if AllowedUser is equal to "Everyone".
		if (meta.AllowedUser == AllowedUser.Everyone)
		{
			model.Source = motion;
			model.SourceType = HumanPoseTransfer.HumanPoseTransferSourceType.HumanPoseTransfer;
		}
	}

	/// <summary>
	/// Load the motion from a BVH file.
	/// </summary>
	/// <param name="path"></param>
	private void LoadMotion(string path)
	{
		ImporterContext context = new ImporterContext
		{
			Path = path
		};

		BvhImporter.Import(context);
		motion = context.Root.GetComponent<HumanPoseTransfer>();
		motion.GetComponent<Renderer>().enabled = false;

		SetMotion(motion, model, meta);
	}

	/// <summary>
	/// Unload the old model and load the new model from VRM file.
	/// </summary>
	/// <param name="path"></param>
	private void LoadModel(string path)
	{
		if (!File.Exists(path)) return;

		GameObject newModel = null;

		try
		{
			// Load from a VRM file.
			byte[] bytes = File.ReadAllBytes(path);
			context = new VRMImporterContext(UniGLTF.UnityPath.FromFullpath(path));
			context.ParseGlb(bytes);
			VRMImporter.LoadFromBytes(context);

			newModel = context.Root;
			meta = context.ReadMeta();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
			return;
		}

		if (model)
		{
			GameObject.Destroy(model.gameObject);
		}

		if (newModel)
		{
			model = newModel.AddComponent<HumanPoseTransfer>();
			SetMotion(motion, model, meta);

			model.gameObject.AddComponent<CharacterBehaviour>();


			if (uiController)
			{
				uiController.Show(meta);
			}
		}
	}
}
