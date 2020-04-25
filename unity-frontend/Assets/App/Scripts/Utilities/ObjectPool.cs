using System.Collections.Generic;

/// <summary>
/// A generic ObjectPool for handeling the reusability of objects. This is a great way
/// to manage resources to prevent either too many objects from being created or as it's primary purpose,
/// to reuse objects to control the garbage collection invocation.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectPool<T> : System.IDisposable {
	// A generic list of T objects that will be poolable.
	private List<T> pool = null;
	// A hard limit on the number of objects that can be created.
	private int max = 0;

	// A returnable lamba function to handle the creation of the object T.
	private System.Func<T> create = null;
	// A returnable lamba function to handle if an object is available to be reused.
	private System.Func<T, bool> isAvailable = null;
	// A non-returnable function to destroy/free the object T.
	private System.Action<T> destroy = null;

	/// <summary>
	/// Constructs an ObjectPool object of type T. This will handeling the recycling of objects for better
	/// memory management and also to prevent spikes in garbage collection when creating and destroying objects
	/// frequently.
	/// </summary>
	/// <param name="create">A returnable lamba function to handle the creation of the object T.</param>
	/// <param name="isAvailable">A returnable lamba function to handle if an object is available to be reused.</param>
	/// <param name="destroy">A non-returnable function to destroy/free the object T.</param>
	/// <param name="max">[Optional] Set a hard limit on the number of objects that can be created and maintained.</param>
	public ObjectPool(System.Func<T> create, System.Func<T, bool> isAvailable, System.Action<T> destroy, int max = 0) {
		this.create = create;
		this.isAvailable = isAvailable;
		this.destroy = destroy;

		if(max > 0) {
			this.pool = new List<T>(capacity: max);
			this.max = max;
		} else {
			this.pool = new List<T>();
			this.max = int.MaxValue;
		}
	}

	/// <summary>
	/// Attempt to get the next available objects for reuse. If no object is found in the current pool, it will attempt
	/// to create a new one if the number of objects already pooled is less than the hard limit.
	/// </summary>
	/// <param name="instance">Instance to be assgined when a reusable object is found.</param>
	/// <returns>Returns true if a reusable object was found, else returns false.</returns>
	public bool GetAvailable(out T instance) {
		// Check the objects already pooled.
		for(int i = 0; i < this.pool.Count; ++i) {
			// Is this pooled object ready for reuse?
			if(this.isAvailable(this.pool[i])) {
				// If so, assign it to instance...
				instance = this.pool[i];

				// ...return true that we did find a reusable object.
				return true;
			}
		}

		// At this point we did not found an object ready for reuse.
		// Check to make sure the number of objects does not exceed the hard limit.
		if(this.pool.Count < this.max) {
			// If it does not create a new instance of type T using the create method passed at creation.
			instance = this.create();

			// add the new item to the pool
			this.pool.Add(instance);

			// return true.
			return true;
		}

		// we have met the hard limit, assign a default instance to instance...
		instance = default(T);

		// ...return false, as we were not able to create a new instance of type T.
		return false;
	}

	/// <summary>
	/// Invoke dispose when you want to dump the contents of this ObjectPool. After this is called,
	/// the state of this object is in a null state, attempting to call methods may result in null-refs or a crash.
	/// </summary>
	public void Dispose() {
		for(int i = 0; i < this.pool.Count; ++i) {
			this.destroy(this.pool[i]);
		}

		this.pool.Clear();
		this.pool = null;
		this.create = null;
		this.isAvailable = null;
		this.destroy = null;
		this.max = -1;
	}

	[System.Serializable]
	public class Builder : System.Object {
		System.Func<T> create;
		System.Func<T, bool> isAvailale;
		System.Action<T> destroy;
		int max = 0;

		public Builder Create(System.Func<T> callback) {
			this.create = callback;

			return this;
		}

		public Builder IsAvailable(System.Func<T,bool> callback) {
			this.isAvailale = callback;

			return this;
		}

		public Builder Destroy(System.Action<T> callback) {
			this.destroy = callback;

			return this;
		}

		public Builder Max(int value) {
			this.max = value;

			return this;
		}

		public ObjectPool<T> Build() {
			return new ObjectPool<T>(
				this.create,
				this.isAvailale,
				this.destroy,
				this.max);
		}
	}
}