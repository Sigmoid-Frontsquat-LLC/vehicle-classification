using UnityEngine;
using System.Collections.Generic;

public sealed class AudioPool : Singleton<AudioPool> {
	private Dictionary<object, ObjectPool<AudioSource>> pools = new Dictionary<object, ObjectPool<AudioSource>>();

	public static void Register(object owner) {
		if(Instance == null) return;

		if(Instance.pools.ContainsKey(owner) == false) {
			var pool = new ObjectPool<AudioSource>.Builder()
				.Create(() => {
					var source = new GameObject("Source").AddComponent<AudioSource>();

					source.transform.SetParent(Instance.transform);

					//source.gameObject.hideFlags = HideFlags.HideInHierarchy;

					return source;
				}).IsAvailable((source) => {
					return source.isPlaying == false || source.gameObject.activeSelf == false;
				}).Destroy((source) => {
					if(source) {
						Destroy(source.gameObject);
					}
				}).Build();

			Instance.pools.Add(owner, pool);
		}
	}

	public static void Unregister(object owner) {
		if(Instance == null) return;

		if(Instance.pools.ContainsKey(owner)) {
			var pool = Instance.pools[owner];

			pool.Dispose();

			Instance.pools.Remove(owner);
		}
	}

	public static void Play(object owner, AudioClip clip) {
		AudioPool.Play(owner, clip, 1.0F);
	}

	public static void Play(object owner, AudioClip clip, float volume) {
		AudioPool.Play(owner, clip, volume, 1.0F);
	}

	public static void Play(object owner, AudioClip clip, float volume, float pitch) {
		if(Instance == null) return;

		if(Instance.pools.ContainsKey(owner) == false) return;

		var pool = Instance.pools[owner];

		AudioSource source;

		if(pool.GetAvailable(out source)) {
			source.clip = clip;
			source.volume = volume;
			source.pitch = pitch;
			source.Play();
		}
	}
}