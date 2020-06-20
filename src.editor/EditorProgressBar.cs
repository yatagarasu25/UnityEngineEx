using System.Linq;
using System.Reflection;
using UnityEditor;



namespace UnityEditorEx
{
	public static class EditorProgressBar
	{
		static MethodInfo m_Display = null;
		static MethodInfo m_Clear = null;

		static EditorProgressBar()
		{
			var type = typeof(Editor).Assembly.GetTypes().Where(t => t.Name == "AsyncProgressBar").FirstOrDefault();
			if (type != null)
			{
				m_Display = type.GetMethod("Display");
				m_Clear = type.GetMethod("Clear");
			}
		}

		public static void ShowProgressBar(string aText, float aProgress)
		{
			if (m_Display != null)
				m_Display.Invoke(null, new object[] { aText, aProgress });
		}
		public static void ClearProgressBar()
		{
			if (m_Clear != null)
				m_Clear.Invoke(null, null);
		}
	}
}
