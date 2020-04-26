using UnityEngine;
using UnityEngine.UI;

public class Settings : UIMenu {
	public InputField inputIP;
	public InputField inputPort;

	protected virtual void OnEnable() {
		this.inputIP.text = Classification.Endpoint;
		this.inputPort.text = Classification.Port.ToString();
	}

	public virtual void OnIPChanged(string value) {
		Classification.Endpoint = value;
	}

	public virtual void OnPortChanged(string value) {
		int port;

		if(int.TryParse(value, out port)) {
			Classification.Port = port;
		}
	}
}