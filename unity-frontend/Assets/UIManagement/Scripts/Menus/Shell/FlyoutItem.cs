using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FlyoutItem : MonoBehaviour {
	[SerializeField]
	private Image icon;
	[SerializeField]
	private Text label;

	private Button button;

	public Button Button {
		get {
			if(this.button == null) this.button = this.GetComponent<Button>();

			return this.button;
		}
	}

	public Image Icon {
		get {
			return this.icon;
		}
	}

	public Text Label {
		get {
			return this.label;
		}
	}
}