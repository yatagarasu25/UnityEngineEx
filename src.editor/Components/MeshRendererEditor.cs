﻿using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditorEx.Components
{
	[CustomEditor(typeof(MeshRenderer))]
	class MeshRendererEditor : Editor<MeshRenderer>
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			Renderer t = target;

			GUILayout.BeginHorizontal();
			GUILayout.Label("Sorting Layer");
			string[] layerNames = GetSortingLayerNames();
			int selected = Array.IndexOf(layerNames, t.sortingLayerName);
			selected = EditorGUILayout.Popup(selected, layerNames);
			if (selected >= 0)
			{
				t.sortingLayerName = layerNames[selected];
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Order in layer");
			t.sortingOrder = EditorGUILayout.IntField(t.sortingOrder);
			GUILayout.EndHorizontal();
		}

		// Get the sorting layer names
		public string[] GetSortingLayerNames()
		{
			Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			return (string[])sortingLayersProperty.GetValue(null, new object[0]);
		}
	}
}
