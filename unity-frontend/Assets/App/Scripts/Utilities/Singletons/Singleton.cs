using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
	private static T instance = null;
	private static object mutex = new object();
	private static bool isApplicationQuitting = false;

	public static T Instance {
		get {
			if(isApplicationQuitting) return null;

			lock(mutex) {
				if(instance == null) {
					instance = Object.FindObjectOfType<T>();

					if(instance == null) {
						var clone = new GameObject(typeof(T).Name);

						clone.SetActive(false);

						instance = clone.AddComponent<T>();

						clone.SetActive(true);
					}

					DontDestroyOnLoad(instance.gameObject);
				}

				return instance;
			}
		}
	}

	protected virtual void Awake() {
		if(Instance != this) Destroy(this.gameObject);
	}

	protected virtual void OnDestroy() {
		instance = null;
		mutex = null;
	}

	protected virtual void OnApplicationQuit() {
		isApplicationQuitting = true;
	}
}