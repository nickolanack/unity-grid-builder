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

    Selection selection;
    GridBuilder gridBuilder;
    LookingAt lookAt;


    float clickStart=-1;

    List<DragMove> dragging=null;
    List<DragRotate> rotating=null;

    void Start()
    {

        if(toggleRotatingBtn!=null){

            toggleRotatingBtn.onClick.AddListener(delegate(){
                ToggleRotateEntity();
            });

        }

        if(togglePositionBtn!=null){

            togglePositionBtn.onClick.AddListener(delegate(){
                ToggleMoveEntity();
            });
            
        }



        InitApplyButton();
    }


    void ToggleMoveEntity(){

        if(rotating!=null){
            ToggleRotateEntity();
        }

        if(dragging!=null){
            foreach(DragMove drag in dragging){
                Destroy(drag);
            }
            dragging=null;
            return;
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
        }


    }


    void ToggleRotateEntity(){

        if(dragging!=null){
            ToggleMoveEntity();
        }

        if(rotating!=null){
            foreach(DragRotate rot in rotating){
                Destroy(rot);
            }
            rotating=null;
            return;
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
        }


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


        CheckMouseClick();
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

    void CheckMouseClick(){

        if(Input.GetMouseButtonDown(0)){
            clickStart=Time.time;
        }
        if(Input.GetMouseButtonUp(0)&&Time.time-clickStart<0.3f&&Input.mousePresent){
            // TODO: ensure single tap
            // Note this is also triggered by touch tap which caused issues with TouchMovePan behavior
            ActivateLookingAt();
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
