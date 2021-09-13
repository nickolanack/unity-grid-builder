using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFeature : MonoBehaviour
{



    public bool additive=true;
    public bool mergeHeight=true;
    public bool autoSeed=true;


    public bool addHeight=true;
    public bool addTrees=true;
    public bool addDetails=true;
    public bool addObjects=true;
    public bool addTextures=true;


    private delegate void DrawDelegate(Vector2 offset, Terrain terrain);


    public UIActionBtn
 enableTrees;
    public UIActionBtn
 enableHeight;
    public UIActionBtn
 enableDetails;
    public UIActionBtn
 enableObjects;
    public UIActionBtn
 enableTextures;


    public TerrainDetailStamp[] detailStamps;

    public AssetsMenu assetsMenu;
    public HeightmapTexturesMenu heightMapsMenu;
    public DetailTexturesMenu detailMapsMenu;
    public TexturesMenu textureMapsMenu;


    Terrain ResolveNeighborTerrain(int x, int y, Terrain terrain){

        GameTile gt=terrain.transform.parent.GetComponent<GameTile>();

        GameTile n=gt.ResolveNeighbor(x, y);
        if(n!=null){
            return n.GetComponentInChildren<Terrain>();
        }
        return null;


        // if(!GridBuilder.Main.HasTile(gt.x+x, gt.y+y)){
        //     return null;
        // }

        // return GridBuilder.Main.GetTile(gt.x+x, gt.y+y).GetComponentInChildren<Terrain>();

    }


    public void DrawFeature(Vector2 pos){

       
        TerrainEdit editor=new TerrainEdit();

        editor.AtScreenPos(pos, delegate(Vector3 pos, Terrain t){

            editor.additive=additive;
            editor.mergeHeight=mergeHeight;
            editor.neighborResolver=delegate(int x, int y, Terrain terrain){
                return ResolveNeighborTerrain(x,y,terrain);
            };

            DrawOnTerrain(pos, t, editor);


        });
        
        




    }




    void DrawOnTerrain(Vector3 pos, Terrain terrain, TerrainEdit editor){

    
        if(enableTrees!=null){
            addTrees=enableTrees.gameObject.activeSelf&&enableTrees.active;
        }

        if(enableHeight!=null){
            addHeight=enableHeight.gameObject.activeSelf&&enableHeight.active;
        }

        if(enableDetails!=null){
            addDetails=enableDetails.gameObject.activeSelf&&enableDetails.active;
        }

        if(enableObjects!=null){
            addObjects=enableObjects.gameObject.activeSelf&&enableObjects.active;
        }

        if(enableTextures!=null){
            addTextures=enableTextures.gameObject.activeSelf&&enableTextures.active;
        }


        
        if(addHeight){

            if(heightMapsMenu==null){
                Debug.Log("heightMapsMenu not set");
                return;
            }

            editor.DrawHeight(pos, terrain, heightMapsMenu.GetTerrainTextures());
        }
        

        if(addDetails){

            if(detailMapsMenu==null){
                Debug.Log("detailMapsMenu not set");
               
                return;
            }
            editor.DrawDetails(pos, terrain, detailMapsMenu.GetTerrainTextures());
            
        }

        if(addTextures){

            if(textureMapsMenu==null){
                Debug.Log("textureMapsMenu not set");
                return;
            }

            editor.DrawTexture(pos, terrain, textureMapsMenu.GetTerrainTextures());
        }


        if(addObjects){
            DrawObject(pos, terrain, null);
        }

       

        if(addTrees){
            editor.DrawTrees(pos, terrain);
        }


        

    }

    void DrawObject(Vector3 pos, Terrain terrain, AssetCreateButton.AssetCreated del){

        Vector3 worldPosition=(pos*terrain.terrainData.size.x)+terrain.transform.position;
        DrawObject(worldPosition, del);
    }

    public void DrawObject(Vector3 worldPosition, AssetCreateButton.AssetCreated del){

        if(assetsMenu==null||assetsMenu.selected==null){
            return;
        }
        assetsMenu.selected.GetComponent<AssetCreateButton>().CreateAssetAt(worldPosition, delegate(GameObject asset){
            Mesh mesh = asset.GetComponent<MeshFilter>().mesh;
            if(mesh!=null){
                Debug.Log("Mesh Min: "+mesh.bounds.min);
            }

            if(del!=null){
                del(asset);
            }

        });

     }

}
