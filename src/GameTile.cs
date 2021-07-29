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


    public GameObject terrainTemplate;

    public void AddEntity(GameObject obj){

        GameEntity entity=obj.GetComponent<GameEntity>();
        if(entity==null){
            throw new Exception("Expected GameObject Entity to be initialized with GameEntity MonoBehaviour");
        }

        items.Add(entity.entityId, entity);
        onAddEntity(entity);

    }


    void Update(){
        Terrain[] currentTerrain=gameObject.GetComponentsInChildren<Terrain>(true);
        if(currentTerrain.Length==0){


            
            GameObject terrain=null;
            if(terrainTemplate!=null){
                terrain=Instantiate(terrainTemplate);
                terrain.SetActive(true);
            }

            if(terrain==null){
                terrain=CreateBlankTerrain();
            }

            terrain.transform.parent = transform;
            terrain.transform.localPosition = terrainOffset;
            ConnectTerrain(terrain.GetComponent<Terrain>());

        }
    }



   
    private float width  = 10;
    private float lenght = 10;
    private float height = 600;
   
    private int heightmapResoltion          = 513;
    private int detailResolution            = 1024;
    private int detailResolutionPerPatch    = 8;
    private int controlTextureResolution    = 512;
    private int baseTextureReolution        = 1024;

    private GameObject CreateBlankTerrain(){
 
    
        TerrainData terrainData = new TerrainData();
       
        // = (Alphabet)x;
        string name = "terrain-"+ x + "-" + y;

        terrainData.size = new Vector3(width / 16f, height, lenght / 16f);
       
        terrainData.baseMapResolution = baseTextureReolution;
        terrainData.heightmapResolution = heightmapResoltion;
        terrainData.alphamapResolution = controlTextureResolution;
        terrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);

        terrainData.name = name;
        GameObject terrain = (GameObject)Terrain.CreateTerrainGameObject(terrainData);
        terrain.name = name;

       
        return terrain;
       
    }

    private void ConnectTerrain(Terrain current){

        Terrain left=null;
        Terrain right=null;
        Terrain bottom=null;
        Terrain top=null;

        foreach(GameTile gt in GridBuilder.Main.GetNeighborGameTiles(x, y)){

            Terrain nt=gt.gameObject.GetComponentInChildren<Terrain>();
            
           
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
    

        }

        current.SetNeighbors (left, top, right, bottom);
        Terrain.SetConnectivityDirty();


    }




}
