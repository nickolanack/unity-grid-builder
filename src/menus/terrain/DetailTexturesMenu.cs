using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailTexturesMenu : AssetsMenuBase
{


    public int layerIndex=0;
    public string[] textureNames=new string[]{

    };

    protected override void InitAsset(GameObject assetObject, string assetName){
        TerrainStamp stamp=assetObject.AddComponent<TerrainDetailStamp>();
        stamp.textureName=assetName;
        stamp.scale=0.3f;
    }

    protected override void LoadAssets(){


        foreach(string textureName in textureNames){
            assetQueue.Enqueue(textureName);
        }
    }


    public TerrainTexture[] GetTerrainTextures(){

        TerrainDetailStamp[] stamps=GetTextureStamps();

        TerrainTexture[] textures=new TerrainTexture[stamps.Length];
        for(int i=0;i<stamps.Length;i++){
            textures[i]=stamps[i].GetTerrainTexture();
        }
        return textures;

    }


    public TerrainDetailStamp[] GetTextureStamps(){

        


        selected.GetComponent<TerrainDetailStamp>().layerIndex=layerIndex;
        return new TerrainDetailStamp[]{selected.GetComponent<TerrainDetailStamp>()};
        


    }

}
