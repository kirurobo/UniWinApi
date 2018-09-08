using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WindowController))]
public class WindowControllerEditor : Editor {
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		bool showButton = false;

		if (!PlayerSettings.runInBackground)
		{
			EditorGUILayout.HelpBox("'Run in background' is recommended.", MessageType.Warning);
			showButton = true;
		}

		if (!PlayerSettings.resizableWindow)
		{
			EditorGUILayout.HelpBox("'Resizable window' is recommended.", MessageType.Warning);
			showButton = true;
		}

#if UNITY_2018_1_OR_NEWER
		if (PlayerSettings.fullScreenMode != FullScreenMode.Windowed)
		{
			EditorGUILayout.HelpBox("It is recommmended to select 'Windowed' in fullscreen mode.", MessageType.Warning);
			showButton = true;
		}

		if (showButton)
		{
			if (GUILayout.Button("Apply all recommended settings"))
			{
				PlayerSettings.runInBackground = true;
				PlayerSettings.resizableWindow = true;
				PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
			}
		}
#else
		if (PlayerSettings.defaultIsFullScreen)
		{
			EditorGUILayout.HelpBox("'Default is full screen' is not recommended.", MessageType.Warning);
			showButton = true;
		}

		if (showButton) {
			if (GUILayout.Button("Apply all recommended settings"))
			{
				PlayerSettings.runInBackground = true;
				PlayerSettings.resizableWindow = true;
				PlayerSettings.defaultIsFullScreen = false;
			}
		}
#endif
	}
}
