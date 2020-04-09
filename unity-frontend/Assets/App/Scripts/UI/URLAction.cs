﻿using UnityEngine;

public class URLAction : MonoBehaviour {
	public string url;

	public virtual void Open() {
		Application.OpenURL(this.url);
	}
}