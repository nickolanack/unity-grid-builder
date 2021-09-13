using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableAssetMenuBtn : MonoBehaviour
{

    public GameObject selectedImage;
    public void Select(){
        if(selectedImage!=null){
            selectedImage.SetActive(true);
        }
    }

    public void Deselect(){
        if(selectedImage!=null){
            selectedImage.SetActive(false);
        }
    }

}
