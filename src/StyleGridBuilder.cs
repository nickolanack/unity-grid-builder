using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;


public class StyleGridBuilder : GridBuilder
{


    public Material activeMaterial;
    public Material tileMaterial;
    public Material fillerMaterial;

    public LookingAt lookAt;
    public HighlightSelectable selectable;


    public Button applyButton;
    PushSocketIOClient client=null;

    bool handlingEvent=false;


    Queue<JToken[]> taskEventQueueStream=new Queue<JToken[]>();

     Queue<EntityEventData> sendEventQueue=new Queue<EntityEventData>();

    class EntityEventData{

      public string channel;
      public string eventName;
      public JObject data;
    }


    public bool connected=false;

    // Start is called before the first frame update
    protected override void Start()
    {
        
        onActivate=delegate(GameObject tile, int x, int y){
            tile.GetComponent<Renderer>().material = activeMaterial;
        };
        onDeactivate=delegate(GameObject tile, int x, int y){
            tile.GetComponent<Renderer>().material = tileMaterial;
        };

        onFill=delegate(GameObject fillerTile, int x, int y){
            fillerTile.GetComponent<Renderer>().material = fillerMaterial;
        };

        onCommit=delegate(GameObject tile, int x, int y){
            tile.GetComponent<Renderer>().material = tileMaterial;

            if(client!=null&&!handlingEvent){
                PushSocketIOClient.Client.Send("levelChat", "builder", new JObject(
                    new JProperty("task", "create-tile"),
                    new JProperty("tile", new JArray(x, y))
                ));
            }

        };


         onAddEntity=delegate(GameEntity entity, int x, int y){

            if(!handlingEvent){




                Vector3 p=entity.gameObject.transform.position;
                Vector3 r=entity.gameObject.transform.eulerAngles;


                EntityEventData evt=new EntityEventData();

                evt.channel="levelChat";
                evt.eventName="builder";
                evt.data=new JObject(
                    new JProperty("task", "create-entity"),
                    new JProperty("tile", new JArray(x, y)),
                    new JProperty("asset", entity.assetName),
                    new JProperty("id", entity.entityId),
                    new JProperty("position", new JArray(p.x, p.y, p.y)),
                    new JProperty("rotation", new JArray(r.x, r.y, r.y))
                );

                sendEventQueue.Enqueue(evt);

            }

        };



        selectable=Camera.main.gameObject.AddComponent<HighlightSelectable>();
        

        if(applyButton!=null){
            if(!Input.mousePresent){
                //in case we need it
                applyButton.gameObject.SetActive(true);
            }
            applyButton.onClick.AddListener(delegate(){
                ActivateLookingAt();
            });
        }


        base.Start();
    }

    void Update(){

        if(client==null){
            client=PushSocketIOClient.Client;
            if(client!=null){
                PushSocketIOClient.Client.Subscribe("levelChat", "builder", delegate(JToken[] data){
                    taskEventQueueStream.Enqueue(data);
                }, delegate(JToken[] ack){
                    if(ack[0].ToObject<bool>()){
                        Debug.Log("Ready");
                        connected=true;
                    }

                });
            }
        }

        while(connected&&client!=null&&sendEventQueue.Count>0){
            EntityEventData evt=sendEventQueue.Dequeue();
            PushSocketIOClient.Client.Send(evt.channel, evt.eventName, evt.data);
        }


        if(taskEventQueueStream.Count>0){

            JToken[] data=taskEventQueueStream.Dequeue();
            if(data[0]["task"].ToObject<string>().Equals("create-tile")){

                Debug.Log(data[0]["task"]);

                int x=data[0]["tile"][0].ToObject<int>();
                int y=data[0]["tile"][1].ToObject<int>();
                handlingEvent=true;
                Debug.Log("Set active: "+x+", "+y);
                SetActiveTile(x, y);
                handlingEvent=false;
            }
        }




        if(lookAt==null){
            lookAt=selectable.lookAt;
            if(lookAt!=null){
                lookAt.targetFilter=delegate(GameObject obj){
                    return true;
                };
            }
        }
        
        if(lookAt!=null&&Input.GetMouseButtonDown(0)){
            ActivateLookingAt();
        }
    }

    void ActivateLookingAt(){
        if(lookAt!=null){

            if(ContainsTile(lookAt.lookingAt)){
                SetActiveTile(lookAt.lookingAt);
                return;
            }

        }

    }


 }
