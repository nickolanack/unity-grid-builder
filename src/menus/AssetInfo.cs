using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetInfo : MonoBehaviour
{


    public Text text;
    public string assetName;
    public Image sprite;
    Sprite s;

    public string label;

    void Start()
    {
        if(text==null){
            Text[] t=gameObject.GetComponentsInChildren<Text>();
            if(t.Length>0){
                text=t[0];
            }
        }


        label=assetName;
        label=label.Replace(".prefab", "");
        string[] parts=label.Split('/');
        label=parts[parts.Length-1];

    }

    // Update is called once per frame
    void Update()
    {
        text.text=label;

        if(s==null&&gameObject.GetComponent<TerrainStamp>()!=null&&gameObject.GetComponent<TerrainStamp>().texture!=null){
            Texture2D t =gameObject.GetComponent<TerrainStamp>().texture;
            s=Sprite.Create(t, new Rect(0.0f, 0.0f, t.width, t.height), new Vector2(0.5f, 0.5f), 10f);
            sprite.sprite=s;
        }

    }
}
