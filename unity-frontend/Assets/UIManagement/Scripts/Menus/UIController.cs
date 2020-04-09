using UnityEngine;

public abstract class UIController<T> : UIMenu where T : UIView {
	private T view;

	public T View {
		get {
			return this.view;
		} set {
			if(this.View) this.Detach();

			this.view = value;

			this.Attach();
		}
	}

	protected abstract void Detach();
	protected abstract void Attach();
}