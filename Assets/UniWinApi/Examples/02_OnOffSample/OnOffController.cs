using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnOffController : MonoBehaviour {
	WindowController windowController;

	public Toggle transparentToggle;
	public Toggle topmostToggle;
	public Toggle maximizedToggle;
	public Toggle minimizedToggle;
	public Toggle enableFileDropToggle;
	public Text droppedFilesText;

	// Use this for initialization
	void Start () {
		// 同じゲームオブジェクトに WindowController をアタッチしておくこと
		windowController = GetComponent<WindowController>();

		// ファイルドロップ時の処理
		windowController.OnFilesDropped += (string[] files) =>
		{
			if (droppedFilesText)
			{
				// ドロップされたファイルのパスを表示
				droppedFilesText.text = string.Join("\n", files);
			}
		};

		if (transparentToggle) transparentToggle.isOn = windowController.isTransparent;
		if (topmostToggle) topmostToggle.isOn = windowController.isTopmost;
		if (maximizedToggle) maximizedToggle.isOn = windowController.isMaximized;
		if (minimizedToggle) minimizedToggle.isOn = windowController.isMinimized;
		if (enableFileDropToggle) enableFileDropToggle.isOn = windowController.enableFileDrop;
	}

	// Update is called once per frame
	void Update () {

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

		// ファイルドロップ受け付け状態を切り替え
		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
		{
			windowController.enableFileDrop = !windowController.enableFileDrop;
		}
	}
}
