using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGhost : MonoBehaviour
{



	public delegate GameObject CreateInstance(Vector3 position, Quaternion rotation);
	public CreateInstance instantiate;

    public delegate void DestroyInstance();
    public DestroyInstance onDestroy;

    public GameObject ghost=null;
    public bool lockY=false;




	void Update(){



        if(transform.GetComponent<DragMove>()!=null){
           
            if(transform.GetComponent<DragMove>().onDrag==null){

                
                transform.GetComponent<DragMove>().onDragStart=delegate(Vector3 position){
                    if(ghost!=null){
                        Destroy(ghost);
                    }

                    if(instantiate!=null){
                    	ghost=instantiate(position, transform.rotation);
                    	Destroy(ghost.GetComponent<MoveGhost>());
                    	Destroy(ghost.GetComponent<DragMove>());
                    	Destroy(ghost.GetComponent<Collider>());

                    }

                };
                transform.GetComponent<DragMove>().onDrag=delegate(Vector3 position){
                    if(ghost!=null){

                        if(lockY){
                            position.y=transform.position.y;
                        }

                        position.y+=0.001f;
                        
                        ghost.transform.position=position;

                    }
                };

                

                transform.GetComponent<DragMove>().onDragEnd=delegate(Vector3 position){
                    if(ghost!=null){
                        Destroy(ghost);
                         if(onDestroy!=null){
                            onDestroy();
                        }
                    }

                };
            }

            return;
        }



        if(transform.GetComponent<DragRotate>()!=null){
            GameObject ghost=null;
            if(transform.GetComponent<DragRotate>().onRotate==null){

                
                transform.GetComponent<DragRotate>().onRotateStart=delegate(Quaternion rotation){
                    if(ghost!=null){
                        Destroy(ghost);
                    }

                    if(instantiate!=null){
                    	ghost=instantiate(transform.position, rotation);
                    	Destroy(ghost.GetComponent<MoveGhost>());
                    	Destroy(ghost.GetComponent<DragRotate>());
                    	Destroy(ghost.GetComponent<Collider>());
                    }
                };
                transform.GetComponent<DragRotate>().onRotate=delegate(Quaternion rotation){
                    if(ghost!=null){
                        ghost.transform.rotation=rotation;
                    }
                };

                transform.GetComponent<DragRotate>().onRotateEnd=delegate(Quaternion rotation){
                    if(ghost!=null){
                        Destroy(ghost);
                         if(onDestroy!=null){
                            onDestroy();
                        }
                    }

                };
            }

            return;
        }

        if(ghost!=null){
            Destroy(ghost);
            if(onDestroy!=null){
                onDestroy();
            }
        }

    }

}