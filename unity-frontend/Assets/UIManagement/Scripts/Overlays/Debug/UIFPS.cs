using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIFPS : MonoBehaviour {
	private Text text;

	int frameCount = 0;
	float nextUpdate = 0.0F;
	float fps = 0.0F;
	float updateRate = 4.0F;

	private void Start() {
		this.text = this.GetComponent<Text>();
	}

	private void Update() {
		++this.frameCount;

		this.nextUpdate += Time.unscaledDeltaTime;

		if(this.nextUpdate >= (1.0F / this.updateRate)) {
			this.fps = this.frameCount * this.updateRate;

			this.frameCount = 0;

			this.nextUpdate -= (1.0F / this.updateRate);

			this.text.text = string.Format("{0:00} fps", (int)this.fps);
		}
	}
}
