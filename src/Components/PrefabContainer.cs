﻿using UnityEngine;

namespace UnityEngineEx
{
	[ExecuteInEditMode]
	public class PrefabContainer : MonoBehaviour
	{
		[SerializeField] GameObject prefab = null;

		void Awake()
		{
			Execute();
		}

		public void Execute()
		{
#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying)
				gameObject.Reinstantiate(prefab);
#else
			gameObject.Reinstantiate(prefab);
#endif
		}

#if UNITY_EDITOR
		GameObject prevPrefab = null;
		void Update()
		{
			if (prevPrefab != prefab)
			{
				gameObject.transform.Clear();
				prefab.Construct(gameObject).CallRecursive((GameObject o) => o.tag = "EditorOnly");
				prevPrefab = prefab;
			}
		}

		public void DoReset()
		{
			prevPrefab = null;
			Update();
		}
#else
		public void DoReset()
		{
		}
#endif
	}
}
