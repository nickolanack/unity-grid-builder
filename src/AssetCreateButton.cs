using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetCreateButton : MonoBehaviour
{

    Button btn;

    public string library;
    public string assetName;


    public Text text;

    void Start()
    {
        btn=gameObject.GetComponent<Button>();
        btn.onClick.AddListener(delegate(){
            Debug.Log("Create Asset");

            AssetLibrary.Library.LoadAsset(library, assetName, delegate(GameObject asset){

                Debug.Log("Loaded Asset: "+asset);
                GameObject newAsset=Instantiate(asset);
                GameEntity entity=newAsset.AddComponent<GameEntity>();
                entity.entityId="cube-"+Random.Range(0, 99999);

                // Vector3 pos=Vector3.zero;
                // Vector3 rot=Vector3.zero;

                // pos.x=data["position"][0].ToObject<float>();
                // pos.y=data["position"][1].ToObject<float>();
                // pos.z=data["position"][2].ToObject<float>();

                // rot.x=data["rotation"][0].ToObject<float>();
                // rot.y=data["rotation"][1].ToObject<float>();
                // rot.z=data["rotation"][2].ToObject<float>();

                newAsset.transform.position=new Vector3(Random.Range(-1.0f, 1.0f), 6+Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                
                Rigidbody rb=newAsset.GetComponent<Rigidbody>();
                if(rb==null){
                    rb=newAsset.AddComponent<Rigidbody>();
                }
                rb.useGravity=true;
                rb.velocity=new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, -15.0f), Random.Range(-1.0f, 1.0f));
                

                GridBuilder.Main.AddEntity(newAsset, 0, 0);


            });
           




        });

        if(text==null){
            Text[] t=gameObject.GetComponentsInChildren<Text>();
            if(t.Length>0){
                text=t[0];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        string label=assetName;
        label=label.Replace(".prefab", "");
        label=label.Replace("assets/", "");
        text.text=label;
    }
}
