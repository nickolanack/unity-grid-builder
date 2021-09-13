using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnapDecals : MonoBehaviour
{



	Dictionary<Vector3, GameObject> decals=new Dictionary<Vector3, GameObject>();
	public GameObject decalPrefab;


	public void AddDecal(SnapGuide.SnapObject snap){
		if(!decals.ContainsKey(snap.center)){
			decals.Add(snap.center, (new Decal()).Create(snap.center, Vector3.up, decalPrefab));
		}
	}
	   



	public void RemoveDecal(SnapGuide.SnapObject snap){

		if(decals.ContainsKey(snap.center)){
			Object.Destroy(decals[snap.center]);
			decals.Remove(snap.center);
		}
	}


	public void Clear(){

		foreach(KeyValuePair<Vector3, GameObject> entry in decals)
		{
		    Object.Destroy(entry.Value);
		}

		decals=new Dictionary<Vector3, GameObject>();
	
	}
	   


}