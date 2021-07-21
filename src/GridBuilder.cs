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

    public delegate void TilePrepare(GameObject tile, int x, int y);
    public TilePrepare onActivate=delegate(GameObject tile, int x, int y){

    };

    public TilePrepare onDeactivate=delegate(GameObject tile, int x, int y){

    };

    public TilePrepare onFill=delegate(GameObject fillerTile, int x, int y){

    };

    public TilePrepare onCommit=delegate(GameObject tile, int x, int y){

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
        SetActive(0,0);

    }


    void AddTile(int x, int y, GameObject tile){
        grid.Add(new KeyValuePair<int, int>(x,y), tile);
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


    void SetActive(int x, int y){
        if(!HasTile(x, y)){

            if(!HasFillerTile(x,y)){
                throw new Exception("Tile does not exist at "+x+", "+y);   
            }
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



            if(!HasTile(t[0], t[1])){
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


    void CommitFillerTile(int x, int y){
        KeyValuePair<int, int> t=new KeyValuePair<int, int>(x,y);

        GameObject tile=fillers[t];
        grid.Add(t, tile);
        fillers.Remove(t);

        onCommit(tile, x, y);

    }
}
