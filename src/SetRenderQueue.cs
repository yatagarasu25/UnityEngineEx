﻿/*
	SetRenderQueue.cs

	Sets the RenderQueue of an object's materials on Awake. This will instance
	the materials, so the script won't interfere with other renderers that
	reference the same materials.

	Author: Neil Carter (NCarter) and Daniel Brauer (Danielbrauer)
	Source: http://wiki.unity3d.com/index.php?title=DepthMask
*/

using UnityEngine;

namespace UnityEngineEx
{
	[AddComponentMenu("Rendering/SetRenderQueue")]
	public class SetRenderQueue : MonoBehaviour
	{
		[SerializeField] protected int[] m_queues = new int[] { 3000 };

		protected void Awake()
		{
			Material[] materials = GetComponent<Renderer>().materials;
			for (int i = 0; i < materials.Length && i < m_queues.Length; ++i) {
				materials[i].renderQueue = m_queues[i];
			}
		}
	}
}
