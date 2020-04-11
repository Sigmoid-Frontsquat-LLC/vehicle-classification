using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ClickedAudio : MonoBehaviour {
	public AudioClip clip;
	[Range(0.0F, 1.0F)]
	public float volume = 1.0F;

	private void Start() {
		AudioPool.Register(this);
	}

	private void OnDestroy() {
		AudioPool.Unregister(this);
	}

	public virtual void Play() {
		AudioPool.Play(this, this.clip, this.volume);
	}
}