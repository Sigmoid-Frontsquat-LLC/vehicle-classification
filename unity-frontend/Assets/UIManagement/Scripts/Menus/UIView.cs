using UnityEngine;

public abstract class UIView : MonoBehaviour {
	[Header("Localization")]
	[SerializeField, Lang]
	private string lang = "en";
	[SerializeField]
	private Localization localization;
	[SerializeField]
	private string title;

	public string Lang {
		get {
			return this.lang;
		} set {
			this.lang = value;

			this.UpdateLocalization();
		}
	}

	public Localization Localization {
		get {
			return this.localization;
		} set {
			this.localization = value;

			this.UpdateLocalization();
		}
	}

	public string Title {
		get {
			return this.title;
		}
	}

	protected virtual void OnEnable() {
		//DataManager.Instance.AppData.Subscribe<string>("Lang", this.OnLangChanged);

		//this.OnLangChanged(DataManager.Instance.AppData.Lang);
	}

	protected virtual void OnDisable() {
		//if(DataManager.Instance == null) return;

		//DataManager.Instance.AppData.Unsubscribe<string>("Lang", this.OnLangChanged);
	}

	protected virtual void UpdateLocalization() { }

	protected virtual void OnLangChanged(string lang) {
		this.Lang = lang;
	}
}