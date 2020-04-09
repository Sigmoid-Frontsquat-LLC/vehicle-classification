using UnityEngine;

public class Rotate : MonoBehaviour {
	[SerializeField]
	private Vector3 speed = new Vector3(10.0F, 10.0F, 10.0F);


	private void Update() {
		var rotation = Quaternion.Euler(speed * Time.deltaTime);

		this.transform.rotation *= rotation;
	}
}