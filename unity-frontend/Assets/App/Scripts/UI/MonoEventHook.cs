using UnityEngine;
using UnityEngine.Events;

public class MonoEventHook : MonoBehaviour {
	public UnityEvent onEnable = new UnityEvent();
	public UnityEvent onDisable = new UnityEvent();

	protected virtual void OnEnable() {
		this.onEnable.Invoke();
	}

	protected virtual void OnDisable() {
		this.onDisable.Invoke();
	}
}