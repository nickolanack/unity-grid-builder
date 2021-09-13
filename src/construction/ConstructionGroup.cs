using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;


public class ConstructionGroup : MonoBehaviour{


	/**
	 * A ConstructionGroup is attached to a root object. and contains a collection of GameEntity items.
	 * 
	 * GameEntity objects do not need to be parented to this item but can be.
	 * 
	 * 
	 */


	public string createPlaneName="floor";

	public static Dictionary<string, ConstructionGroup> ConstructionGroups=new Dictionary<string, ConstructionGroup>();




	public List<GameObject> items=new List<GameObject>();




	public List<GameObject> GetAlignmentItems(){
		List<GameObject> alignmentItems=new List<GameObject>(){gameObject};
        alignmentItems.AddRange(items);
        return alignmentItems;
	}


	void Start(){
		ConstructionGroup.ConstructionGroups.Add(createPlaneName, this);
	}




	public void SetBelongsToGroup(GameObject item){
        AddItem(item);
    }

    void AddItem(GameObject item){

		if(item.GetComponent<GameEntity>()==null){
			throw new Exception("Must have a GameEntity MonoBehaviour");
		}

		if(!items.Contains(item)){
            items.Add(item);
        }
	}



    public List<GameObject> GetOverlappingItems(GameObject item){
    	
    	List<GameObject> list =new List<GameObject>();

    	foreach(GameObject g in GetAlignmentItems()){
     		if(g==null||g==item){
     			continue;
     		}
     		if(Mathf.Abs(Vector3.Distance(item.transform.position, g.transform.position))<0.1f){
     			list.Add(g);
         	}
     	}

     	return list;
    }




	public static void CreateGroup(GameObject root){
 		root.AddComponent<ConstructionGroup>();
	}


	public static bool ShouldMakeGroup(GameObject item){

		string createPlaneName="floor";

		return !ConstructionGroup.ConstructionGroups.ContainsKey(createPlaneName);
	}
	


	public static ConstructionGroup GetConstructionGroup(GameObject item){


		if(item.GetComponent<ConstructionGroup>()!=null){
			return null;
		}

		string createPlaneName="floor";

		string[] snapPlaneTargets=new string[]{createPlaneName};

        foreach(string planeName in snapPlaneTargets){
            
            if(ConstructionGroup.ConstructionGroups.ContainsKey(planeName)){
                return ConstructionGroup.ConstructionGroups[planeName];//.SetBelongsToGroup(this.gameObject);
            }
        }


        return null;

    }



}
