using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class FlyoutShell : UIMenu {
	[SerializeField]
	private RectTransform content;
	[SerializeField]
	private Flyout flyout;
	[SerializeField]
	private RectTransform navigation;
	[SerializeField]
	private Text menuTitle;

	private List<UIMenu> menus = new List<UIMenu>();

	private UIMenu current;

	protected override void Start() {
		base.Start();

		this.flyout.gameObject.SetActive(true);

		Debug.Log(Screen.safeArea.size);
		Debug.Log(Screen.safeArea.position);
		Debug.Log("W: " + Screen.width + " H: " + Screen.height);
	}

	protected virtual void OnEnable() {
		this.adaptive.GetComponent<Crystal.SafeArea>().onRefresh.AddListener(this.OnRefresh);
	}

	protected virtual void OnDisable() {
		this.adaptive.GetComponent<Crystal.SafeArea>().onRefresh.RemoveListener(this.OnRefresh);
	}

	public override bool OnBack() {
		if(this.flyout.gameObject.activeSelf) {
			this.flyout.Close();
		}

		return base.OnBack();
	}

	public virtual void Open(string name) {
		if(this.current != null) {
			if(this.current != this.menus[0]) {
				this.menus.Remove(this.current);
				Destroy(this.current.gameObject);
			} else {
				this.current.gameObject.SetActive(false);
				this.current.transform.SetParent(this.transform);
			}
		}

		var index = this.menus.FindIndex(i => string.Compare(i.name, name) == 0);

		UIMenu menu = null;

		if(index >= 0) {
			menu = this.menus[index];

			menu.transform.SetParent(this.content);
			menu.gameObject.SetActive(true);
		} else {
			menu = UIManager.Create<UIMenu>(name);
			this.menus.Add(menu);
		}

		var rectTransform = menu.GetComponent<RectTransform>();

		menu.AutoAttach = false;

		menu.transform.SetParent(this.content);

		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;
		rectTransform.sizeDelta = Vector2.zero;
		rectTransform.anchoredPosition = Vector2.zero;

		menu.Open();

		this.flyout.Close();

		this.menuTitle.text = menu.Title;

		this.current = menu;

		this.current.transform.localScale = Vector3.one;
	}
}
