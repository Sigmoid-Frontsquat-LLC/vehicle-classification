using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Flyout : MonoBehaviour {
	[SerializeField]
	private RectTransform content;
	[SerializeField]
	private RectTransform body;
	[SerializeField]
	private CanvasGroup background;
	[SerializeField]
	private RectTransform handle;
	[SerializeField]
	private RectTransform block;
	[SerializeField]
	private FlyoutItemData[] items = new FlyoutItemData[0];
	[SerializeField]
	private FlyoutItem flyoutItemTemplate;

	LTDescr runningTween = null;
	private Image highlight = null;
	private Text text;
	private Color textColor = Color.clear;

	public RectTransform Content {
		get {
			return this.content;
		} set {
			this.content = value;
		}
	}

	public FlyoutItemData[] Items {
		get {
			return this.items;
		}
	}

	public Image Highlight {
		get {
			if(this.highlight == null) {
				var clone = new GameObject("[Highlight]");
				clone.transform.SetParent(this.transform);
				clone.SetActive(false);
				clone.AddComponent<RectTransform>();

				this.highlight = clone.AddComponent<Image>();
				this.highlight.raycastTarget = false;
			}

			return this.highlight;
		}
	}

	private void Awake() {
		LeanTween.value(0.0F, 1.0F, 0.01F)
				.setOnUpdate((float t) => {
					this.Content.anchoredPosition = Vector2.Lerp(Vector2.zero, Vector2.left * this.Content.rect.size.x, t);

					this.background.alpha = Mathf.Lerp(0.5F, 0.0F, t);
				})
				.setOnStart(() => {
					this.background.blocksRaycasts = false;
					this.Content.gameObject.SetActive(false);
				})
				.setEaseOutBack()
				.setOnComplete(() => {
					this.runningTween = null;
				});
	}

	private void Start() {
		for(int i = 0; i < this.items.Length; ++i) {
			var data = this.items[i];
			var flyoutItem = Object.Instantiate(this.flyoutItemTemplate.gameObject).GetComponent<FlyoutItem>();
			flyoutItem.name = "[Button] " + this.items[i].menuId;

			flyoutItem.Label.text = this.items[i].text;
			flyoutItem.Icon.sprite = this.items[i].icon;

			if(flyoutItem.Icon.sprite == null) {
				flyoutItem.Icon.gameObject.SetActive(false);
			}

			if(this.items[i].useOnClickEvent) {
				flyoutItem.Button.onClick.AddListener(
					() => {
						data.onClick.Invoke();

						if(data.hideAfterClick) this.Close();

				});
			} else {
				flyoutItem.Button.onClick.AddListener(() => {
					var shell = this.GetComponentInParent<FlyoutShell>();

					shell.Open(data.menuId);

					if(data.hideAfterClick) this.Close();

					var graphic = flyoutItem.Button.targetGraphic;

					var color = graphic.color * flyoutItem.Button.colors.pressedColor;

					this.Highlight.transform.SetParent(flyoutItem.Button.transform);
					this.Highlight.transform.SetAsFirstSibling();

					color.a = 0.5F;

					if(this.text) {
						this.text.color = this.textColor;
					}

					this.Highlight.color = color;

					var rect = this.Highlight.rectTransform;

					rect.anchorMin = Vector2.zero;
					rect.anchorMax = Vector2.one;
					rect.sizeDelta = Vector2.zero;
					rect.anchoredPosition = Vector2.zero;
					rect.localScale = Vector3.one;

					this.Highlight.gameObject.SetActive(true);

					float h, s, v;
					Color.RGBToHSV(color, out h, out s, out v);

					if(v <= 50.0F) {
						s = 0.0F;
						v = 100.0F;

						var contrast = Color.HSVToRGB(h, s, v);

						var text = flyoutItem.Button.GetComponentInChildren<Text>();

						if(text) {
							this.textColor = text.color;
							text.color = contrast;
						}
					} else {
						s = 0.0F;
						v = 0.0F;

						var contract = Color.HSVToRGB(h, s, v);

						var text = flyoutItem.Button.GetComponentInChildren<Text>();
					}

					this.text = flyoutItem.Button.GetComponentInChildren<Text>();
				});
			}

			flyoutItem.transform.SetParent(this.body);

			flyoutItem.transform.localScale = Vector3.one;

			flyoutItem.gameObject.SetActive(true);
		}

		var first_child = this.body.GetChild(1);

		var flyout = first_child.GetComponent<FlyoutItem>();

		if(flyout != null) {
			ExecuteEvents.Execute(flyout.Button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
		}
	}

	public virtual void Open() {
		this.Open(0.0F, 1.0F, 0.75F, LeanTweenType.easeOutBack);
	}

	public virtual void Close() {
		this.Close(0.0F, 1.0F, 0.75F, LeanTweenType.easeOutBack);
	}

	private Vector2 start;

	public virtual void OnBeginDrag(BaseEventData eventData) {
		this.start = this.handle.rect.center;
		this.Content.gameObject.SetActive(true);
		this.block.gameObject.SetActive(true);
	}

	public virtual void OnDrag(BaseEventData eventData) {
		var pointer = eventData as PointerEventData;

		var deltaX = pointer.position.x - this.start.x;
		var progress = deltaX / (Screen.width / 2.0F);

		var position = Vector2.Lerp(Vector2.left * this.Content.rect.size.x, Vector2.zero, progress);

		this.Content.anchoredPosition = position;

		this.background.alpha = Mathf.Lerp(0.0F, 1.0F, progress);
	}

	public virtual void OnEndDrag(BaseEventData eventData) {
		var pointer = eventData as PointerEventData;

		var iLerp = Mathf.InverseLerp(0.0F, 1.0F, this.background.alpha);

		var deltaX = pointer.position.x - this.start.x;
		var progress = deltaX / (Screen.width / 2.0F);

		if(progress >= 0.5F) {
			this.Open(iLerp, 1.0F, 0.15F, LeanTweenType.notUsed);
		} else {
			this.Close(1.0F - iLerp, 1.0F, 0.1F, LeanTweenType.notUsed);
		}

		this.block.gameObject.SetActive(false);
	}

	protected virtual void Open(float from, float to, float time, LeanTweenType leanTween) {
		if(this.runningTween != null) return;

		this.runningTween = LeanTween.value(from, to, time)
				.setOnUpdate((float t) => {
					this.Content.anchoredPosition = Vector2.Lerp(Vector2.left * this.Content.rect.size.x, Vector2.zero, t);

					this.background.alpha = Mathf.Lerp(0.0F, 1.0F, t);
				})
				.setOnStart(() => {
					this.block.gameObject.SetActive(true);
					this.background.blocksRaycasts = true;
					this.Content.gameObject.SetActive(true);
				})
				.setEase(leanTween)
				.setOnComplete(() => {
					this.background.blocksRaycasts = true;
					this.runningTween = null;
					this.block.gameObject.SetActive(false);
				});
	}

	protected virtual void Close(float from, float to, float time, LeanTweenType leanTween) {
		if(this.runningTween != null) return;

		this.runningTween = LeanTween.value(from, to, time)
				.setOnUpdate((float t) => {
					this.Content.anchoredPosition = Vector2.Lerp(Vector2.zero, Vector2.left * this.Content.rect.size.x, t);

					this.background.alpha = Mathf.Lerp(1.0F, 0.0F, t);

					if(t > 0.8F) {
						this.block.gameObject.SetActive(false);
						this.background.blocksRaycasts = false;
					}
				})
				.setOnStart(() => {
					this.block.gameObject.SetActive(true);
					this.background.blocksRaycasts = true;
				})
				.setEase(leanTween)
				.setOnComplete(() => {
					this.runningTween = null;
					this.Content.gameObject.SetActive(false);
				});
	}

	[System.Serializable]
	public class FlyoutItemData {
		public string text;
		public string menuId;
		public Sprite icon;
		public bool useOnClickEvent = false;
		public bool hideAfterClick = false;
		public UnityEvent onClick = new UnityEvent();
	}
}
