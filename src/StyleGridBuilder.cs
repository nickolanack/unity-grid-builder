using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


public class StyleGridBuilder : GridBuilder
{


    public Material activeMaterial;
    public Material tileMaterial;
    public Material fillerMaterial;

    public GameObject terrainTemplate;

    public LookingAt lookAt;
    public HighlightSelectableURP selectable;


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

        terrainSize=terrainTemplate.GetComponent<Terrain>().terrainData.size;


        /*

        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
 
         // set the pixel values
        texture.SetPixel(0, 0, new Color(1.0f, 1.0f, 1.0f, 0.5f));
        texture.SetPixel(1, 0, Color.clear);
        texture.SetPixel(0, 1, Color.white);
        texture.SetPixel(1, 1, Color.black);
     
         // Apply all SetPixel calls
        texture.Apply();
        activeMaterial.mainTexture=texture;

        */
        
        onActivate=delegate(GameObject tile, int x, int y){

            GameTile gt=tile.GetComponent<GameTile>();
            gt.terrainTemplate=terrainTemplate;
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

            Debug.Log("onAddEntity: "+entity.entityId+" > Event:"+(!handlingEvent));

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
                    new JProperty("rotation", new JArray(r.x, r.y, r.z))
                );

                Debug.Log(evt.data.ToString(Formatting.None).Replace("\"", "\\\""));

                sendEventQueue.Enqueue(evt);

            }

        };



        selectable=Camera.main.gameObject.AddComponent<HighlightSelectableURP>();
        


        base.Start();


       // taskEventQueueStream.Enqueue(new JToken[]{
       //      JToken.Parse("{\"task\":\"create-entity\",\"tile\":[0,0],\"library\":\"cube\",\"asset\":\"assets/cubewhite.prefab\",\"id\":\"cube-01\",\"position\":[1.25895035,2.71713233,2.71713233],\"rotation\":[0.149460062,331.186768,331.186768]}")
       //  });


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
            handlingEvent=true;
            Debug.Log(data[0]);
            HandleEventTask(data[0]["task"].ToObject<string>(), data[0]["tile"][0].ToObject<int>(), data[0]["tile"][1].ToObject<int>(), data[0]);
            handlingEvent=false;
        }




        if(lookAt==null){
            lookAt=selectable.lookAt;
            if(lookAt!=null){
                lookAt.targetFilter=delegate(GameObject obj){
                    /**
                     * Only look at tiles and entities
                     */
                    return obj.GetComponent<GameEntity>()!=null||obj.GetComponent<GameTile>()!=null||obj.GetComponent<GameFillerTile>()!=null;
                };

                lookAt.targetResolve=delegate(GameObject obj){
                    if(obj.GetComponent<Terrain>()!=null){

                        if(obj.transform.parent==null){
                            return null;
                        }

                        return obj.transform.parent.gameObject;
                    }
                    return obj;
                };

                }
        }

    }




    void HandleEventTask(string task, int x, int y, JToken data){

         Debug.Log(task);
        
        if(task.Equals("create-tile")){

            OnCreateTileEvent(x, y);

        }

        if(task.Equals("create-entity")){

            if(data["library"]==null){
                return;
            }

            string bundle=data["library"].ToObject<string>();
            string asset=data["asset"].ToObject<string>();

            AssetLibrary.Library.LoadAsset(bundle, asset, delegate(GameObject asset){

                Debug.Log("Loaded Asset: "+asset);
                GameObject newAsset=Instantiate(asset);
                GameEntity entity=newAsset.AddComponent<GameEntity>();
                
                Vector3 pos=Vector3.zero;
                Vector3 rot=Vector3.zero;

                pos.x=data["position"][0].ToObject<float>();
                pos.y=data["position"][1].ToObject<float>();
                pos.z=data["position"][2].ToObject<float>();

                rot.x=data["rotation"][0].ToObject<float>();
                rot.y=data["rotation"][1].ToObject<float>();
                rot.z=data["rotation"][2].ToObject<float>();

                newAsset.transform.position=pos;
                newAsset.transform.eulerAngles=rot;


                AddEntity(newAsset, x, y);


            });
           
        }


    }




    void OnCreateTileEvent(int x, int y){
        Debug.Log("Set active: "+x+", "+y);
        SetActiveTile(x, y);

    }

    


 }
