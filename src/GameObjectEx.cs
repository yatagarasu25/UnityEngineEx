﻿using MathEx;
using System;
using System.Collections;
using System.Collections.Generic;
using SystemEx;
using UnityDissolve;
using UnityEngine;



namespace UnityEngineEx {
	public static class GameObjectEx {
		public static readonly string RootTag = "RootTag";
		public static readonly string SubObjectTag = "SubObject";

		#region Enumerators

		/// <summary>
		/// Enumerates GameObject children.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static IEnumerable<GameObject> GetEnumerator(this GameObject o) {
			foreach (Transform child in o.transform)
				yield return child.gameObject;
			yield break;
		}

		/// <summary>
		/// Enumerates all GameObject children recursively.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static IEnumerable<GameObject> GetEnumeratorRecursive(this GameObject o) {
			Stack<Transform> next = new Stack<Transform>();

			next.Push(o.transform);
			while (next.Count != 0) {
				Transform current = next.Pop();
				foreach (Transform child in current) {
					yield return child.gameObject;
					next.Push(child);
				}
			}
			yield break;
		}

		/// <summary>
		/// Enumerates all GameObject children recursively preserving depth of recursion.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static IEnumerable<Tuple<GameObject, int>> GetEnumeratorRecursiveWithDepth(this GameObject o) {
			Stack<Tuple<Transform, int>> next = new Stack<Tuple<Transform, int>>();

			next.Push(Tuple.Create(o.transform, 0));
			while (next.Count != 0) {
				var current = next.Pop();
				foreach (Transform child in current.Item1) {
					yield return Tuple.Create(child.gameObject, current.Item2 + 1);
					next.Push(Tuple.Create(child, current.Item2 + 1));
				}
			}
			yield break;
		}

		public static IEnumerable<T> Enumerate<T>(this GameObject o)
			where T : Component {
			foreach (Transform child in o.transform) {
				T c = child.GetComponent<T>();
				if (c != null)
					yield return c;
			}
		}

		#endregion

		#region Creation

		/// <summary>
		/// Creates child object with a given name at given local position.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="name"></param>
		/// <param name="po"></param>
		/// <param name="components"></param>
		/// <returns></returns>
		public static GameObject Create(this GameObject o, string name, Vector3 po, params Type[] components) {
			GameObject i = new GameObject(name, components);
			i.transform.position = po;
			o.transform.Add(i);
			return i;
		}

		public static GameObject Create(this GameObject o, string name, params Type[] components) {
			return o.Create(name, Vector3.zero, components);
		}

		public static GameObject Create(this GameObject o, string name, ActionContainer ctor, params Type[] components) {
			return o.Create(name, Vector3.zero, ctor, components);
		}

		public static GameObject Create(this GameObject o, string name, Vector3 po, ActionContainer ctor, params Type[] components) {
			var go = o.Create(name, po, components);
			go.Dissolve(ctor);
			return go;
		}

		public static GameObject Create(this GameObject o, string name, Vector3 po, Mesh mesh, params Type[] components) {
			var go = o.Create(name, po, components);
			go.AddComponent<MeshRenderer>();
			go.AddComponent<MeshFilter>().mesh = mesh;
			return go;
		}

		public static GameObject Create(this GameObject o, string name, Mesh mesh, params Type[] components) {
			return o.Create(name, Vector3.zero, mesh, components);
		}

		public static GameObject Create(this GameObject o, string name, Vector3 po, Sprite sprite, params Type[] components) {
			var go = o.Create(name, po, components);
			go.AddComponent<SpriteRenderer>().sprite = sprite;
			return go;
		}

		public static GameObject Create(this GameObject o, string name, Sprite sprite, params Type[] components) {
			return o.Create(name, Vector3.zero, sprite, components);
		}

		public static GameObject Create(this GameObject o, string name, Vector3 po, string text, params Type[] components) {
			var go = o.Create(name, po, components);
			go.AddComponent<TextMesh>().text = text;
			return go;
		}

		public static GameObject Create(this GameObject o, string name, string text, params Type[] components) {
			return o.Create(name, Vector3.zero, text, components);
		}

		#endregion

		public static TComponent CreateActor<TComponent>(string name, Transform parent)
			where TComponent : Component {
			var go = new GameObject(name);
			go.transform.parent = parent;

			return go.AddComponent<TComponent>();
		}


		#region Instantiation

		public static GameObject Construct(this GameObject instance) {
			GameObject go = null;

			try {
				go = GameObject.Instantiate(instance) as GameObject;
			}
			catch (Exception e) { Debug.LogException(e); }

			return go;
		}

		public static GameObject Construct(this GameObject instance, Component parent)
			=> instance.Construct(parent.gameObject);

		public static GameObject Construct(this GameObject instance, GameObject parent)
			=> instance.Construct(Containers.a((Transform t) => { parent.Add(t); }));

		public static GameObject Construct(this GameObject instance, Component parent, params ActionContainer[] initializers)
			=> instance.Construct(parent.gameObject, initializers);

		public static GameObject Construct(this GameObject instance, GameObject parent, params ActionContainer[] initializers)
			=> instance.Construct(
				ArrayEx.Concat(
					Containers.a((Transform t) => { parent.Add(t); })
					, initializers));

		public static GameObject Construct(this GameObject instance, params Tuple<Type, object>[] initializers) {
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;

				foreach (var i in initializers) try {
						var c = go.GetComponent(i.Item1);
						if (c != null)
							c.Setup(i.Item2);
					}
					catch (Exception e) { Debug.LogException(e); }

				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

		public static GameObject Construct(this GameObject instance, GameObject parent, params Tuple<Type, object>[] initializers) {
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;
				parent.Add(go);

				foreach (var i in initializers) try {
						var c = go.GetComponent(i.Item1);
						if (c != null)
							c.Setup(i.Item2);
					}
					catch (Exception e) { Debug.LogException(e); }

				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

		public static GameObject Construct(this GameObject instance, params ActionContainer[] initializers) {
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;

				foreach (var i in initializers) try {
						go.Dissolve(i);
					}
					catch (Exception e) { Debug.LogException(e); }

				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

		[Obsolete("Use Construct method instead")]
		public static GameObject New(this GameObject instance, params Tuple<Type, IDictionary<string, object>>[] initializers) {
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;

				foreach (var i in initializers) try {
						var c = go.GetComponent(i.Item1);
						if (c != null)
							c.Setup(i.Item2);
					}
					catch (Exception e) { Debug.LogException(e); }

				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

		[Obsolete("Use Construct method instead")]
		public static GameObject New(this GameObject instance, params ActionContainer[] initializers) {
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;

				foreach (var i in initializers) try {
						go.Dissolve(i);
					}
					catch (Exception e) { Debug.LogException(e); }

				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

		[Obsolete("Use Construct method instead")]
		public static GameObject New(this GameObject instance, string name, params Tuple<Type, object>[] initializers) {
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;

				if (initializers != null) {
					foreach (var i in initializers) try {
							var c = go.GetComponent(i.Item1);
							if (c != null)
								c.Setup(i.Item2);
						}
						catch (Exception e) { Debug.LogException(e); }
				}

				go.name = name;
				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

		[Obsolete("Use Construct method instead")]
		public static GameObject New(this GameObject instance, string name, Vector3 po, params Tuple<Type, object>[] initializers) {
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;

				if (initializers != null) {
					foreach (var i in initializers) try {
							var c = go.GetComponent(i.Item1);
							if (c != null)
								c.Setup(i.Item2);
						}
						catch (Exception e) { Debug.LogException(e); }
				}

				go.name = name;
				go.transform.position = po;
				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

		[Obsolete("Use Construct method instead")]
		public static GameObject New(this GameObject instance, GameObject parent, params ActionContainer[] initializers) {
			return instance.New(
				ArrayEx.Concat(
					Containers.a((Transform t) => { t.SetParent(parent.transform); })
					, initializers));
		}

		[Obsolete("Use Construct method instead")]
		public static GameObject New(this GameObject instance, GameObject parent, Vector3 po, params ActionContainer[] initializers) {
			return instance.New(
				ArrayEx.Concat(
					Containers.a((Transform t) => { t.SetParent(parent.transform); t.localPosition = po; })
					, initializers));
		}

		//[Obsolete("Instantiate semantics have changed.")]
		public static GameObject Instantiate(GameObject instance, params Tuple<Type, Action<object>>[] initializers) {
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;

				foreach (var i in initializers) try {
						var c = go.GetComponent(i.Item1);
						if (c != null)
							i.Item2(c);
					}
					catch (Exception e) { Debug.LogException(e); }

				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

		//[Obsolete("Instantiate semantics have changed.")]
		public static GameObject Instantiate(this GameObject o, GameObject instance, Vector3 po, params Tuple<Type, object>[] initializers) {
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;

				if (initializers != null) {
					foreach (var i in initializers) try {
							var c = go.GetComponent(i.Item1);
							if (c != null)
								c.Setup(i.Item2);
						}
						catch (Exception e) { Debug.LogException(e); }
				}

				go.transform.position = po;
				o.transform.Add(go);
				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

		//[Obsolete("Instantiate semantics have changed.")]
		public static GameObject Instantiate(this GameObject o, GameObject instance, params Tuple<Type, object>[] initializers) {
			return o.Instantiate(instance, Vector3.zero, initializers);
		}

		/// <summary>
		/// Instantiate new GameObject in place of GameObject.
		/// Function fully replaces one object by another.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="instance"></param>
		/// <param name="initializers"></param>
		/// <returns></returns>
		public static GameObject Reinstantiate(this GameObject o, GameObject instance, params Tuple<Type, object>[] initializers) {
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;

				foreach (var i in initializers) try {
						var c = go.GetComponent(i.Item1);
						if (c != null)
							c.Setup(i.Item2);
					}
					catch (Exception e) { Debug.LogException(e); }

				go.name = o.name;
				go.transform.position = o.transform.localPosition + go.transform.position;
				go.transform.rotation = o.transform.localRotation * go.transform.rotation;
				go.transform.localScale = o.transform.localScale;
				o.transform.parent.Add(go);
				GameObject.DestroyImmediate(o);
				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

#if MONO_BUG_IS_FIXED
		public static GameObject Reinstantiate(this GameObject o, GameObject instance, params object[] initializers)
#else
		public static GameObject Reinstantiate2(this GameObject o, GameObject instance, params object[] initializers)
#endif
		{
			GameObject go = null;
			bool a = instance.activeSelf;
			instance.SetActive(false);

			try {
				go = GameObject.Instantiate(instance) as GameObject;

				foreach (var i in initializers) try {
						Type ct = i.GetType().GetGenericArguments()[0];
						var c = go.GetComponentOrThis(ct);
						if (c != null)
							((Delegate)(i)).DynamicInvoke(c);
					}
					catch (Exception e) { Debug.LogException(e); }

				go.name = o.name;
				go.transform.position = o.transform.localPosition + go.transform.position;
				go.transform.rotation = o.transform.localRotation * go.transform.rotation;
				go.transform.localScale = o.transform.localScale;
				o.transform.parent.Add(go);
				GameObject.DestroyImmediate(o);
				go.SetActive(a);
			}
			catch (Exception e) { Debug.LogException(e); }

			instance.SetActive(a);
			return go;
		}

		#endregion

#if !UNITY_EDITOR
		public static void Destroy(this UnityEngine.Object o)
		{
			if (o == null)
				return;

			UnityEngine.Object.Destroy(o);
		}

		public static void DestroyGameObject(this GameObject o) {
			if (o != null) {
				o.Destroy();
			}
		}

		public static void DestroyGameObject(this Component c) {
			if (c != null) {
				c.gameObject.Destroy();
			}
		}
#else
		public static void Destroy(this UnityEngine.Object o, bool onValidOrCallback = false) {
			if (o == null)
				return;

			if (UnityEditor.EditorApplication.isPlaying)
				UnityEngine.Object.Destroy(o);
			else /*if (onValidOrCallback) {
				IEnumerable DeleteCoro() {
					yield return new WaitForEndOfFrame();
					UnityEngine.Object.DestroyImmediate(o);
				}
				o.StartCoroutine(DeleteCoro());
			}
			else*/
				UnityEngine.Object.DestroyImmediate(o);
		}

		public static void Destroy(this GameObject go, bool onValidOrCallback = false) {
			if (go == null)
				return;

			if (UnityEditor.EditorApplication.isPlaying)
				UnityEngine.Object.Destroy(go);
			else if (onValidOrCallback)
				CorotineEx.RunAtEndOfFrameEditor(() => UnityEngine.Object.DestroyImmediate(go));
			else
				UnityEngine.Object.DestroyImmediate(go);
		}

		public static void DestroyGameObject(this GameObject o, bool onValidOrCallback = false) {
			if (o != null) {
				o.Destroy(onValidOrCallback);
			}
		}

		public static void DestroyGameObject(this Component c, bool onValidOrCallback = false) {
			if (c != null) {
				c.gameObject.Destroy(onValidOrCallback);
			}
		}
#endif

		public static GameObject Replace(this GameObject go, GameObject newGo) {
			newGo.name = go.name;
			newGo.transform.parent = go.transform.parent;
			newGo.transform.position = go.transform.position;
			newGo.transform.rotation = go.transform.rotation;
			newGo.transform.localScale = go.transform.localScale;
			go.Destroy();
			return newGo;
		}

		/// <summary>
		/// Add a Component to the GameObject setting SerializeFields to a parameters values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		[Obsolete("Do not use this. Use AddComponent on inactive gameObject instead.")]
		public static T AddComponent<T>(this GameObject o, IDictionary<string, object> parameters) where T : Component {
			bool a = o.activeSelf;
			o.SetActive(false);

			T c = o.AddComponent<T>().Setup(parameters);

			o.SetActive(a);
			return c;
		}

		/// <summary>
		/// Add a Component to the GameObject calling a ctor on it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o"></param>
		/// <param name="ctor"></param>
		/// <returns></returns>
		[Obsolete("Do not use this. Use AddComponent on inactive gameObject instead.")]
		public static T AddComponent<T>(this GameObject o, Action<T> ctor) where T : Component {
			bool a = o.activeSelf;
			o.SetActive(false);

			T c = o.AddComponent<T>();
			ctor(c);

			o.SetActive(a);
			return c;
		}

		/// <summary>
		/// Add a Component to the GameObject calling a ctor on it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o"></param>
		/// <param name="ctor"></param>
		/// <returns></returns>
		[Obsolete("Do not use this. Use AddComponent on inactive gameObject instead.")]
		public static T AddComponent<T>(this GameObject o, ActionContainer ctor) where T : Component {
			bool a = o.activeSelf;
			o.SetActive(false);

			T c = o.AddComponent<T>();

			object[] prms = new object[ctor.args.Length];
			for (int ai = 0; ai < ctor.args.Length; ai++) {
				if (ctor.args[ai] != typeof(T))
					prms[ai] = o.GetComponentOrThis(ctor.args[ai]);
				else
					prms[ai] = c;
			}

			ctor.DynamicInvoke(prms);

			o.SetActive(a);
			return c;
		}

		/// <summary>
		/// Adds a component passing arguments to it's Counstructor method.
		/// So emulating a constructor call.
		/// Component should implement IConstructable interface and handle construction in Constructor method.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o">GameObject to add component to.</param>
		/// <param name="args">Arguments to a constructor.</param>
		/// <returns>Returns new;y added component.</returns>
		[Obsolete("Do not use this. Use AddComponent on inactive gameObject instead.")]
		public static T AddComponent<T>(this GameObject o, params object[] args) where T : Component, IConstructable {
			bool a = o.activeSelf;
			try {
				o.SetActive(false);

				T c = o.AddComponent<T>();
				c.Constructor(args);
				return c;
			}
			finally {
				o.SetActive(a);
			}
		}

		/// <summary>
		/// Adds GameObject as a child to another GameObject.
		/// Objects position and rotation are set to localPosition and localRotation.
		/// <seealso cref="TransformEx.Add"/>
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="o"></param>
		/// <returns></returns>
		public static GameObject Add(this GameObject parent, GameObject o) {
			return parent.transform.Add(o).gameObject;
		}

		/// <summary>
		/// Adds Transform as a child to another GameObject.
		/// Objects position and rotation are set to localPosition and localRotation.
		/// <seealso cref="TransformEx.Add"/>
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="child"></param>
		/// <returns></returns>
		public static GameObject Add(this GameObject parent, Transform child) {
			return parent.transform.Add(child).gameObject;
		}

		/// <summary>
		/// Set parent of GameObject short way.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public static GameObject SetParent(this GameObject o, GameObject parent) {
			o.transform.SetParent(parent.transform);
			return o;
		}

		public static GameObject SetParent<C>(this GameObject o, C parent) where C : Component {
			o.transform.SetParent(parent.gameObject.transform);
			return o;
		}

		public static GameObject SetParent(this GameObject o, GameObject parent, bool worldPositionStays) {
			o.transform.SetParent(parent.transform, worldPositionStays);
			return o;
		}

		public static GameObject SetParent<C>(this GameObject o, C parent, bool worldPositionStays) where C : Component {
			o.transform.SetParent(parent.gameObject.transform, worldPositionStays);
			return o;
		}

		/// <summary>
		/// SetActive only if needed and return ttue if active state was changed.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="active"></param>
		/// <returns></returns>
		public static bool ChangeActive(this GameObject o, bool active) {
			if (o.activeSelf != active) {
				o.SetActive(active);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Get most accurate Bounds of the GameObject.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static Bounds GetBounds(this GameObject o) {
			Bounds b = BoundsEx.Empty;
			o.CallRecursive((GameObject c) => {
				if (!c.activeInHierarchy)
					return;

				var renderer = c.GetComponent<Renderer>();
				if (renderer != null) {
					b = b.Extend(renderer.bounds);
				}
			});

			return b;
		}

		/// <summary>
		/// Get most accurate Bounds of the GameObject.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static Bounds GetBoundsLocal(this GameObject o) {
			var renderer = o.GetComponent<Renderer>();
			if (renderer != null) {
				return renderer.bounds;
			}

			return new Bounds();
		}

		/// <summary>
		/// Traverse GameObject hierarchy until find "root" object.
		/// Root object is first parent object not marked with tag SubObject.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public static GameObject GetRootObject(this GameObject o) {
			GameObject go = o;
			while (go != null && !go.CompareTag(RootTag))
				go = go.transform.parent.gameObject;

			Debug.Assert(go != null);

			return go;
		}

		/// <summary>
		/// Apply some Action recursively to GameObject and all its child objects.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static GameObject CallRecursive(this GameObject o, Action<GameObject> a) {
			a(o);
			foreach (GameObject child in o.GetEnumeratorRecursive()) {
				a(child);
			}

			return o;
		}

		/// <summary>
		/// Apply some Action recursively to GameObject and all its child objects passing depth as a parameter to action.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="a"></param>
		/// <returns></returns>
		public static GameObject CallRecursive(this GameObject o, Action<GameObject, int> a) {
			a(o, 0);
			foreach (var child in o.GetEnumeratorRecursiveWithDepth()) {
				a(child.Item1, child.Item2);
			}

			return o;
		}

		/// <summary>
		/// Calculated adequate instance id of multiple objects.
		/// </summary>
		/// <param name="oa"></param>
		/// <returns></returns>
		public static int GetInstanceID(this UnityEngine.Object[] oa) {
			int result = 0;

			for (int i = 0; i < oa.Length; i++)
				result = 37 * result + oa[i].GetInstanceID();

			return result;
		}

		public static IEnumerable<TResult> Select<TComponent, TResult>(this GameObject go, Func<TComponent, TResult> func)
			where TComponent : Component {
			foreach (var c in go.GetComponents<TComponent>()) {
				yield return func(c);
			}
		}

		public static IDisposable EnsureInactive(this GameObject go) {
			if (go.activeSelf) {
				go.SetActive(false);
				return DisposableLock.Lock(() => go.SetActive(true));
			}
			else {
				return DisposableLock.empty;
			}
		}

		public static void SetLayerRecursive(this GameObject go, string name) {
			var layer = LayerMask.NameToLayer(name);
			go.CallRecursive(go => go.layer = layer);
		}
	}
}
