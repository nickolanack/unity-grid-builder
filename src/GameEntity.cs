using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{

    public string assetName="cube";
    public string entityId="cube-01";
    public int x=0;
    public int y=0;


    public GridBuilder builder;
    
    void Update(){

        if(builder==null){
            builder=GridBuilder.Main;
            if(builder!=null){
                builder.AddEntity(gameObject, x, y);
            }
        }

    }


   
}
