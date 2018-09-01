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

        if (PlayerSettings.defaultIsFullScreen)
        {
            EditorGUILayout.HelpBox("'Default is full screen' is no recommended.", MessageType.Warning);
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
    }
}
