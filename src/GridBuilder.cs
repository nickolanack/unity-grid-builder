using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class GridBuilder : MonoBehaviour
{


    public static GridBuilder Main;

    public GameObject mainTile;
    public GameObject activeTile=null;



    public delegate Vector3 GetTilePosition(int x, int y);
    public GetTilePosition getTilePosition=delegate(int x, int y){
        return GridBuilder.Main.mainTile.transform.position+new Vector3(10*x, 0, 10*y);
    };


    public delegate List<int[]> GetTileNeighbors(int x, int y);
    public GetTileNeighbors getTileNeighbors=delegate(int x, int y){

       List<int[]> list= new List<int[]>(){
            new int[] {x-1, y},
            new int[] {x+1, y},
            new int[] {x, y-1}, 
            new int[] {x, y+1}
        };

        return list;
    };




    public delegate void EntityEvent(GameEntity tile, int x, int y);
    public delegate void TilePrepare(GameObject tile, int x, int y);
    public TilePrepare onActivate=delegate(GameObject tile, int x, int y){

    };

    public TilePrepare onDeactivate=delegate(GameObject tile, int x, int y){

    };

    public TilePrepare onFill=delegate(GameObject fillerTile, int x, int y){

    };

    public TilePrepare onCommit=delegate(GameObject tile, int x, int y){

    };



    public EntityEvent onAddEntity=delegate(GameEntity entity, int x, int y){

    };

    public EntityEvent onRemoveEntity=delegate(GameEntity entity, int x, int y){

    };

    public EntityEvent onUpdateEntity=delegate(GameEntity entity, int x, int y){

    };



    public delegate GameObject CreateFillerTile(int x, int y);
    public CreateFillerTile createFillerTile=delegate(int x, int y){
        return Instantiate(GridBuilder.Main.mainTile);
    };


    Dictionary<KeyValuePair<int, int>, GameObject> grid=new Dictionary<KeyValuePair<int, int>, GameObject>();
    Dictionary<KeyValuePair<int, int>, GameObject> fillers=new Dictionary<KeyValuePair<int, int>, GameObject>();


    // Start is called before the first frame update
    protected virtual void Start()
    {

        GridBuilder.Main=this;
        AddTile(0,0, mainTile);
        SetActiveTile(0,0);

    }

    public void AddEntity(GameObject obj, int x, int y){

        GameEntity entity=obj.GetComponent<GameEntity>();
        if(entity==null){
            throw new Exception("Expected GameObject Entity to be initialized with GameEntity MonoBehaviour");
        }

        GameObject tile=GetTile(x,y);
        GameObjectEntityList list=tile.GetComponent<GameObjectEntityList>();
        Debug.Log("Add Entity: "+entity.entityId);
        list.AddEntity(obj);

    }


    void AddTile(int x, int y, GameObject tile){

        Debug.Log("Add Tile: "+x+" "+y);

        grid.Add(new KeyValuePair<int, int>(x,y), tile);
        GameObjectEntityList list=tile.GetComponent<GameObjectEntityList>();
        if(list==null){
            list=tile.AddComponent<GameObjectEntityList>();
        }

        list.onAddEntity=delegate(GameEntity entity){
            onAddEntity(entity, x, y);
        };

        list.onRemoveEntity=delegate(GameEntity entity){
            onRemoveEntity(entity, x, y);
        };

        list.onUpdateEntity=delegate(GameEntity entity){
            onUpdateEntity(entity, x, y);
        };


    }

    void AddFiller(int x, int y, GameObject tile){
        fillers.Add(new KeyValuePair<int, int>(x,y), tile);
    }


    List<int[]> GetNeighbors(int x, int y){
        return getTileNeighbors(x, y);
    }

    Vector3 GetPosition(int x, int y){

        return getTilePosition(x, y);
    }


    protected void SetActiveTile(GameObject tile){
        foreach(KeyValuePair<KeyValuePair<int, int>, GameObject> entry in fillers)
        {
            if(entry.Value==tile){
                SetActiveTile(entry.Key.Key, entry.Key.Value);
                return;
            }
        }

        foreach(KeyValuePair<KeyValuePair<int, int>, GameObject> entry in grid)
        {
            if(entry.Value==tile){
                SetActiveTile(entry.Key.Key, entry.Key.Value);
                return;
            }
        }
    }

    protected void SetActiveTile(int x, int y){
        if(!HasTile(x, y)){

            if(!HasFillerTile(x,y)){
                throw new Exception("Tile does not exist at "+x+", "+y);   
            }
            Debug.Log("Commit filler tile");
            CommitFillerTile(x, y);
        }

        GameObject tile=GetTile(x,y);

        if(tile==activeTile){
            return;
        }


        if(activeTile!=null){
            onDeactivate(activeTile,x,y);
        }
        activeTile=tile;
        onActivate(activeTile,x,y);
        
        foreach(int[] t in GetNeighbors(x, y)){



            if(!ContainsTile(t[0], t[1])){
                GameObject fillterTile=(GameObject)createFillerTile(t[0], t[1]); //, GetPosition(t[0], t[1]), Quaternion.identity);
                fillterTile.transform.position=GetPosition(t[0], t[1]);
                AddFiller(t[0], t[1], fillterTile);
                onFill(fillterTile, t[0], t[1]);
            }
        }


    }

    GameObject GetTile(int x, int y){
        KeyValuePair<int, int> t=new KeyValuePair<int, int>(x,y);
        return grid[t];
    }

    bool HasTile(int x, int y){
        KeyValuePair<int, int> t=new KeyValuePair<int, int>(x,y);
        return grid.ContainsKey(t);
    }

    bool HasFillerTile(int x, int y){
        KeyValuePair<int, int> t=new KeyValuePair<int, int>(x,y);
        return fillers.ContainsKey(t);
    }

    public bool ContainsTile(int x, int y){
        return HasTile(x, y)||HasFillerTile(x, y);
    }

    public bool ContainsTile(GameObject obj){
        return grid.ContainsValue(obj)||fillers.ContainsValue(obj);
    }

    void CommitFillerTile(int x, int y){
        KeyValuePair<int, int> t=new KeyValuePair<int, int>(x,y);

        GameObject tile=fillers[t];
        AddTile(x, y, tile);
        fillers.Remove(t);

        onCommit(tile, x, y);

    }
}
