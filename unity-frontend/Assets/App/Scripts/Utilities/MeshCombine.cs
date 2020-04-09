using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombine : MonoBehaviour {
	public GameObject mesh;

	IEnumerator _Wait() {
		yield return new WaitForSecondsRealtime(2.0F);

		var parent = this.mesh.transform.parent;

		this.mesh.transform.parent = null;
		this.mesh.transform.position = Vector3.zero;
		this.mesh.transform.rotation = Quaternion.identity;

		//var filters = this.GetComponentsInChildren<MeshFilter>();
		//var combine = new CombineInstance[filters.Length];

		//Debug.Log(combine == null);

		//for(int i = 0; i < filters.Length; ++i) {
		//	if(filters[i].transform == this.mesh.transform) continue;

		//	combine[i].mesh = filters[i].sharedMesh;
		//	combine[i].transform = filters[i].transform.localToWorldMatrix;

		//	filters[i].gameObject.SetActive(false);
		//}

		//this.mesh.GetComponent<MeshFilter>().mesh = new Mesh();
		//this.mesh.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
		//this.mesh.SetActive(true);
		this.AdvancedMerge();

		this.transform.parent = parent;

		this.transform.localPosition = Vector3.zero;
		this.transform.localRotation = Quaternion.identity;
	}

	public void AdvancedMerge() {
		var parent = this.mesh.transform.parent;

		this.mesh.transform.parent = null;
		this.mesh.transform.position = Vector3.zero;
		this.mesh.transform.rotation = Quaternion.identity;

		// All our children (and us)
		MeshFilter[] filters = GetComponentsInChildren<MeshFilter>(false);

		// All the meshes in our children (just a big list)
		List<Material> materials = new List<Material>();
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(false); // <-- you can optimize this
		foreach(MeshRenderer renderer in renderers) {
			if(renderer.transform == transform)
				continue;
			Material[] localMats = renderer.sharedMaterials;
			foreach(Material localMat in localMats)
				if(!materials.Contains(localMat))
					materials.Add(localMat);
		}

		// Each material will have a mesh for it.
		List<Mesh> submeshes = new List<Mesh>();
		foreach(Material material in materials) {
			// Make a combiner for each (sub)mesh that is mapped to the right material.
			List<CombineInstance> combiners = new List<CombineInstance>();
			foreach(MeshFilter filter in filters) {
				if(filter.transform == transform) continue;
				// The filter doesn't know what materials are involved, get the renderer.
				MeshRenderer renderer = filter.GetComponent<MeshRenderer>();  // <-- (Easy optimization is possible here, give it a try!)
				if(renderer == null) {
					Debug.LogError(filter.name + " has no MeshRenderer");
					continue;
				}

				// Let's see if their materials are the one we want right now.
				Material[] localMaterials = renderer.sharedMaterials;
				for(int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++) {
					if(localMaterials[materialIndex] != material)
						continue;
					// This submesh is the material we're looking for right now.
					CombineInstance ci = new CombineInstance();
					ci.mesh = filter.sharedMesh;
					ci.subMeshIndex = materialIndex;
					//ci.transform = Matrix4x4.identity;
					ci.transform = filter.transform.localToWorldMatrix;
					filter.gameObject.SetActive(false);
					combiners.Add(ci);
				}
			}
			// Flatten into a single mesh.
			Mesh mesh = new Mesh();
			mesh.CombineMeshes(combiners.ToArray(), true);
			submeshes.Add(mesh);
		}

		// The final mesh: combine all the material-specific meshes as independent submeshes.
		List<CombineInstance> finalCombiners = new List<CombineInstance>();
		foreach(Mesh mesh in submeshes) {
			CombineInstance ci = new CombineInstance();
			ci.mesh = mesh;
			ci.subMeshIndex = 0;
			ci.transform = Matrix4x4.identity;
			finalCombiners.Add(ci);
		}
		Mesh finalMesh = new Mesh();
		finalMesh.CombineMeshes(finalCombiners.ToArray(), false);
		this.mesh.GetComponent<MeshFilter>().sharedMesh = finalMesh;
		this.mesh.GetComponent<MeshRenderer>().sharedMaterials = materials.ToArray();
		Debug.Log("Final mesh has " + submeshes.Count + " materials.");

		this.transform.parent = parent;

		this.transform.localPosition = Vector3.zero;
		this.transform.localRotation = Quaternion.identity;
	}
}
