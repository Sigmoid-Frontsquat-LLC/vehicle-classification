using UnityEngine;

public sealed class UIModule : MonoBehaviour {
	private void Update() {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			UIManager.OnBack();
		}
	}
}