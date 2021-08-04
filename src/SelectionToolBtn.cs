using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionToolBtn : MonoBehaviour
{
    public delegate bool ToolBtnFunction();
    public ToolBtnFunction enableFunction;
    public ToolBtnFunction disableFunction;


    public Color enabledColor=Color.green;
    public Color disabledColor=Color.black;
    public Image sprite;


    public bool active=false;

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
