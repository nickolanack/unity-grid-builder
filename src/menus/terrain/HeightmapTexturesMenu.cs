using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightmapTexturesMenu : AssetsMenuBase
{


    public float scale=0.3f;

    public string[] textureNames=new string[]{

    };

    protected override void InitAsset(GameObject assetObject, string assetName){
        TerrainStamp stamp=assetObject.AddComponent<TerrainStamp>();
        stamp.textureName=assetName;
    }

    protected override void LoadAssets(){


        foreach(string textureName in textureNames){
            assetQueue.Enqueue(textureName);
        }
    }


    public TerrainTexture[] GetTerrainTextures(){

        TerrainStamp[] stamps=GetTextureStamps();

        TerrainTexture[] textures=new TerrainTexture[stamps.Length];
        for(int i=0;i<stamps.Length;i++){
            textures[i]=stamps[i].GetTerrainTexture();
        }
        return textures;

    }


    public TerrainStamp[] GetTextureStamps(){

        selected.GetComponent<TerrainStamp>().scale=scale;
        selected.GetComponent<TerrainStamp>().modifier=delegate(Texture2D texture){


            for (int y = 0; y < texture.height; y++){
                for (int x = 0; x < texture.width; x++)
                {

                    Color color=texture.GetPixel(x, y);
                    float value=color.r;

                    value+=Mathf.PerlinNoise(x/(texture.width/5f), y/(texture.height/5f));//*0.2f;
                    value+=Mathf.PerlinNoise(x/(texture.width/2f), y/(texture.height/2f))*.3f;//*0.2f;

                    

                    int cx=texture.width/2;
                    int cy=texture.height/2;

                    float dx=x-cx;
                    float dy=y-cy;

                    float d=dx*dx+dy*dy;
                    float c=cx*cx+cy*cy;

                    value*=Mathf.Min(1, d/(c/3f));
                    value*=Mathf.Min(1, ((c+c/5f)-d)/(c/3f));


                    value+=Mathf.PerlinNoise(x/(texture.width/2f), y/(texture.height/2f))*.1f;//*0.2f;




                    value*=Mathf.Min(1, x/(texture.width/4f));
                    value*=Mathf.Min(1, (texture.width-x)/(texture.width/4f));

                    value*=Mathf.Min(1, y/(texture.height/4f));
                    value*=Mathf.Min(1, (texture.height-y)/(texture.height/4f));




                    color.r=value;

                    texture.SetPixel(x, y, color);


                }
            }

            return texture;


        };
        return new TerrainStamp[] {selected.GetComponent<TerrainStamp>()};
    }







}
