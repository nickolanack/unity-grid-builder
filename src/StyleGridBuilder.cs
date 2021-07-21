using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleGridBuilder : GridBuilder
{


    public Material activeMaterial;
    public Material tileMaterial;
    public Material fillerMaterial;

    // Start is called before the first frame update
    protected override void Start()
    {
        
        onActivate=delegate(GameObject tile, int x, int y){
            tile.GetComponent<Renderer>().material = activeMaterial;
        };
        onDeactivate=delegate(GameObject tile, int x, int y){
            tile.GetComponent<Renderer>().material = tileMaterial;
        };

        onFill=delegate(GameObject fillerTile, int x, int y){
            fillerTile.GetComponent<Renderer>().material = fillerMaterial;
        };

        onCommit=delegate(GameObject tile, int x, int y){
            tile.GetComponent<Renderer>().material = tileMaterial;
        };



         base.Start();
    }

 }
