using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIActionBtn: MonoBehaviour
{
    public delegate bool ToolBtnFunction();
    public ToolBtnFunction enableFunction;
    public ToolBtnFunction disableFunction;


    public Color enabledColor=Color.green;
    public Color disabledColor=Color.black;
    public Image sprite;


    public bool active=false;
    
    public bool initSelf=false;
    public bool canDisableSelf=true;
    public GameObject[] activatesObjects;
    public MonoBehaviour[] enablesBehaviors;
    public UIActionBtn[] disablesButtons;



    void Start(){
        if(initSelf){
            gameObject.GetComponent<Button>().onClick.AddListener(delegate(){
                if(!canDisableSelf){
                    Enable();
                    return;
                }
                Toggle();
            });

            enableFunction=delegate(){

                enableObjects();
                return true;
            };
            disableFunction=delegate(){
                
                disableObjects();
                return true;
            };
        }

        if(active){
            if(sprite==null){
                sprite=gameObject.GetComponent<Image>();
            }

            if(sprite!=null&&enabledColor!=null){
                if(disabledColor==null){
                    disabledColor=sprite.color;
                }
                sprite.color=enabledColor;
            }
            enableObjects();
            return;
        }
        disableObjects();



    }

    protected void disableObjects(){
        foreach(GameObject obj in activatesObjects){
            obj.SetActive(false);
        }

        foreach(MonoBehaviour script in enablesBehaviors){
            script.enabled=false;
        }
    }

    protected void enableObjects(){
        foreach(GameObject obj in activatesObjects){
            obj.SetActive(true);
        }

        foreach(MonoBehaviour script in enablesBehaviors){
            script.enabled=true;
        }

        foreach(UIActionBtn btn in disablesButtons){
            btn.Disable();
        }
    }

    public void Enable(){
        if(active){
            return;
        }

        bool success=enableFunction();
        active=success;

        if(active){

       
            if(sprite==null){
                sprite=gameObject.GetComponent<Image>();
            }

            if(sprite!=null&&enabledColor!=null){
                if(disabledColor==null){
                    disabledColor=sprite.color;
                }
                sprite.color=enabledColor;
            }
        }
    }

    public void Disable(){
        if(!active){
            return;
        }

        bool success=disableFunction();
        active=!success;

        if(!active){

            if(sprite==null){
                sprite=gameObject.GetComponent<Image>();
            }
            
            if(sprite!=null&&disabledColor!=null){
                if(enabledColor==null){
                    enabledColor=sprite.color;
                }
                sprite.color=disabledColor;
            }
        }
    }

    public void Toggle(){
        if(active){
            Disable();
            return;
        }
        Enable();
    }

}
