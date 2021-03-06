﻿using MathEx;
using System;
using UnityEngine;

namespace UnityEngineEx
{
	public class GizmosColor : IDisposable
	{
		Color oldColor;

		public GizmosColor(Color color)
		{
			oldColor = Gizmos.color;
			Gizmos.color = color;
		}

		public void Dispose()
		{
			Gizmos.color = oldColor;
		}
	}

	public static class GizmosEx
	{
		public static GizmosColor Color(Color color)
		{
			return new GizmosColor(color);
		}

		public static void DrawLineTo(Vector3 origin, Vector3 direction, Color color)
		{
			using (Color(color))
			{
				Gizmos.DrawLine(origin, origin + direction);
			}
		}

		public static void DrawSphere(Transform t, Vector3 center, float radius)
		{
			Gizmos.DrawSphere(t.TransformPoint(center), t.localScale.Min() * radius);
		}
	}
}
