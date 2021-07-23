using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameObjectEntityList : MonoBehaviour
{
    
    public Dictionary<string, GameEntity> items = new Dictionary<string, GameEntity>();



    public delegate void EntityEvent(GameEntity entity);
    public EntityEvent onAddEntity=delegate(GameEntity entity){

    };

    public EntityEvent onRemoveEntity=delegate(GameEntity entity){

    };

    public EntityEvent onUpdateEntity=delegate(GameEntity entity){

    };



    public void AddEntity(GameObject obj){

        GameEntity entity=obj.GetComponent<GameEntity>();
        if(entity==null){
            throw new Exception("Expected GameObject Entity to be initialized with GameEntity MonoBehaviour");
        }

        items.Add(entity.entityId, entity);
        onAddEntity(entity);

    }

}
