using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class GridBuilder : MonoBehaviour
{


    public static GridBuilder Main;

    public GameObject mainTile;
    public GameObject templateTile;
    public GameObject activeTile=null;



    protected delegate Vector3 GetTilePosition(int x, int y);
    protected GetTilePosition getTilePosition=delegate(int x, int y){
        return GridBuilder.Main.GetTile(0,0).transform.position+new Vector3(10*x, 0, 10*y);
    };


    protected delegate List<int[]> GetTileNeighbors(int x, int y);
    protected GetTileNeighbors getTileNeighbors=delegate(int x, int y){

       List<int[]> list= new List<int[]>(){
            new int[] {x-1, y},
            new int[] {x+1, y},
            new int[] {x, y-1}, 
            new int[] {x, y+1}
        };

        return list;
    };




    public delegate void EntityEvent(GameEntity tile, int x, int y);
    public delegate void TileEvent(GameObject tile, int x, int y);
    
    protected TileEvent onActivate=delegate(GameObject tile, int x, int y){

    };

    protected TileEvent onDeactivate=delegate(GameObject tile, int x, int y){

    };

    protected TileEvent onFill=delegate(GameObject fillerTile, int x, int y){

    };

    protected TileEvent onCommit=delegate(GameObject tile, int x, int y){

    };


    List<TileEvent> onActivateTileListeners=new List<TileEvent>();
    List<TileEvent> onDeactivateTileListeners=new List<TileEvent>();



    protected EntityEvent onAddEntity=delegate(GameEntity entity, int x, int y){

    };

    protected EntityEvent onRemoveEntity=delegate(GameEntity entity, int x, int y){

    };

    protected EntityEvent onUpdateEntity=delegate(GameEntity entity, int x, int y){

    };



    protected delegate GameObject CreateFillerTile(int x, int y);
    protected CreateFillerTile createFillerTile=delegate(int x, int y){
        GameObject tile = Instantiate(GridBuilder.Main.templateTile);
        tile.SetActive(true);
        return tile;
    };


    Dictionary<KeyValuePair<int, int>, GameObject> grid=new Dictionary<KeyValuePair<int, int>, GameObject>();
    Dictionary<KeyValuePair<int, int>, GameObject> fillers=new Dictionary<KeyValuePair<int, int>, GameObject>();


    // Start is called before the first frame update
    protected virtual void Start()
    {

        GridBuilder.Main=this;
        templateTile=Instantiate(mainTile, transform);
        templateTile.name="Template Tile";

        Terrain[] terrain=templateTile.GetComponentsInChildren<Terrain>();
        foreach(Terrain t in terrain){
            Destroy(t.gameObject);
        }

        templateTile.SetActive(false);
        AddTile(0,0, mainTile);
        SetActiveTile(0,0);

    }

    public void AddEntity(GameObject obj, int x, int y){

        GameEntity entity=obj.GetComponent<GameEntity>();
        if(entity==null){
            throw new Exception("Expected GameObject Entity to be initialized with GameEntity MonoBehaviour");
        }

        GameObject tile=GetTile(x,y);
        GameTile list=tile.GetComponent<GameTile>();
        Debug.Log("Add Entity: "+entity.entityId);
        list.AddEntity(obj);

    }


    public void OnActivateTile(TileEvent listener){
        onActivateTileListeners.Add(listener);
    }

    public void OnDeactivateTile(TileEvent listener){
        onDeactivateTileListeners.Add(listener);
    }


    void AddTile(int x, int y, GameObject tile){

        Debug.Log("Add Tile: "+x+" "+y);

        grid.Add(new KeyValuePair<int, int>(x,y), tile);
        GameTile list=tile.GetComponent<GameTile>();
        if(list==null){
            list=tile.AddComponent<GameTile>(); 
        }

        if(tile.GetComponent<GameFillerTile>()){
            Destroy(tile.GetComponent<GameFillerTile>());
        }

        list.x=x;
        list.y=y;

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

    public List<GameTile> GetNeighborGameTiles(int x, int y){
        List<GameTile> list=new List<GameTile>(); 
        foreach(int[]  c in getTileNeighbors(x, y)){
            if(HasTile(c[0], c[1])){
                list.Add(GetTile(c[0], c[1]).GetComponent<GameTile>());
            }
        }
        return list;
    }


    List<int[]> GetNeighbors(int x, int y){
        return getTileNeighbors(x, y);
    }

    Vector3 GetPosition(int x, int y){

        return getTilePosition(x, y);
    }


    public void SetActiveTile(GameObject tile){
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

    public void SetActiveTile(int x, int y){
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
            foreach(TileEvent listener in onDeactivateTileListeners){
                listener(activeTile,x,y);
            }
        }
        activeTile=tile;
        activeTile.name="Tile: "+x+","+y;
        onActivate(activeTile,x,y);
        foreach(TileEvent listener in onActivateTileListeners){
            listener(activeTile,x,y);
        }
        
        foreach(int[] t in GetNeighbors(x, y)){



            if(!ContainsTile(t[0], t[1])){
                GameObject fillerTile=(GameObject)createFillerTile(t[0], t[1]); //, GetPosition(t[0], t[1]), Quaternion.identity);

                GameFillerTile f=fillerTile.GetComponent<GameFillerTile>();
                if(f==null){
                    f=fillerTile.AddComponent<GameFillerTile>();
                }
                f.x=t[0];
                f.y=t[1];

                fillerTile.transform.parent=transform;
                fillerTile.transform.position=GetPosition(t[0], t[1]);
                fillerTile.name="Filler: "+t[0]+","+t[1];
                AddFiller(t[0], t[1], fillerTile);
                onFill(fillerTile, t[0], t[1]);
            }
        }


    }

    GameObject GetTile(int x, int y){
        KeyValuePair<int, int> t=new KeyValuePair<int, int>(x,y);
        return grid[t];
    }

    public GameObject GetActiveTile(){
        return activeTile;
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
