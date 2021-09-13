using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;


public class GameAlign : MonoBehaviour
{
    public bool snapToGround=true;
    public bool snapToPlane=true;
    public float planeOffsetY=0f;

    public bool createsDefaultPlane=false;
    public string createPlaneName="floor";


    public string[] snapPlaneTargets=new string[]{"floor"};


  

    public List<GameObject> anchorGuides;
    public float[] rotateGuides;

    public List<GameObject> planeGuides;

    public string anchorType="floor";
    public List<string> guidesTypes=new List<string>{"*"};


    public SnapGuide.SnapObject snapAnchor=SnapGuide.SnapObject.Empty();


    public MoveGhost moveGhost=null;
    public SnapDecals snapDecals=null;



    public delegate void SnapEvent(SnapGuide.SnapObject snap);
    public SnapEvent onSnapInRange=delegate(SnapGuide.SnapObject snap){};
    public SnapEvent onSnapOutOfRange=delegate(SnapGuide.SnapObject snap){};


    public delegate void AlignEvent();
    public AlignEvent onAlignEnd;
    public AlignEvent onAlignStart;

    public SnapEvent onAlignSnap;



    public bool PreventItemSnap(SnapGuide.SnapObject snap){

        if(snap.itemA==this.gameObject&&anchorType=="stair"){

            //do not allow floor above this
            //
            //
            Debug.Log("Place Over Stair");

            Vector3 c=snap.center;
            Vector3 p=transform.position;


            if(c.y-p.y>1){

                c.y=0;
                p.y=0;

                if(Vector3.Distance(c, p)<0.1f){
                     Debug.Log("Prevent");
                    return true;
                }
            }
        }

        return false;
    }


    public DragMove EnableDragging(){


        DragMove dragMove=gameObject.AddComponent<DragMoveSnap>();
        dragMove.useRayCastPosition=true;
        dragMove.validStartClick=delegate(Vector2 click){
            Ray ray = Camera.main.ScreenPointToRay(click);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)){
                return hit.transform==transform; //only drag start on item
            }
            return false;
        };

        dragMove.validStartThreshold=delegate(Vector2 mouse){
           return Time.time-dragMove.startTime>0.2f&&Vector2.Distance(dragMove.start, mouse)>5; //5 pixels
        };

        dragMove.afterDrag=delegate(Vector3 position){
            Align();
        };

        return dragMove;
    } 
    public void DisableDragging(){
        Destroy(gameObject.GetComponent<DragMoveSnap>());
    } 


    public DragRotate EnableRotating(){
        DragRotate dragRotate= gameObject.AddComponent<DragRotateSnap>();

        //dragRotate.afterRotate=delegate(Quarternion rot){};

        return dragRotate;
    } 
    public void DisableRotating(){
        Destroy(gameObject.GetComponent<DragRotateSnap>());
    } 



    public SnapGuide.SnapObject CheckEdgeClick(){

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){
            if(hit.transform==transform){

                SnapGuide.SnapObject snap = (new SnapGuide()).GetItemsClosestPlaneGuide(this, hit.point);
                if(snap.distance<0.5f){
                    return snap; 
                }

            }
        }

        return SnapGuide.SnapObject.Empty();
            
    }

    void Start(){


        if(gameObject.GetComponent<SnapDecals>()==null){
            snapDecals=gameObject.AddComponent<SnapDecals>();

            onSnapInRange=delegate(SnapGuide.SnapObject snap){
                snapDecals.AddDecal(snap);
            };

            onSnapOutOfRange=delegate(SnapGuide.SnapObject snap){
                snapDecals.RemoveDecal(snap);
            };


            onAlignEnd=delegate(){
                snapDecals.Clear();
            };

        }


        if(gameObject.GetComponent<MoveGhost>()==null){
            moveGhost=gameObject.AddComponent<MoveGhost>();
            moveGhost.instantiate=delegate(Vector3 position, Quaternion rotation){


                if(onAlignStart!=null){
                    onAlignStart();
                }

                GameObject ghost=Instantiate(gameObject, position, rotation);
                Destroy(ghost.GetComponent<GameAlign>());
                Destroy(ghost.GetComponent<GameEntity>());

                Renderer r=ghost.GetComponent<Renderer>();
                if(r!=null){
                    r.materials=new Material[]{GridBuilder.Main.dragMaterial};
                    r.shadowCastingMode=ShadowCastingMode.Off;
                }


                Renderer[] renderers=ghost.GetComponentsInChildren<Renderer>();
                foreach(Renderer childRenderer in renderers){
                    childRenderer.materials=new Material[]{GridBuilder.Main.dragMaterial};
                    childRenderer.shadowCastingMode=ShadowCastingMode.Off;
                }




                Collider c=GetComponent<Collider>();
                if(c!=null){
                    c.enabled=false;
                }  

                Collider[] colliders=ghost.GetComponentsInChildren<Collider>();
                foreach(Collider childCollider in colliders){
                    childCollider.enabled=false;
                }              

                return ghost;

            };

            moveGhost.onDestroy=delegate(){
                GetComponent<Collider>().enabled=true;

                if(onAlignEnd!=null){
                    onAlignEnd();
                }
            };
        
        }


        Debug.Log("Initial Align");
        Align();


    }

    void Align(){


        if(snapToPlane&&ConstructionGroup.GetConstructionGroup(gameObject)!=null){
            SnapToPlaneInGroup(ConstructionGroup.GetConstructionGroup(gameObject));
        }else if(snapToGround){
            AutoAlignToGround(); 
        }

        if(createsDefaultPlane&&ConstructionGroup.ShouldMakeGroup(gameObject)){
            ConstructionGroup.CreateGroup(gameObject);
        } 
    }


 

    public void SnapToPlaneInGroup(ConstructionGroup group){

   
        group.SetBelongsToGroup(gameObject);
        SnapItemToClosestAvailableGuideInPlane(group);
    }

   

    
 

    void AutoAlignToGround(){

        Vector3 pos = transform.position;
        pos.y = Terrain.activeTerrain.SampleHeight(transform.position)+Terrain.activeTerrain.transform.position.y+planeOffsetY;

        if(transform.GetComponent<DragMoveSnap>()!=null){

            transform.GetComponent<DragMoveSnap>().lockAt=pos;
            transform.GetComponent<DragMoveSnap>().lockY=true;
            
            return;
        }

        transform.position = pos;

    }








    public List<SnapGuide.SnapObject> lastSnaps;
    //public List<SnapGuide.SnapObject> snapsDebug;

    void SnapItemToClosestAvailableGuideInPlane(ConstructionGroup group){


        if(anchorGuides.Count==0){
            return;
        }


        float distance=3;

        List<GameObject> planeItems=group.GetAlignmentItems();

        List<SnapGuide.SnapObject> newSnaps=(new SnapGuide())
            .ForPlaneGuideObjects(planeItems)
            .InRange(distance)
            .GetPlaneGuidesInGroupForItem(gameObject);

        foreach(SnapGuide.SnapObject s in lastSnaps){
            if(!newSnaps.Contains(s)){
                onSnapOutOfRange(s);
            }
        }

        foreach(SnapGuide.SnapObject s in newSnaps){
            if(!lastSnaps.Contains(s)){
                onSnapInRange(s);
            }else{

            }
        }

        lastSnaps=newSnaps;

        if(newSnaps.Count>0){

            SnapGuide.SnapObject snap=newSnaps[0];

           //Debug.Log("Debug: Get Item Snaps");

            // .snapsDebug=(new SnapGuide())
            // .ForPlaneGuideObjects(new List<GameObject>{snap.itemA})
            // .GetPlaneGuidesInGroupForItem(snap.itemB);


            if(snap.distance<distance){
                snapAnchor=snap;
                (new SnapGuide()).SnapItemToGuide(snap);

                if(onAlignSnap!=null){
                    onAlignSnap(snap);
                }

            }
        
        }else{
            snapAnchor=SnapGuide.SnapObject.Empty();
        }

    }


}
