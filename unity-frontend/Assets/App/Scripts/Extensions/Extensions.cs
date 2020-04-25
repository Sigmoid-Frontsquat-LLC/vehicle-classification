using UnityEngine;

public static class Extensions {
	public static T FindInParent<T>(this Transform root) where T : Component {
		if(root == null) return null;

		if(root.parent == null) return null;

		var component = root.parent.GetComponent<T>();

		if(component) return component;

		return root.parent.FindInParent<T>();
	}

	public static T FindInParent<T>(this GameObject root) where T : Component {
		return root.transform.FindInParent<T>();
	}
}