using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraView : UIMenu {
	public CameraPreview cameraPreview;
	public RawImage output;
	public RectTransform target;
	private float fragment = 0.0F;
	private float[] samples = new float[5];

	protected virtual void OnEnable() {
		this.cameraPreview.Play();

		this.StartCoroutine("_CameraSample");
		this.StartCoroutine("_Sample");
	}

	protected virtual void OnDisable() {
		this.cameraPreview.Stop();

		this.StopCoroutine("_CameraSample");
		this.StopCoroutine("_Sample");

		for(int i = 0; i < this.samples.Length; ++i) this.samples[i] = 0.0F;
	}

	IEnumerator _CameraSample() {
		var time = new WaitForSecondsRealtime(2.5F);
		var endOfFrame = new WaitForEndOfFrame();

		int index = 0;

		while(true) {
			if(this.cameraPreview.IsRendering == false) {
				yield return null;
				continue;
			}

			this.samples[index] = this.fragment;

			index = (index + 1) % this.samples.Length;

			if(index == 0) {
				var sum = 0.0F;

				for(int i = 0; i < this.samples.Length; ++i) {
					sum += this.samples[i];

					this.samples[0] = 0.0F;
				}

				var avg = sum / this.samples.Length;

				if(avg >= 10.0F) {
					yield return null;

					continue;
				}
			} else {
				yield return null;
				continue;
			}

			this.target.gameObject.SetActive(false);
			yield return endOfFrame;
			this.SnapShot();
			this.target.gameObject.SetActive(true);
			yield return time;
		}
	}

	IEnumerator _Sample() {
		var last_acc = Input.acceleration.magnitude;

		while(true) {
			var new_acc = Input.acceleration.magnitude;

			var delta = 1000.0F * (new_acc - last_acc);

			this.fragment = Mathf.Abs(Mathf.Round(delta));

			last_acc = new_acc;

			yield return null;
		}
	}

	public virtual void SnapShot() {
		ScreenShot.CaptureByBounds(
			this.target,
			Camera.main,
			(texture) => {
				TextureScale.Bilinear(texture, 128, 128);

				this.output.texture = texture;
			});
	}

	public virtual void Toggle() {
		if(this.cameraPreview.IsRendering) {
			this.cameraPreview.Stop();
		} else {
			this.cameraPreview.Play();
		}
	}
}
