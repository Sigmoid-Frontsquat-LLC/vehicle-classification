using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomInputModule : StandaloneInputModule {
	public AudioClip onClickSound;
	public GameObject ripple;
	public int maxRipples = 5;
	[Range(0.0F, 1.0F)]
	public float volume = 1.0F;
	[Range(-3.0F, 3.0F)]
	public float pitch = 1.0F;

	private ObjectPool<GameObject> ripples = null;

	protected override void Awake() {
		base.Awake();

		this.ripples = new ObjectPool<GameObject>
			.Builder()
			.Create(() => {
				var clone = Object.Instantiate(this.ripple);

				clone.transform.SetParent(this.transform);

				var tween = clone.GetComponent<EasyTween>();

				tween.animationParts.ExitEvents.AddListener(() => {
					clone.transform.SetParent(this.transform);
				});

				return clone;
			})
			.IsAvailable((ripple) => {
				return ripple && ripple.activeSelf == false;
			})
			.Destroy((ripple) => {
				if(ripple) {
					Destroy(ripple.gameObject);
				}
			})
			.Max(this.maxRipples)
			.Build();
	}

	protected override void Start() {
		base.Start();

		AudioPool.Register(this);
	}

	protected override void OnDestroy() {
		base.OnDestroy();

		AudioPool.Unregister(this);
	}

	protected override void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released) {
		base.ProcessTouchPress(pointerEvent, pressed, released);

		var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEvent.pointerCurrentRaycast.gameObject);

		if(pressed) {
			Selectable selectable = null;

			if(pointerUpHandler) {
				selectable = pointerUpHandler.GetComponent<Selectable>();

				if(selectable == null) selectable = pointerUpHandler.GetComponentInParent<Selectable>();

				if(selectable) {
					this.ExecuteRipple(pointerEvent, selectable.gameObject);
				}
			}
		}

		if(released) {
			if(pointerUpHandler) {
				Selectable selectable = pointerUpHandler.GetComponent<Selectable>();

				if(selectable == null) selectable = pointerUpHandler.GetComponentInParent<Selectable>();

				if(selectable) {
					AudioPool.Play(this, this.onClickSound, this.volume, this.pitch);
				}
			}
		}
	}

	protected override void ProcessMousePress(MouseButtonEventData data) {
		base.ProcessMousePress(data);

		var pointerEventData = data.buttonData;

		if(data.PressedThisFrame()) {
			var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEventData.pointerCurrentRaycast.gameObject);

			if(pointerUpHandler == null) return;

			var selectable = pointerUpHandler.GetComponent<Selectable>();
			selectable = selectable ? selectable : pointerUpHandler.GetComponentInParent<Selectable>();

			if(selectable) {
				this.ExecuteRipple(pointerEventData, selectable.gameObject);
			}
		}
	}

	protected override void ReleaseMouse(PointerEventData pointerEvent, GameObject currentOverGo) {
		base.ReleaseMouse(pointerEvent, currentOverGo);

		var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

		if(pointerUpHandler && pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick) {
			var selectable = pointerUpHandler.GetComponentInParent<Selectable>();

			if(selectable) {
				AudioPool.Play(this, this.onClickSound, this.volume);
			}
		}
	}

	protected virtual void ExecuteRipple(PointerEventData pointerEvent, GameObject target) {
		GameObject ripple;

		if(this.ripples.GetAvailable(out ripple)) {
			var canvas = target.transform.FindInParent<Canvas>();
			var isCenter = target.GetComponent<Mask>() == null;
			var rectTransform = ripple.GetComponent<RectTransform>();
			rectTransform.SetParent(canvas.transform);
			var targetSize = target.GetComponent<RectTransform>().rect.size;
			var sizeDelta = Vector2.one * Mathf.Max(targetSize.x, targetSize.y);

			if(isCenter) {
				rectTransform.SetParent(target.transform);
				rectTransform.sizeDelta = sizeDelta;
				rectTransform.anchoredPosition = Vector2.zero;
			} else {
				var position = Camera.main.ScreenToViewportPoint(pointerEvent.position);

				position.x = position.x * Screen.width - Screen.width / 2.0F;
				position.y = position.y * Screen.height - Screen.height / 2.0F;
				position.z = 0.0F;

				rectTransform.sizeDelta = sizeDelta;
				rectTransform.localPosition = position / canvas.transform.localScale.x;

				rectTransform.SetParent(target.transform);
			}

			var tween = ripple.GetComponent<EasyTween>();
			tween.OpenCloseObjectAnimation();
		}
	}
}