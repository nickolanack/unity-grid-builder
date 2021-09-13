using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AssetCreateButton : MonoBehaviour
{

    Button btn;

    public string library;
    public string assetName;

    public bool instantiateOnClick=false;
   
    public delegate void AssetCreated(GameObject asset);


    void Start()
    {
        btn=gameObject.GetComponent<Button>();

       
            btn.onClick.AddListener(delegate(){
                if(!instantiateOnClick){
                    return;
                }

                CreateAssetAt(Camera.main.transform.position + Camera.main.transform.forward*5 + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
            
            });
        

      
    }

    
    public void CreateAssetAt(Vector3 position){
        CreateAssetAt(position, null);
    }
    public void CreateAssetAt(Vector3 position, AssetCreated del){

        Debug.Log("Create Asset");

        AssetLibrary.Library.LoadAsset(library, assetName, delegate(GameObject asset){

            Debug.Log("Loaded Asset: "+asset);
            GameObject newAsset=Instantiate(asset);
            GameEntity entity=newAsset.AddComponent<GameEntity>();
            


            entity.assetName=assetName;
            entity.library=library;

            entity.entityId="cube-"+Random.Range(0, 99999);

            newAsset.transform.position=position;
   
            GridBuilder.Main.AddEntity(newAsset, 0, 0);

            if(del!=null){
                 del(newAsset);
            }
        });


    }

}
