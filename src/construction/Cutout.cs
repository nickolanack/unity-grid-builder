using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutout : MonoBehaviour
{



	public GameObject cutoutItem;

	
	void Update(){


		if (Time.frameCount % 100 == 0)
     	{


     		GameObject ob=transform.parent.gameObject;

			ConstructionGroup group=ConstructionGroup.GetConstructionGroup(ob);
			if(group==null){
				return;
			}

			List<GameObject> items=group.GetOverlappingItems(ob);

			if(items.Count!=1){
				return;
			}

			GameObject item=items[0];

			if(cutoutItem==item){
				return;
			}


			Bounds b = GetComponent<MeshFilter>().mesh.bounds;

	        
        	MeshFilter mf=item.GetComponent<MeshFilter>();
        	if(mf!=null){
	        	Mesh m=mf.mesh;

	          	if(item.GetComponent<GameEntity>()!=null){
	          		
	          		Debug.Log("door: "+b);
	          		Debug.Log(item.name+": "+m.bounds);

	          		Debug.Log(b.Intersects(m.bounds)?"Intersects":"Does not intersect");


	          		if(b.Intersects(m.bounds)){
	          			ApplyCutout(item);
	          			cutoutItem=item;
	          		}

	          	}
           	}
	          
	        


    	}

	}


	void ApplyCutout(GameObject item){


		Mesh mesh=GetComponent<MeshFilter>().mesh;
		
		Vector3[] newVertices=ClampVertices(mesh.vertices, item);
		//mesh.Clear();
		mesh.vertices=newVertices;
		mesh.RecalculateBounds();


		for(int i=0;i<newVertices.Length;i++){
			newVertices[i]=item.transform.InverseTransformVector(transform.TransformVector(newVertices[i]));
		}


		int[] triangles=item.GetComponent<MeshFilter>().mesh.triangles;

		for(int i=0;i<triangles.Length;i+=3){



		}


	}


	Vector3[] ClampVertices(Vector3[] verts, GameObject item){


		Vector3[] obj=item.GetComponent<MeshFilter>().mesh.vertices;

		float minx=Mathf.Infinity, maxx=-Mathf.Infinity, minz=Mathf.Infinity, maxz=-Mathf.Infinity;

		foreach(Vector3 vert in obj){

			Vector3 v=transform.InverseTransformVector(item.transform.TransformVector(vert));

			minx=Mathf.Min(minx, v.x);
			minz=Mathf.Min(minz, v.z);

			maxx=Mathf.Max(maxx, v.x);
			maxz=Mathf.Max(maxz, v.z);

		}



		for(int i=0;i<verts.Length;i++){
			verts[i]=new Vector3(Mathf.Min(Mathf.Max(minx, verts[i].x), maxx), verts[i].y, Mathf.Min(Mathf.Max(minz, verts[i].z), maxz));
		}


		return verts;
	}



	bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b){
	    Vector3 cp1 = Vector3.Cross(b-a, p1-a);
	    Vector3 cp2 = Vector3.Cross(b-a, p2-a);
	    if (Vector3.Dot(cp1, cp2) >= 0){
	   		return true;
	   	}
	   	return false;
	}

	bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c){
	    if (SameSide(p,a, b,c)&& SameSide(p,b, a,c)
	        && SameSide(p,c, a,b)){
	    	return true;
		}
	    return false;
	}



	void ___ApplyCutout(GameObject item){


		Bounds b = GetComponent<MeshFilter>().mesh.bounds;


		MeshFilter mf=item.GetComponent<MeshFilter>();	
	   	Mesh mesh=mf.mesh;

	   	Vector3[] vertices=mesh.vertices;
	   	Vector2[] uvs=mesh.uv;


	   	List<int> removeIndexes=new List<int>();
	   	List<Vector3> newVertices=new List<Vector3>();

	   	List<int> removeTriangles=new List<int>();
	   	List<int> newTriangles=new List<int>();

	   	List<Vector2> newUvs=new List<Vector2>();

	   	for(int i=0;i<vertices.Length; i++){


	   		Debug.Log("vertex: "+i);

	   		Vector3 v=vertices[i];
	   		Vector3 tv=transform.TransformVector(v);
	   		if(b.Contains(tv)){
	   			removeIndexes.Add(i);
	   			Debug.Log("Remove Index: "+i+" "+tv);
	   		}else{

	   			Debug.Log("Keep Index: "+i+" "+tv);

				newVertices.Add(v);
				newUvs.Add(uvs[i]);
	   		}
	   	}


	   	int[] triangles=mesh.triangles;



	   	for(int i=0;i<triangles.Length;i+=3){

	   		if(removeIndexes.Contains(triangles[i])||removeIndexes.Contains(triangles[i+1])||removeIndexes.Contains(triangles[i]+2)){
	   			 removeTriangles.Add(triangles[i]);
	   			 removeTriangles.Add(triangles[i+1]);
	   			 removeTriangles.Add(triangles[i+2]);
	   		}else{
	   			newTriangles.Add(triangles[i]);
	   			newTriangles.Add(triangles[i+1]);
	   			newTriangles.Add(triangles[i+2]);
	   		}
	   	}


	   	mesh.Clear();

	    mesh.vertices = newVertices.ToArray();
        mesh.uv = newUvs.ToArray();
        mesh.triangles = newTriangles.ToArray();



	}


}