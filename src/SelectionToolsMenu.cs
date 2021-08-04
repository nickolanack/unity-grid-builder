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

    public Button togglePlayBtn;
    public Button toggleBuildBtn;

    Selection selection;
    GridBuilder gridBuilder;
    LookingAt lookAt;



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
                DecalClick(Input.mousePosition);
                TerrainGridBuilder.main.DrawFeature(Input.mousePosition, gameObject.GetComponent<TerrainFeature>());
                //TerrainGridBuilder.main.TerrainMakeTree(Input.mousePosition);
                if(gameObject.GetComponent<TerrainExport>()!=null){
                    //gameObject.GetComponent<TerrainExport>().ExportTerrain(Input.mousePosition);
                }
            };
        }else{
            TouchTap touch= gameObject.AddComponent<TouchTap>();
            touch.onTap=delegate(Touch touch){
                ActivateLookingAt();
                DecalClick(touch.position);
                TerrainGridBuilder.main.DrawFeature(touch.position, gameObject.GetComponent<TerrainFeature>());
                //TerrainGridBuilder.main.TerrainMakeTree(touch.position);
                if(gameObject.GetComponent<TerrainExport>()!=null){
                    //gameObject.GetComponent<TerrainExport>().ExportTerrain(touch.position);
                }
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



    public GameObject decalPrefab;

    void DecalClick(Vector2 pos){

        Debug.Log("Decal");

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        
        if (Physics.Raycast(ray, out hit)) {
            
            Quaternion q=Quaternion.LookRotation(hit.normal);
            Debug.Log("Instantiate");
            GameObject decal=Instantiate(decalPrefab, hit.point+(q*Vector3.forward*0.002f), q);
            Quaternion.LookRotation(hit.normal);



            Animation anim = decal.AddComponent<Animation>();
            AnimationCurve curve;

            // create a new AnimationClip
            AnimationClip clip = new AnimationClip();
            clip.legacy = true;

            // create a curve to move the GameObject and assign to the clip
            Keyframe[] keys;
            keys = new Keyframe[3];
            keys[0] = new Keyframe(0.0f, 0.01f);
            keys[1] = new Keyframe(0.3f, 0.04f);
            keys[2] = new Keyframe(0.4f, 0.03f);

            foreach(string prop in new string[]{
                "localScale.x","localScale.y","localScale.z"
            }){

                curve = new AnimationCurve(keys);
                clip.SetCurve("", typeof(Transform), prop, curve);

            }


            keys = new Keyframe[2];
            keys[0] = new Keyframe(0.0f, 1f);
            keys[1] = new Keyframe(0.4f, 1f);

            foreach(string prop in new string[]{
              //  "m_Color.r", "m_Color.g", "m_Color.b"
            }){

                curve = new AnimationCurve(keys);
                clip.SetCurve("", typeof(SpriteRenderer), prop, curve);

            }

            keys = new Keyframe[2];
            keys[0] = new Keyframe(0.0f, 1f);
            keys[1] = new Keyframe(0.4f, 0f);

            
            curve = new AnimationCurve(keys);
            clip.SetCurve("", typeof(SpriteRenderer),  "m_Color.a", curve);

            
          
            // now animate the GameObject
            anim.AddClip(clip, clip.name);
            anim.Play(clip.name);
            Destroy(decal, 0.5f);
        }
    }



}
