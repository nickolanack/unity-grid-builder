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

                TerrainExport export=terrain.GetComponent<TerrainExport>();
                if(export!=null){
                    Destroy(export);
                }


                CopyTerrain(terrain.GetComponent<Terrain>(), terrainTemplate.GetComponent<Terrain>());
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
    private float height = 100;
   
    private int heightmapResolution          = 513;
    private int detailResolution            = 256;
    private int detailResolutionPerPatch    = 32;
    private int controlTextureResolution    = 512;
    private int baseTextureResolution        = 1024;


    private void CopyTerrain(Terrain instance, Terrain template){

        TerrainData terrainData = new TerrainData();
       
        // = (Alphabet)x;
        string name = "terrain-"+ x + "-" + y;


        Vector3 size= template.terrainData.size/ 2f;
        size.y*=2f;
        terrainData.size=size;
       
        terrainData.baseMapResolution = template.terrainData.baseMapResolution;
        terrainData.heightmapResolution = template.terrainData.heightmapResolution;
        terrainData.alphamapResolution = template.terrainData.alphamapResolution;
        terrainData.SetDetailResolution(template.terrainData.detailResolution, template.terrainData.detailResolutionPerPatch);

        terrainData.wavingGrassSpeed = template.terrainData.wavingGrassSpeed;
        terrainData.wavingGrassAmount = template.terrainData.wavingGrassAmount;
        terrainData.wavingGrassStrength = template.terrainData.wavingGrassStrength;

        terrainData.treePrototypes=template.terrainData.treePrototypes;
        terrainData.terrainLayers=template.terrainData.terrainLayers;
        terrainData.detailPrototypes=template.terrainData.detailPrototypes;
        TerrainCollider collider=instance.gameObject.GetComponent<TerrainCollider>();
        collider.terrainData=terrainData;
        instance.terrainData=terrainData;

    }

    private GameObject CreateBlankTerrain(){
 
    
        TerrainData terrainData = new TerrainData();
       
        // = (Alphabet)x;
        string name = "terrain-"+ x + "-" + y;


        terrainData.size = new Vector3(width / 8f, height, lenght / 8f);
       
        terrainData.baseMapResolution = baseTextureResolution;
        terrainData.heightmapResolution = heightmapResolution;
        terrainData.alphamapResolution = controlTextureResolution;
        terrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);
        

        terrainData.name = name;
        GameObject terrain = (GameObject)Terrain.CreateTerrainGameObject(terrainData);
        terrain.name = name;

       
        return terrain;
       
    }

    private void ConnectTerrain(Terrain newTerrain){

        Terrain left=null;
        Terrain right=null;
        Terrain bottom=null;
        Terrain top=null;

        foreach(GameTile gt in GridBuilder.Main.GetNeighborGameTiles(x, y)){

            Terrain nt=gt.gameObject.GetComponentInChildren<Terrain>();
            
           
            if(gt.x<x){
                left=nt;    
                left.SetNeighbors(left.leftNeighbor, left.topNeighbor, newTerrain, left.bottomNeighbor);    
                Debug.Log("Connect new left:"+gt.x+", "+gt.y+ " of ("+x+","+y+")" );        
            }
            if(gt.x>x){
                right=nt;
                right.SetNeighbors(newTerrain, right.topNeighbor, right.rightNeighbor, right.bottomNeighbor);
                Debug.Log("Connect new right:"+gt.x+", "+gt.y+ " of ("+x+","+y+")" );   
            }
            if(gt.y<y){
                bottom=nt;
                bottom.SetNeighbors(bottom.leftNeighbor, newTerrain, bottom.rightNeighbor, bottom.bottomNeighbor);
                Debug.Log("Connect new  bottom:"+gt.x+", "+gt.y+ " of ("+x+","+y+")" );   
            }
            if(gt.y>y){
                top=nt;
                top.SetNeighbors(top.leftNeighbor, top.topNeighbor, top.rightNeighbor, newTerrain);
                Debug.Log("Connect new top:"+gt.x+", "+gt.y+ " of ("+x+","+y+")" );   
            }
    

        }

        newTerrain.SetNeighbors(left, top, right, bottom);

        if(left!=null&&(left.rightNeighbor!=newTerrain||newTerrain.leftNeighbor!=left)){
            throw new Exception("Invalid assignment");
        }
        if(right!=null&&(right.leftNeighbor!=newTerrain||newTerrain.rightNeighbor!=right)){
            throw new Exception("Invalid assignment");
        }
        if(top!=null&&(top.bottomNeighbor!=newTerrain||newTerrain.topNeighbor!=top)){
            throw new Exception("Invalid assignment");
        }
        if(bottom!=null&&(bottom.topNeighbor!=newTerrain||newTerrain.bottomNeighbor!=bottom)){
            throw new Exception("Invalid assignment");
        }

        Terrain.SetConnectivityDirty();


    }




}
