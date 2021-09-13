using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectionToolsMenu : MonoBehaviour
{
    

    public Button createTileBtn;
    public Button selectLookingAtBtn;
    public Button toggleRotatingBtn;
    public Button togglePositionBtn;
    public Button toggleRigidBodyBtn;

    public Button eventDeleteBtn;

    public Button togglePlayBtn;
    public Button toggleBuildBtn;

    Selection selection;
    GridBuilder gridBuilder;
    LookingAt lookAt;



    List<DragMove> dragging=null;
    List<DragRotate> rotating=null;
    List<Rigidbody> rigids=null;


    List<Button> btns=new List<Button>();


    public bool preventClickThrough=false;

    public static SelectionToolsMenu ToolsMenu;


    void Start()
    {

        ToolsMenu=this;
        
        AddToggle(InitMoveButton());
        AddToggle(InitGravityButton());
        AddToggle(InitRotateButton());


        InitApplyButton();
        InitDeleteButton();


        if(Input.mousePresent){
            MouseClick mouse=gameObject.AddComponent<MouseClick>();
            mouse.onClick=delegate(){
                HandleTouch(Input.mousePosition);
            };
        }else{
            TouchTap touch= gameObject.AddComponent<TouchTap>();
            touch.onTap=delegate(Touch touch){
                HandleTouch(touch.position);
            };
        }

    }

    void HandleTouch(Vector2 pos){
        if(preventClickThrough||EventSystem.current.currentSelectedGameObject!=null){
            return;
        }
        ActivateLookingAt();
        DecalClick(pos);
        GetComponent<TerrainFeature>().DrawFeature(pos);
        // if(gameObject.GetComponent<TerrainExport>()!=null){
        //     //gameObject.GetComponent<TerrainExport>().ExportTerrain(touch.position);
        // }
    }





    void AddButton(Button b){

        b.onClick.AddListener(delegate(){
            preventClickThrough=true;
            CancelInvoke("ClearPreventClickThrough");
            Invoke("ClearPreventClickThrough", 0.2f);
        });


        btns.Add(b);
    }

    void ClearPreventClickThrough(){
         preventClickThrough=false;
    }

    void AddToggle(Button b){

        if(b==null){
            return;
        }

        b.onClick.AddListener(delegate(){

            Debug.Log("Toggle Btn");

            b.GetComponent<UIActionBtn
>().Toggle();
            foreach(Button btn in btns){
                if(btn!=b){
                    btn.GetComponent<UIActionBtn
>().Disable();
                }
            }
        });
        AddButton(b);
        

    }


    Button InitRotateButton(){


        if(toggleRotatingBtn==null){
            return null;
        }

        
     

        toggleRotatingBtn.GetComponent<UIActionBtn
>().enableFunction=delegate(){

        
            if(rotating!=null){
                return true;
            }

            rotating=new List<DragRotate>();

            if(selection!=null){
                List<GameObject> list=selection.Get();
                foreach(GameObject item in list){
                    if(item.GetComponent<GameEntity>()!=null){

                        if(item.GetComponent<GameAlign>()!=null){
                            rotating.Add(item.GetComponent<GameAlign>().EnableRotating());
                            continue;
                        }

                        DragRotate rot=item.AddComponent<DragRotateSnap>();
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
        toggleRotatingBtn.GetComponent<UIActionBtn
>().disableFunction=delegate(){
            if(rotating!=null){
                foreach(DragRotate rot in rotating){

                    if(rot==null){
                        continue;
                    }

                    if(rot.GetComponent<GameAlign>()!=null){
                        rot.GetComponent<GameAlign>().DisableRotating();
                        continue;
                    }

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

      

        togglePositionBtn.GetComponent<UIActionBtn
>().enableFunction=delegate(){

            if(dragging!=null){
                return true;
            }

            dragging=new List<DragMove>();

            if(selection!=null){
                List<GameObject> list=selection.Get();
                foreach(GameObject item in list){
                    if(item.GetComponent<GameEntity>()!=null){

                        if(item.GetComponent<GameAlign>()!=null){
                            dragging.Add(item.GetComponent<GameAlign>().EnableDragging());
                            continue;
                        }

                        DragMove drag=item.AddComponent<DragMoveSnap>();
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
        togglePositionBtn.GetComponent<UIActionBtn
>().disableFunction=delegate(){
            if(dragging!=null){
                foreach(DragMove drag in dragging){

                    if(drag==null){
                        continue;
                    }

                    if(drag.GetComponent<GameAlign>()!=null){
                        drag.GetComponent<GameAlign>().DisableDragging();
                        continue;
                    }

                    Destroy(drag);
                }
                dragging=null;
            }
            return true;
        };
        
    
        return togglePositionBtn;



    }


    Button InitDeleteButton(){
        if(eventDeleteBtn==null){
            return null;
        }


         eventDeleteBtn.GetComponent<UIActionBtn
>().enableFunction=delegate(){

            Debug.Log("Press Delete");

            if(selection!=null){
                List<GameObject> list=selection.Get();
                foreach(GameObject item in list){
                    if(item.GetComponent<GameEntity>()!=null){
                        Debug.Log("Delete GameEntity");
                        item.GetComponent<GameEntity>().DestroySelf();
                    }
                }

            }

            return false; 

        };

        eventDeleteBtn.onClick.AddListener(delegate(){
            eventDeleteBtn.GetComponent<UIActionBtn
>().Toggle();
        });

        return eventDeleteBtn;


    }

    Button InitGravityButton(){
        if(toggleRigidBodyBtn==null){
            return null;
        }



        toggleRigidBodyBtn.GetComponent<UIActionBtn
>().enableFunction=delegate(){
            if(rigids!=null){
                return true;
            }

            rigids=new List<Rigidbody>();

            if(selection!=null){
                List<GameObject> list=selection.Get();
                foreach(GameObject item in list){
                    if(item.GetComponent<GameEntity>()!=null&&item.GetComponent<Rigidbody>()==null){
                        Rigidbody rb=item.AddComponent<Rigidbody>();
                        rb.isKinematic=true;
                        rb.useGravity=true;
                        rb.isKinematic=false;
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

        toggleRigidBodyBtn.GetComponent<UIActionBtn
>().disableFunction=delegate(){

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


    void OnSelectionChanged(List<GameObject> selection){

        foreach(Button b in btns){

            UIActionBtn
 btn=b.GetComponent<UIActionBtn
>();

            if(btn.active){
                btn.Disable();
                btn.Enable(); //Reapply to to active
                
            }

           
        }


    }



    // Update is called once per frame
    void Update()
    {
        if(selection==null){
            selection=gameObject.GetComponent<Selection>();
            if(selection!=null){
                selection.OnSelectionChanged(delegate(List<GameObject> selection){
                    OnSelectionChanged(selection);
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

                if(selection.IsAlreadySelected(list)){

                    if(lookAt.lookingAt.GetComponent<GameAlign>()!=null){
                        SnapGuide.SnapObject snap=lookAt.lookingAt.GetComponent<GameAlign>().CheckEdgeClick();

                        if(snap.itemA!=null){

                            gameObject.GetComponent<TerrainFeature>().DrawObject(snap.center, delegate(GameObject asset){

                            });
                        }
                        
                    }

                    return;
                }

                selection.SetSelected(list);

            }

        }

    }



    public GameObject decalPrefab;
    void DecalClick(Vector2 pos){
        (new Decal()).Display(pos, decalPrefab);
    }



}
