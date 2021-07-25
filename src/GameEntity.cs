using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{

    public string library="cube";
    public string assetName="cube";
    public string entityId="cube-01";
    public int x=0;
    public int y=0;

    public bool autoAdd=false;


    public GridBuilder builder;
    
    void Update(){

        if(autoAdd&&builder==null){
            builder=GridBuilder.Main;
            if(builder!=null){
                builder.AddEntity(gameObject, x, y);
            }
        }

    }


   
}
