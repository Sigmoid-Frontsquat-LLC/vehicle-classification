using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ClickedAudio : MonoBehaviour {
	public AudioClip clip;

	private void Start() {
		AudioPool.Register(this);
	}

	private void OnDestroy() {
		AudioPool.Unregister(this);
	}

	public virtual void Play() {
		AudioPool.Play(this, this.clip);
	}
}