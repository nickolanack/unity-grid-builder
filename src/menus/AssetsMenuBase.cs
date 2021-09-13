using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class AssetsMenuBase : MonoBehaviour
{

    
    public GameObject template;

    protected AssetLibrary Library;

    protected Queue<string> assetQueue=new Queue<string>();

    GameObject lastAsset=null;
    public GameObject selected=null;

    // Start is called before the first frame update
    void Update()
    {

        if(Library==null){
             Library=AssetLibrary.Library;
             if(Library!=null){

             	template.SetActive(false);
             	Library=AssetLibrary.Library;
             	LoadAssets();
                
            }
        }




        while(assetQueue.Count>0){

            string assetName=assetQueue.Dequeue();

            Debug.Log("Create Menu Item: "+assetName);

            GameObject assetObject=Instantiate(template, gameObject.transform);
            assetObject.GetComponent<Button>().onClick.AddListener(delegate(){
                SelectAsset(assetObject);
            });

            InitAsset(assetObject, assetName);
            if( assetObject.GetComponent<AssetInfo>()!=null){
        	    assetObject.GetComponent<AssetInfo>().assetName=assetName;
        	}


            if(!selected){
            	SelectAsset(assetObject);
	        }
	           
            if(lastAsset!=null){
                SimpleStack.StackVertical(lastAsset, assetObject);
            }

            lastAsset=assetObject;
            assetObject.SetActive(true);
        }


    }

    protected virtual void SelectAsset(GameObject assetObject){
        
        if(assetObject==null||assetObject==selected){
            return;
        }

        if(selected!=null){
            selected.GetComponent<SelectableAssetMenuBtn>().Deselect();
        }



    	assetObject.GetComponent<SelectableAssetMenuBtn>().Select();
	    selected=assetObject;
    }

    protected abstract void LoadAssets();
    protected abstract void InitAsset(GameObject assetObject, string assetName);

}
