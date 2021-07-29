using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionToolsMenu : MonoBehaviour
{
    

    public Button createTileBtn;
    public Button selectLookingAtBtn;
    public Button toggleRotatingBtn;
    public Button togglePositionBtn;
    public Button toggleRigidBodyBtn;

    Selection selection;
    GridBuilder gridBuilder;
    LookingAt lookAt;


    float clickStart=-1;

    List<DragMove> dragging=null;
    List<DragRotate> rotating=null;
    List<Rigidbody> rigids=null;


    List<Button> btns=new List<Button>();


    void Start()
    {


        
        AddToggle(InitMoveButton());
        AddToggle(InitGravityButton());
        AddToggle(InitRotateButton());


        InitApplyButton();


        if(Input.mousePresent){
            MouseClick mouse=gameObject.AddComponent<MouseClick>();
            mouse.onClick=delegate(){
                ActivateLookingAt();
            };
        }else{
            TouchTap touch= gameObject.AddComponent<TouchTap>();
            touch.onTap=delegate(Touch touch){
                ActivateLookingAt();
            };
        }

    }


    void AddToggle(Button b){

        if(b==null){
            return;
        }
        b.onClick.AddListener(delegate(){
            b.GetComponent<SelectionToolBtn>().Toggle();
            foreach(Button btn in btns){
                if(btn!=b){
                    btn.GetComponent<SelectionToolBtn>().Disable();
                }
            }
        });
        btns.Add(b);

    }


    Button InitRotateButton(){


        if(toggleRotatingBtn==null){
            return null;
        }

        
     

        toggleRotatingBtn.GetComponent<SelectionToolBtn>().enableFunction=delegate(){

        
            if(rotating!=null){
                return true;
            }

            rotating=new List<DragRotate>();

            if(selection!=null){
                List<GameObject> list=selection.Get();
                foreach(GameObject item in list){
                    if(item.GetComponent<GameEntity>()!=null){
                        DragRotate rot=item.AddComponent<DragRotate>();
                        rotating.Add(rot);
                    }
                }

            }

            if(rotating.Count==0){
                rotating=null;
                return false;
            }
            return true;

        };
        toggleRotatingBtn.GetComponent<SelectionToolBtn>().disableFunction=delegate(){
            if(rotating!=null){
                foreach(DragRotate rot in rotating){
                    Destroy(rot);
                }
                rotating=null;
                
            }
            return true;
        };
            
        

        return toggleRotatingBtn;

    }


    Button InitMoveButton(){


        if(togglePositionBtn==null){
            return null;
        }

      

        togglePositionBtn.GetComponent<SelectionToolBtn>().enableFunction=delegate(){

            if(dragging!=null){
                return true;
            }

            dragging=new List<DragMove>();

            if(selection!=null){
                List<GameObject> list=selection.Get();
                foreach(GameObject item in list){
                    if(item.GetComponent<GameEntity>()!=null){
                        DragMove drag=item.AddComponent<DragMove>();
                        dragging.Add(drag);
                    }
                }

            }

            if(dragging.Count==0){
                dragging=null;
                return false;
            }
            return true;



        };
        togglePositionBtn.GetComponent<SelectionToolBtn>().disableFunction=delegate(){
            if(dragging!=null){
                foreach(DragMove drag in dragging){
                    Destroy(drag);
                }
                dragging=null;
            }
            return true;
        };
        
    
        return togglePositionBtn;



    }

    Button InitGravityButton(){
        if(toggleRigidBodyBtn==null){
            return null;
        }



        toggleRigidBodyBtn.GetComponent<SelectionToolBtn>().enableFunction=delegate(){
            if(rigids!=null){
                return true;
            }

            rigids=new List<Rigidbody>();

            if(selection!=null){
                List<GameObject> list=selection.Get();
                foreach(GameObject item in list){
                    if(item.GetComponent<GameEntity>()!=null&&item.GetComponent<Rigidbody>()==null){
                        Rigidbody rb=item.AddComponent<Rigidbody>();
                        rb.useGravity=true;
                        rigids.Add(rb);
                    }
                }

            }

            if(rigids.Count==0){
                rigids=null;
                return false;
            }

            return true;
        };

        toggleRigidBodyBtn.GetComponent<SelectionToolBtn>().disableFunction=delegate(){

            if(rigids!=null){
                foreach(Rigidbody rb in rigids){
                    Destroy(rb);
                }
                rigids=null;
            }

            return true;
        };


        return toggleRigidBodyBtn;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(selection==null){
            selection=gameObject.GetComponent<Selection>();
            if(selection!=null){
                selection.OnSelectionChanged(delegate(List<GameObject>selection){

                });
            }
        }


        if(gridBuilder==null){
            gridBuilder=GridBuilder.Main;
            if(gridBuilder!=null){
                gridBuilder.OnActivateTile(delegate(GameObject tile, int x, int y){
                    selection.SetSelected(lookAt.lookingAt);
                });

                gridBuilder.OnDeactivateTile(delegate(GameObject tile, int x, int y){

                });
            }
        }


    }



    void InitApplyButton(){

        if(selectLookingAtBtn!=null){
            if(!Input.mousePresent){
                //in case we need it
                selectLookingAtBtn.gameObject.SetActive(true);
            }
            selectLookingAtBtn.onClick.AddListener(delegate(){
                ActivateLookingAt();
            });
        }

    }

  
    void ActivateLookingAt(){


        if(lookAt==null){
             lookAt=gameObject.GetComponent<LookingAt>();
        }
        if(lookAt==null){
             lookAt=LookingAt.Main;
        }

        if(lookAt!=null&&lookAt.lookingAt!=null){

            if(gridBuilder.ContainsTile(lookAt.lookingAt)){
                gridBuilder.SetActiveTile(lookAt.lookingAt);
                return;
            }

            GameEntity entity=lookAt.lookingAt.GetComponent<GameEntity>();
            if(entity!=null){

                gridBuilder.SetActiveTile(entity.x, entity.y);

                List<GameObject> list=new List<GameObject>();
                list.Add(gridBuilder.GetActiveTile());
                list.Add(lookAt.lookingAt);
                selection.SetSelected(list);

            }

        }

    }

}
