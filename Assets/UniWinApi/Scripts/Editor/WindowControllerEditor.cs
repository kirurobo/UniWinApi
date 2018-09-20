using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(WindowController))]
public class WindowControllerEditor : Editor {
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		// 自動調整ボタンを表示させるならtrueとなる
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

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class WindowControllerReadOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUI.enabled = false;
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = true;
	}
}

/// <summary>
/// Set to readonly during playing
/// Reference: http://ponkotsu-hiyorin.hateblo.jp/entry/2015/10/20/003042
/// Reference: https://forum.unity.com/threads/c-class-property-with-reflection-in-propertydrawer-not-saving-to-prefab.473942/
/// </summary>
[CustomPropertyDrawer(typeof(BoolPropertyAttribute))]
public class WindowControllerDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//base.OnGUI(position, property, label);

		if (EditorApplication.isPlayingOrWillChangePlaymode)
		{
			if ((property.type == "bool") &&  (property.name[0] == '_'))
			{
				Object obj = property.serializedObject.targetObject;
				string propertyName = property.name.Substring(1);
				PropertyInfo info = obj.GetType().GetProperty(propertyName);
				MethodInfo getMethod = default(MethodInfo);
				MethodInfo setMethod = default(MethodInfo);
				if (info.CanRead) { getMethod = info.GetGetMethod(); }
				if (info.CanWrite) { setMethod = info.GetSetMethod(); }

				bool oldValue = property.boolValue;
				if (getMethod != null)
				{
					oldValue = (bool)getMethod.Invoke(obj, null);
				}
				GUI.enabled = (setMethod != null);
				EditorGUI.PropertyField(position, property, label, true);
				GUI.enabled = true;
				bool newValue = property.boolValue;
				if ((setMethod != null) && (oldValue != newValue))
				{
					setMethod.Invoke(obj, new[] { (object)newValue });
				}
			}
			else
			{
				// Readonly
				GUI.enabled = false;
				EditorGUI.PropertyField(position, property, label, true);
				GUI.enabled = true;
			}
		} else
		{
			// Default value
			EditorGUI.PropertyField(position, property, label, true);
		}

	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}
}
