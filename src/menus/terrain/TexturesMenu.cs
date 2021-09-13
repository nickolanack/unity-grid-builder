using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturesMenu : AssetsMenuBase
{

   public int layerIndex=1; 
   public float scale=0.6f; 
   public string[] textureNames=new string[]{

   };

   protected override void InitAsset(GameObject assetObject, string assetName){
        TerrainStamp stamp=assetObject.AddComponent<TerrainTextureStamp>();
        stamp.textureName=assetName;

    }

    protected override void LoadAssets(){


        foreach(string textureName in textureNames){
            assetQueue.Enqueue(textureName);
        }
    }


     public TerrainTexture[] GetTerrainTextures(){

        TerrainTextureStamp[] stamps=GetTextureStamps();

        TerrainTexture[] textures=new TerrainTexture[stamps.Length];
        for(int i=0;i<stamps.Length;i++){
            textures[i]=stamps[i].GetTerrainTexture();
        }
        return textures;

    }

   
    public TerrainTextureStamp[] GetTextureStamps(){

        selected.GetComponent<TerrainTextureStamp>().layerIndex=layerIndex;
        selected.GetComponent<TerrainTextureStamp>().scale=scale;
        return new TerrainTextureStamp[]{selected.GetComponent<TerrainTextureStamp>()};
    }


}
