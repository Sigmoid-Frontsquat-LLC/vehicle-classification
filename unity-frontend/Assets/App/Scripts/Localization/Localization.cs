using UnityEngine;

[CreateAssetMenu(menuName = "Localization", fileName = "Localization")]
public class Localization : ScriptableObject {
	/// <summary>
	/// <seealso cref="https://en.wikipedia.org/wiki/Language_localisation"/>
	/// </summary>
	public static readonly string[] Supported = new string[] {
		"en",
		"es",
		"ru",
		"uk"
	};

	[SerializeField]
	private Phrase[] phrases = new Phrase[0];

	public Phrase[] Phrases {
		get {
			return this.phrases;
		}
	}

	public bool GetPhrase(string tag, out Phrase phrase) {
		var result = System.Array.Find(this.phrases, i => string.Compare(tag, i.Tag) == 0);

		if(result == null) {
			phrase = null;

			return false;
		}

		phrase = result;

		return true;
	}

	[System.Serializable]
	public class Phrase : System.Object {
		[SerializeField]
		private string tag = string.Empty;
		[SerializeField, Lang]
		private string fallback = string.Empty;
		[SerializeField]
		private Local[] locals = new Local[0];

		public string Tag {
			get {
				return this.tag;
			}
		}

		public bool GetLocal(string lang, out Local local) {
			var result = System.Array.Find(this.locals, i => string.Compare(lang, i.Lang) == 0);

			if(result == null) {
				result = System.Array.Find(this.locals, i => string.Compare(this.fallback, i.Lang) == 0);

				if(result == null) {
					local = null;
					return false;
				}
			}

			local = result;

			return true;
		}

		[System.Serializable]
		public class Local : System.Object {
			[SerializeField, Lang]
			private string lang = string.Empty;
			[SerializeField]
			private string text = string.Empty;

			public string Lang {
				get {
					return this.lang;
				}
			}

			public string Text {
				get {
					return this.text;
				}
			}
		}
	}
}