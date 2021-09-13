using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsMenu : AssetsMenuBase
{

    public string[] libraries=new string[]{"cube"};

    protected override void InitAsset(GameObject assetObject, string assetName){
        AssetCreateButton create=assetObject.GetComponent<AssetCreateButton>();
        if(create==null){
            create=assetObject.AddComponent<AssetCreateButton>();
        }


       string[] parts=assetName.Split('.');
       string library=parts[0];
       string asset=assetName.Substring(library.Length+1);

        create.library=library;
        create.assetName=asset;


    }

    protected override void LoadAssets(){

        
        foreach(string library in libraries){

            Library.RequireBundle(library);
            Library.ListAssets(library, delegate(string[] list){

                
                foreach(string assetName in list){
                    assetQueue.Enqueue(library+"."+assetName);
                }
            });

        }
    
    }

   

}
