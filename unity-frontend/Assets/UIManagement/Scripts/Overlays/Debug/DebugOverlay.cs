using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugOverlay : MonoBehaviour {
	[RuntimeInitializeOnLoadMethod]
	static void Init() {
		var asset = Resources.Load<GameObject>("UI/Debug");

		if(asset == null) return;

		var go = GameObject.Instantiate(asset.gameObject);

		DontDestroyOnLoad(go);
	}

	static string LOG_MESSAGE = "> <color=\"#{0}\">{1}</color>\n";

	[SerializeField]
	private Text log;

	private void Awake() {
		Application.logMessageReceived += Application_logMessageReceived;
	}

	private void OnDestroy() {
		Application.logMessageReceived -= Application_logMessageReceived;
	}

	private void Application_logMessageReceived(string condition, string stackTrace, LogType type) {
		this.StopCoroutine("_Clear");

		//if(this.log.text.Split('\n').Length >= 25) {
		//	this.log.text = string.Empty;
		//}

		var color = Color.green;

		switch(type) {
			case LogType.Error:
				color = Color.red;
				break;
			case LogType.Warning:
				color = Color.yellow;
				break;
			case LogType.Assert:
				color = Color.blue;
				break;
		}

		this.log.text += string.Format(LOG_MESSAGE, ColorUtility.ToHtmlStringRGBA(color), condition);

		this.StartCoroutine("_Clear");
	}

	private IEnumerator _Clear() {
		yield return new WaitForSeconds(10.0F);

		this.log.text = "";
	}
}