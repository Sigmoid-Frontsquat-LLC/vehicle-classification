using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ClickedWaveAnimation : MonoBehaviour {

	public GameObject WaveObject;
	public GameObject CanvasMain;
	public Vector2 defaultSize = new Vector2(256, 256);
	public Vector2 freeSize = new Vector2(64, 64);
	public bool inhertColor = true;
	public Color defaultColor = Color.gray;
	public UnityEvent onWaveExecute = new UnityEvent();

	public int PoolSize;

	private Pool poolClass;

	void Start()
	{
		poolClass = gameObject.AddComponent<Pool>();
		poolClass.CreatePool(WaveObject, PoolSize);

		for(int i = 0; i < this.PoolSize; ++i) {
			var go = poolClass.Objects[i];

			go.transform.SetParent(this.CanvasMain.transform);

			var monoEvent = go.GetComponent<MonoEventHook>();

			if(monoEvent) {
				monoEvent.onDisable.AddListener(() => {
					monoEvent.transform.SetParent(this.CanvasMain.transform);
				});
			}
		}
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown(0) 
#if UNITY_EDITOR
		    || Input.GetMouseButtonDown(1) 
#endif
		    )
		{
			GameObject hittedUIButton = UiHitted();

			if (hittedUIButton)
			{


				CreateWave(hittedUIButton.transform, hittedUIButton.GetComponent<Mask>() == null);

				this.onWaveExecute.Invoke();
			}
		}
	}

	void CreateWave(Transform Parent, bool center = false)
	{
		GameObject wave = poolClass.GetObject();

		if (wave)
		{
			wave.transform.SetParent( CanvasMain.transform );

			var rectTransform = wave.GetComponent<RectTransform>();

			if(center) {
				//wave.GetComponent<MaskableGraphic>().color = this.defaultColor;

				wave.transform.SetParent(Parent);
				rectTransform.sizeDelta = this.freeSize;
				rectTransform.anchoredPosition = Vector3.zero;
			} else {
				Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

				mousePos.x = mousePos.x * Screen.width - Screen.width / 2f;
				mousePos.y = mousePos.y * Screen.height - Screen.height / 2f;
				mousePos.z = 0f;

				rectTransform.sizeDelta = this.defaultSize;

				rectTransform.localPosition = mousePos / CanvasMain.transform.localScale.x;

				wave.transform.SetParent(Parent);

				wave.GetComponent<MaskableGraphic>().color = Parent.GetComponent<MaskableGraphic>().color - new Color(.1f, .1f, .1f);
			}

			wave.GetComponent<EasyTween>().OpenCloseObjectAnimation();
		}
	}

	public GameObject UiHitted()
	{
		PointerEventData pe = new PointerEventData(EventSystem.current);
		pe.position =  Input.mousePosition;
		
		List<RaycastResult> hits = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pe, hits);

		hits.Sort((a, b) => {
			if(a.depth < b.depth) return 1;

			if(a.depth > b.depth) return -1;

			return 0;
		});

		//for(int i = 0; i < hits.Count; i++) {
		//	Debug.Log(hits[i].gameObject.name);
		//	//if(hits[i].gameObject.GetComponent<Button>()) {
		//	//	return hits[i].gameObject;
		//	//}
		//}

		if(hits[0].gameObject.GetComponent<Button>()) {
			return hits[0].gameObject;
		}

		var button = hits[0].gameObject.GetComponentInParent<Button>();

		if(button) {
			return button.gameObject;
		}

		return null;
	}
}

public class Pool : MonoBehaviour {

	private GameObject[] ObjectPool;
	private GameObject ObjectToPool;

	public void CreatePool(GameObject ObjectToPool, int numberOfObjects)
	{
		ObjectPool = new GameObject[numberOfObjects];
		this.ObjectToPool = ObjectToPool;

		for (int i = 0; i < ObjectPool.Length; i++)
		{
			ObjectPool[i] = Instantiate(ObjectToPool) as GameObject;
			ObjectPool[i].hideFlags = HideFlags.HideInHierarchy;
			ObjectPool[i].SetActive(false);
		}
	}

	public GameObject GetObject()
	{
		for (int i = 0; i < ObjectPool.Length; i++)
		{
			if (ObjectPool[i])
			{
				if (!ObjectPool[i].activeSelf)
				{
					ObjectPool[i].SetActive(true);
					return ObjectPool[i];
				}
			}
			else
			{
				ObjectPool[i] = Instantiate(ObjectToPool) as GameObject;
				ObjectPool[i].SetActive(false);
			}
		}

		return null;
	}

	public GameObject[] Objects {
		get {
			return this.ObjectPool;
		}
	}
}