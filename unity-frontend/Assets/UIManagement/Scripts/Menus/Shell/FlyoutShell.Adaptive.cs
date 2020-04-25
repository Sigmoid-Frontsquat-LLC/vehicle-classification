using UnityEngine;
using UnityEngine.UI;

public partial class FlyoutShell {
	[Header("Adaptive")]
	[SerializeField]
	private RectTransform adaptive;
	[SerializeField]
	private Color navigatorTint = Color.white;

	private Image notchArea;

	public virtual void OnSafeAreaChanged(Vector2 min, Vector2 max) {
		var flyout = this.flyout.GetComponent<RectTransform>();

		var minAnchor = flyout.anchorMin;
		minAnchor.y = -min.y - 0.01F;
		flyout.anchorMin = minAnchor;
		this.content.anchorMin = minAnchor;

		var maxAnchor = flyout.anchorMax;
		maxAnchor.y = 1.0F - max.y + 1.01F;
		flyout.anchorMax = maxAnchor;

		var yDelta = Mathf.Abs(1.0F - max.y);

		// we have a notch
		if(yDelta > Mathf.Epsilon) {
			if(this.notchArea == null) {
				var go = new GameObject("Notch Area");

				go.transform.SetParent(this.transform);
				go.transform.SetAsFirstSibling();
				go.AddComponent<RectTransform>();
				go.transform.localScale = Vector3.one;

				this.notchArea = go.AddComponent<Image>();
			}

			this.notchArea.transform.SetParent(this.transform);
			this.notchArea.transform.SetAsFirstSibling();

			this.notchArea.color = this.navigation.GetComponent<Image>().color * this.navigatorTint;

			this.notchArea.rectTransform.anchorMin = new Vector2(0.0F, max.y);
			//this.notchArea.rectTransform.anchorMax = new Vector2(1.0F, 1.01F);
			this.notchArea.rectTransform.anchorMax = Vector2.one;
			this.notchArea.rectTransform.sizeDelta = Vector2.zero;
			this.notchArea.rectTransform.anchoredPosition = Vector2.zero;

			this.notchArea.rectTransform.SetParent(this.navigation);
			this.notchArea.rectTransform.SetAsFirstSibling();
		} else {
			if(this.notchArea) {
				Destroy(this.notchArea.gameObject);
			}
		}
	}

	protected virtual void OnAdaptiveValidation() {
		Graphic graphic;
		if(this.notchArea && this.navigation && (graphic = this.navigation.GetComponent<Graphic>())) {
			this.notchArea.color = graphic.color * this.navigatorTint;
		}
	}
}