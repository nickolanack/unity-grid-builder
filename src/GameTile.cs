using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameTile : MonoBehaviour
{
    
    public Dictionary<string, GameEntity> items = new Dictionary<string, GameEntity>();



    public delegate void EntityEvent(GameEntity entity);
    public EntityEvent onAddEntity=delegate(GameEntity entity){

    };

    public EntityEvent onRemoveEntity=delegate(GameEntity entity){

    };

    public EntityEvent onUpdateEntity=delegate(GameEntity entity){

    };


    public int x;
    public int y;

    public Vector3 terrainOffset=new Vector3(-5, 0.001f, -5);



    public void AddEntity(GameObject obj){

        GameEntity entity=obj.GetComponent<GameEntity>();
        if(entity==null){
            throw new Exception("Expected GameObject Entity to be initialized with GameEntity MonoBehaviour");
        }

        items.Add(entity.entityId, entity);
        onAddEntity(entity);

    }


    void Update(){
        Terrain[] terrain=gameObject.GetComponentsInChildren<Terrain>();
        if(terrain.Length==0){
            CreateTerrain();
        }
    }



      private static Vector2 tileAmount = Vector2.one;
   
    private float width  = 10;
    private float lenght = 10;
    private float height = 600;
   
    private int heightmapResoltion          = 513;
    private int detailResolution            = 1024;
    private int detailResolutionPerPatch    = 8;
    private int controlTextureResolution    = 512;
    private int baseTextureReolution        = 1024;


    private void CreateTerrain(){
 
        for(int x0 = 1; x0 <= tileAmount.x; x0++){
            for(int y0 = 1; y0 <= tileAmount.y; y0++){
               
                TerrainData terrainData = new TerrainData();
               
                // = (Alphabet)x;
                string name = "tile-"+ x0 + "-" + y0;
       
                terrainData.size = new Vector3( width / 16f,
                                                height,
                                                lenght / 16f);
               
                terrainData.baseMapResolution = baseTextureReolution;
                terrainData.heightmapResolution = heightmapResoltion;
                terrainData.alphamapResolution = controlTextureResolution;
                terrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);
 
                terrainData.name = name;
                GameObject terrain = (GameObject)Terrain.CreateTerrainGameObject(terrainData);

               
                terrain.name = name;
                terrain.transform.parent = transform;
                terrain.transform.localPosition = terrainOffset;
 
                foreach(GameTile gt in GridBuilder.Main.GetNeighborGameTiles(x, y)){

                    Terrain nt=gt.gameObject.GetComponentInChildren<Terrain>();
                    
                    Terrain left=null;
                    Terrain right=null;
                    Terrain bottom=null;
                    Terrain top=null;

                    Terrain current=terrain.GetComponent<Terrain>();

                    if(gt.x<x){
                        left=nt;                     
                    }
                    if(gt.x>x){
                        right=nt;
                    }
                    if(gt.y<y){
                        bottom=nt;
                    }
                    if(gt.y>y){
                        top=nt;
                    }

                    current.SetNeighbors (left, top, right, bottom);
                    Terrain.SetConnectivityDirty();

                }
               
            }
        }
 
 
       
    }
    

}
