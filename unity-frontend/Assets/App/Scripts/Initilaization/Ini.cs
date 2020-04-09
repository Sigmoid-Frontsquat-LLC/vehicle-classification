using UnityEngine;

public sealed class Ini : MonoBehaviour {
	[RuntimeInitializeOnLoadMethod]
	static void OnLoad() {
		Application.targetFrameRate = 60;
	}
}
