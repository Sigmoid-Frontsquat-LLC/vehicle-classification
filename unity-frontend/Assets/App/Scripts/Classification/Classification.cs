using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public sealed class Classification : Singleton<Classification> {
	private Dictionary<object, System.Action<string, float>> processes = new Dictionary<object, System.Action<string, float>>();

	public static string Url { get; set; } = "http://127.0.0.1";
	public static int Port { get; set; } = 3000;

	public static void Classify(object owner, byte[] image, System.Action<string, float> callback) {
		var processes = Instance.processes;

		if(processes.ContainsKey(owner)) {
			callback(null, 0.0F);
			return;
		}

		processes.Add(owner, callback);

		Instance.StartCoroutine(Instance.DoClassify(image, owner));
	}

	private IEnumerator DoClassify(byte[] image, object owner) {
		var classificationRequest = new ClassificationRequest {
			Image = System.Convert.ToBase64String(image)
		};

		var request = WebRequest.CreateApiPostRequest(Classification.Url, Classification.Port, "/classify", classificationRequest);

		yield return request.SendWebRequest();

		if(request.isNetworkError) {
			Debug.LogError("Network: " + request.error);
			yield break;
		}

		if(request.isHttpError) {
			Debug.LogError("Http:" + request.error);
			yield break;
		}

		var process = this.processes[owner];

		var rawResponse = request.downloadHandler.text;

		Debug.Log("Response: " + rawResponse);

		request.Dispose();
	}

	[System.Serializable]
	class ClassificationRequest {
		public string Image { get; set; }
	}
}