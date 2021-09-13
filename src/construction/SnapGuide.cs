using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;


public class SnapGuide{



	[System.Serializable]
	public struct SnapObject {

		public Vector3 center;

	    public float rotation;   

	    public float distance;   

	    public GameObject itemA;
	    public int itemAGuideIndex;

	    public GameObject itemB;
	    public int itemBGuideIndex;

	    //public bool top;


	   	public static SnapObject Empty(){
	   		return new SnapObject(Vector3.zero, null, -1, null, -1, Mathf.Infinity, 0);
	   	}

	    public SnapObject(Vector3 center, GameObject a, int indexA, GameObject b, int indexB, float distance, float rotation/*, bool top*/) {
	    	this.center=center;
	      	this.itemA=a;
	        this.itemAGuideIndex=indexA;

	        this.itemB=b;
	        this.itemBGuideIndex=indexB;
	        this.distance=distance;
	        this.rotation=rotation;
	       // this.top=top;
	    }
	}


	List<GameObject> planeGuidesObjects;

	public SnapGuide ForPlaneGuideObjects(List<GameObject> guides){
		planeGuidesObjects=new List<GameObject>();

		foreach(GameObject g in guides){
			if(ValidGuide(g)){
            	planeGuidesObjects.Add(g);
        	}
		}
		
		return this;
	}


	bool ValidGuide(GameObject g ){

		if(g==null){
    		Debug.Log("Item was deleted?");
    		return false;
    	}

        GameAlign plane=g.GetComponent<GameAlign>();

        if(plane==null||plane.planeGuides.Count==0){
        	//must have GameAlign with guides
            return false;
        }

        
		return true;

	}

	float range=10f;
	public SnapGuide InRange(float r){
		range=r;
		return this;
	}

	// Vector3 offset=Vector3.zero;

	// public SnapGuide WithOffset(Vector3 o){
	// 	offset=o;
	// 	return this;
	// }


	public SnapObject GetItemsClosestPlaneGuide(GameAlign plane, Vector3 point){

    	SnapObject snap=SnapObject.Empty();

    	ForGuideIndexes(plane.planeGuides, delegate(int indexA){
    		GuidePairAt(plane.planeGuides, indexA, delegate(Vector3 a0, Vector3 a1){

    			//float d=HandleUtility.DistancePointLine(point, a0.transform.position, a1.transform.position); //Vector3.Distance(c, point);
    			//
    			

        		float angle = Vector3.Angle((point-a0), (a1-a0));
        		float d=Vector3.Distance(a0, point)*Mathf.Sin(angle*Mathf.Deg2Rad);


    			if(d<snap.distance){
    				snap=new SnapObject(CenterOf(a0, a1), plane.gameObject, indexA, null, -1, d, 0);
    			}

    		});
    	});

    	return snap;

    }



    public SnapObject GetClosestPlaneGuideInGroup(GameObject item){

    	List<SnapObject> guides= GetPlaneGuidesInGroupForItem(item);

    	if(guides.Count>0){
    		return guides[0];
    	}

    	return SnapObject.Empty();

    }


	public List<SnapObject> GetPlaneGuidesInGroupForItem(GameObject item){

        GameAlign ga=item.GetComponent<GameAlign>();

        if(ga.anchorGuides.Count==0){
           throw new Exception("Item must have GameAlign with anchor guides");
        }


       List<SnapObject> guides=new  List<SnapObject>();

        //List<GameObject> planeItems=new List<GameObject>(){gameObject};
        //planeItems.AddRange(autoAlignItems);


        //Debug.Log("Check guides in: "+planeGuidesObjects.Count+" plane targets");


        foreach(GameObject g in planeGuidesObjects){

            if(g==item){
            	//cant align to self;
                continue;
            }
           
            GameAlign plane=g.GetComponent<GameAlign>();


            if((!plane.guidesTypes.Contains("*"))&&plane.guidesTypes.Contains(ga.anchorType)){
            	//does not align item;
                continue;
            }


            
            ForGuideIndexes(plane.planeGuides, delegate(int indexA){
                ForGuideIndexes(ga.anchorGuides, delegate(int indexB){

                	CheckAnchor(plane, indexA, ga, indexB, delegate(SnapObject snap){
                		guides.Add(snap);
                	});

                });
            });
            
        }


        // if(snap.distance<best.distance){
        //         			best=snap;
        //         		}


        guides.Sort(delegate(SnapObject a, SnapObject b){
        	return a.distance.CompareTo(b.distance);
        });



        
        return guides;

    }


    

    void CheckAnchor(GameAlign plane, int indexA, GameAlign ga, int indexB, ForSnap handler){


    	List<float> rotations=new List<float>{0f};
    	if(ga.rotateGuides.Length>0){
    		rotations.AddRange(ga.rotateGuides);
    	}

    	GuidePairRotationsAt(ga.anchorGuides, indexB, rotations, ga.transform.position, delegate(Vector3 b0, Vector3 b1, Vector3 center, float rot){
    		GuidePairAt(plane.planeGuides, indexA, delegate(Vector3 a0, Vector3 a1){
    		
    			Vector3 ca=CenterOf(a0, a1);
	         	Vector3 cb=CenterOf(b0, b1);


	    		/*
	    		 * measure distance between centers of guides
	    		 */
	            float d=Vector3.Distance(ca, cb);
	            if(d>=range){
	            	//Debug.Log("Out of range: "+indexA+" "+rot);
	                return;
	            }

	           	/*
	           	 * will rotated b0, b1 values affect c?
	           	 */

	         	Vector3 c=/*ga.transform.position*/center-(cb-ca);
	     
	         	foreach(GameObject g in planeGuidesObjects){
	         		if(g==ga.gameObject){
	         			continue;
	         		}
	         		if(Mathf.Abs(Vector3.Distance(c, g.transform.position))<0.1f){
	         			Debug.Log("Overlap: "+indexA+" "+rot);

	         			if(!CanOverlap(g, ga.gameObject)){        			
		         			return;
		         		}
		         	}
	         	}



	         	/*
	         	 * use dot product rule to make sure they are rotationally aligned
	         	 */
	         	
	         	float dot=1f-Mathf.Abs(Vector3.Dot((a1-a0).normalized, (b1-b0).normalized));
	         	if(dot>0.01f){
	         		//Debug.Log("Perpendicular: "+indexA+" "+rot+" ("+dot+")");
	         		return;
	         	}


	         	SnapObject snap=new SnapObject(c, plane.gameObject, indexA, ga.gameObject, indexB, d, rot);


	         	foreach(GameObject g in planeGuidesObjects){
	         		if(g==ga.gameObject){
	         			continue;
	         		}
	         		if(g.GetComponent<GameAlign>().PreventItemSnap(snap)){
	         			return;
		         	}
	         	}

	
	          
	            handler(snap);

	        

            });
    	});

    

    }


    bool CanOverlap(GameObject a, GameObject b){

    	 GameAlign ga=a.GetComponent<GameAlign>();
    	 GameAlign gb=b.GetComponent<GameAlign>();

    	// List<string> noOverlap=new List<string>{"floor", "stair"};
    	// if(noOverlap.Contains(ga.anchorType)&&noOverlap.Contains(gb.anchorType)){
    	// 	return false;
    	// }


    	return ga.anchorType!=gb.anchorType;
    }



    public void SnapItemToGuide(SnapObject snap){
    	SnapItemToGuide(snap.itemB, snap.center, snap.rotation);
    }

    void SnapItemToGuide(GameObject b, Vector3 center, float rotation){


    	//Debug.Log("Snap "+b.name+" "+center+" -> "+rotation);


    	if(Mathf.Abs(rotation)>0.01f){
    		b.transform.RotateAround(b.transform.position, Vector3.up, rotation);
    	}

    	b.transform.position=center;


        // GuideCenterAt(a.GetComponent<GameAlign>().planeGuides, indexA, delegate(Vector3 a0){
        //     GuideCenterAt(b.GetComponent<GameAlign>().anchorGuides, indexB, delegate(Vector3 b0){

        //         b.transform.position-=(b0-a0);

        //     });
        // });




    }


    void GuidePairRotationsAt(List<GameObject> guides, int index, List<float> rotations, Vector3 center, RotationPair handler){


    	GuidePairAt(guides, index, delegate(Vector3 b0, Vector3 b1){


	    	foreach(float rot in rotations){

	    		Vector3 c=CenterOf(b0, b1);

	    		//Debug.Log("Check Rotation: "+rot);

	    		//Debug.Log("Rotation:"+rot);
	    		if(Mathf.Abs(rot)>0.01f){
	    			handler(
	    				RotatePointAround(b0, c, Vector3.up, rot), 
	    				RotatePointAround(b1, c, Vector3.up, rot), 
	    				RotatePointAround(center, c, Vector3.up, rot), 
	    				rot
	    			);
	    			continue;
	    		}

	    		handler(b0, b1, center, rot);

	    	}
    	});

    }



    // void GuideAt(List<GameObject> guides, int index, Guide handler){
    //     handler(guides[index%guides.Count]);
    // }

    void GuidePairAt(List<GameObject> guides, int index, Pair handler){
        handler(guides[index].transform.position, (guides[(index+1)%guides.Count]).transform.position);
    }


    Vector3 CenterOf(GameObject a0, GameObject a1){
    	return CenterOf(a0.transform.position, a1.transform.position);
    }

    Vector3 CenterOf(Vector3 a0, Vector3 a1){
    	return a0+(a1-a0)/2.0f;
    }

    void GuideCenterAt(List<GameObject> guides, int index, Center handler){

    	GuidePairAt(guides, index, delegate(Vector3 a0, Vector3 a1){
    		handler(CenterOf(a0, a1));
    	});
    }

    void ForGuideIndexes(List<GameObject> guides, ForIndex handler){

    	if(guides.Count==2){
    		handler(0);
    		return;
    	}

    	/**
    	 * defineds a loop
    	 */


        for(int i=0;i<guides.Count;i++){
            handler(i);
        }


    }


    // void ForGuidePairs(List<GameObject> guides, ForPair handler){

    //     for(int i=0;i<guides.Count;i++){
    //         handler(guides[i], guides[(i+1)%guides.Count], i);
    //     }
    // }

    delegate void Center(Vector3 a0);
    //delegate void Guide(GameObject a0);
    delegate void Pair(Vector3 a0, Vector3 a1);
    //delegate void ForPair(GameObject a0, GameObject a1, int index);
    delegate void ForIndex(int index);
    delegate void ForSnap(SnapObject snap);


    delegate void RotationPair(Vector3 a0, Vector3 a1, Vector3 c, float rot);

    private Vector3 RotatePointAround(Vector3 pos, Vector3 center, Vector3 axis, float angle) {
	    Quaternion rot = Quaternion.AngleAxis(angle, axis); // get the desired rotation
	    Vector3 dir = pos - center; // find current direction relative to center
	    dir = rot * dir; // rotate the direction
	    return center + dir; // define new position
	    
	}
    


}