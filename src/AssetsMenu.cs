using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsMenu : MonoBehaviour
{

    public string library="cube";
    public GameObject template;

    AssetLibrary Library;

    Queue<string> assetQueue=new Queue<string>();

    GameObject lastAsset=null;

    // Start is called before the first frame update
    void Update()
    {

        if(Library==null){
             Library=AssetLibrary.Library;
             if(Library!=null){

                Library=AssetLibrary.Library;
                Library.RequireBundle(library);
                template.SetActive(false);



                Library.ListAssets(library, delegate(string[] list){

                    
                    foreach(string assetName in list){
                        assetQueue.Enqueue(assetName);
                    }
                });
            }
        }




        while(assetQueue.Count>0){

            string assetName=assetQueue.Dequeue();

            Debug.Log("Create Menu Item: "+assetName);

            GameObject assetObject=Instantiate(template, gameObject.transform);
            AssetCreateButton create=assetObject.GetComponent<AssetCreateButton>();
            if(create==null){
                create=assetObject.AddComponent<AssetCreateButton>();
            }

            create.library=library;
            create.assetName=assetName;

            if(lastAsset!=null){
                SimpleStack.StackVertical(lastAsset, assetObject);
            }

            lastAsset=assetObject;
            assetObject.SetActive(true);
        }


    }

}
