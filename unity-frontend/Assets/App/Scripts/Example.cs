using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Example : MonoBehaviour {
	public Angle angle = Angle._0;
	public RectTransform rectTransform;
	public Vector2 size = new Vector2(0, 1);
	public Vector2 adjusted = Vector2.zero;

	private void Update() {
		this.rectTransform.rotation = Quaternion.Euler(0.0F, 0.0F, 360.0F - (int)this.angle);

		this.adjusted = this.rectTransform.rotation * this.size;
		this.adjusted.x = Mathf.Round(this.adjusted.x);
		this.adjusted.y = Mathf.Round(this.adjusted.y);
	}

	public enum Angle {
		_0 = 0,
		_90 = 90,
		_180 = 180,
		_270 = 270,
		_360 = 360
	}
}
