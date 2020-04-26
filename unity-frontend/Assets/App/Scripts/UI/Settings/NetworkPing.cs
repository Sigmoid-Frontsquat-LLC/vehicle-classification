using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkPing : MonoBehaviour {
	public Text result;
	public Button close;

	protected virtual void OnEnable() {
		this.StartCoroutine("_Ping");
	}

	protected virtual void OnDisable() {
		this.StopCoroutine("_Ping");
	}

	protected IEnumerator _Ping() {
		this.close.interactable = false;
		this.result.text = "Pinging..";

		using(var request = WebRequest.CreateApiGetRequest(Classification.Url, Classification.Port, "/")) {
			yield return request.SendWebRequest();

			if(request.isHttpError) {
				this.result.text = "Error - HTTP: " + request.error;

				this.close.interactable = true;

				yield break;
			}

			if(request.isNetworkError) {
				this.result.text = "Error - Network: " + request.error;

				this.close.interactable = true;

				yield break;
			}

			this.result.text = request.downloadHandler.text;

			this.close.interactable = true;
		}
	}
}