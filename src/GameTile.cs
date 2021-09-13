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



    public delegate void TerrainEvent(Terrain terrain);
    public TerrainEvent onCreateTerrain=delegate(Terrain terrain){

    };





    public int x;
    public int y;

    


    public GameObject terrainTemplate;

    public void AddEntity(GameObject obj){

        GameEntity entity=obj.GetComponent<GameEntity>();
        if(entity==null){
            throw new Exception("Expected GameObject Entity to be initialized with GameEntity MonoBehaviour");
        }

        items.Add(entity.entityId, entity);
        onAddEntity(entity);

    }


    public void RemoveEntity(GameObject obj){

        GameEntity entity=obj.GetComponent<GameEntity>();
        if(entity==null){
            throw new Exception("Expected GameObject Entity to be initialized with GameEntity MonoBehaviour");
        }

        items.Remove(entity.entityId);
        onRemoveEntity(entity);

    }


    


    void Update(){
        Terrain[] currentTerrain=gameObject.GetComponentsInChildren<Terrain>(true);
        if(currentTerrain.Length==0){


            
            GameObject terrain=null;
                if(terrainTemplate==null){
                    throw new Exception("terrainTemplate is null");
                }

                terrain=(new TerrainCopy()).FromPrefab(terrainTemplate);


                TerrainExport export=terrain.GetComponent<TerrainExport>();
                if(export!=null){
                    Destroy(export);
                }

                terrain.SetActive(true);
            

            if(terrain==null){
                throw new Exception("Needs template");
                //terrain=CreateBlankTerrain();
            }

            Vector3 terrainOffset=new Vector3(-5, 1f, -5);
            //
            
            //Vector3 size=terrainTemplate.GetComponent<Terrain>().terrainData.size;
            //Vector3 terrainOffset=new Vector3(-size.x/2, 0.001f, -size.z/2);
            terrain.transform.parent = transform;
            terrain.transform.localPosition = terrainOffset;
            ConnectTerrain(terrain.GetComponent<Terrain>());

            if(onCreateTerrain!=null){
                onCreateTerrain(terrain.GetComponent<Terrain>());
            }

        }
    }



    public GameTile ResolveNeighbor(int xOffset, int yOffset){

        if(!GridBuilder.Main.HasTile(x+xOffset, y+yOffset)){
            return null;
        }

        return GridBuilder.Main.GetTile(x+xOffset, y+yOffset).GetComponent<GameTile>();

    }


    private void ConnectTerrain(Terrain newTerrain){

        Terrain left=newTerrain.leftNeighbor;
        Terrain right=newTerrain.rightNeighbor;
        Terrain bottom=newTerrain.bottomNeighbor;
        Terrain top=newTerrain.topNeighbor;

        if(newTerrain==null){
            throw new Exception("Null Terrain");
        }

        foreach(GameTile gt in GridBuilder.Main.GetNeighborGameTiles(x, y)){

            Terrain nt=gt.gameObject.GetComponentInChildren<Terrain>();
            if(nt==null){
                continue;
                //throw new Exception("Null Terrain");
            }
            
           
            if(gt.x<x){
                left=nt;    
                left.SetNeighbors(left.leftNeighbor, left.topNeighbor, newTerrain, left.bottomNeighbor);    
                //Debug.Log("Connect new left:"+gt.x+", "+gt.y+ " of ("+x+","+y+")" );        
            }
            if(gt.x>x){
                right=nt;
                right.SetNeighbors(newTerrain, right.topNeighbor, right.rightNeighbor, right.bottomNeighbor);
                //Debug.Log("Connect new right:"+gt.x+", "+gt.y+ " of ("+x+","+y+")" );   
            }
            if(gt.y<y){
                bottom=nt;
                bottom.SetNeighbors(bottom.leftNeighbor, newTerrain, bottom.rightNeighbor, bottom.bottomNeighbor);
                //Debug.Log("Connect new  bottom:"+gt.x+", "+gt.y+ " of ("+x+","+y+")" );   
            }
            if(gt.y>y){
                top=nt;
                top.SetNeighbors(top.leftNeighbor, top.topNeighbor, top.rightNeighbor, newTerrain);
                //Debug.Log("Connect new top:"+gt.x+", "+gt.y+ " of ("+x+","+y+")" );   
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
