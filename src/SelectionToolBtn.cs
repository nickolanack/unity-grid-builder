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


    public bool enabled=false;

    public void Enable(){
        if(enabled){
            return;
        }

        bool success=enableFunction();
        enabled=success;

        if(enabled){

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
        if(!enabled){
            return;
        }

        bool success=disableFunction();
        enabled=!success;

        if(!enabled){

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
        if(enabled){
            Disable();
            return;
        }
        Enable();
    }

}
