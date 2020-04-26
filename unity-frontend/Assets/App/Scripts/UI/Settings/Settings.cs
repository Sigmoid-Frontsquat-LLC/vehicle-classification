using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Settings : UIMenu {
	public InputField inputIP;
	public InputField inputPort;
	public RectTransform test;

	protected virtual void OnEnable() {
		this.inputIP.text = Classification.Url;
		this.inputPort.text = Classification.Port.ToString();
	}

	public virtual void OnIPChanged(string value) {
		Classification.Url = value;
	}

	public virtual void OnPortChanged(string value) {
		int port;

		if(int.TryParse(value, out port)) {
			Classification.Port = port;
		}
	}

	public virtual void Test() {
		this.test.gameObject.SetActive(true);
	}
}