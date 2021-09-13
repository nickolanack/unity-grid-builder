using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Decal
{


    public static GameObject defaultPrefab;
    

    public void Display(Vector2 pos, GameObject decalPrefab){

        Debug.Log("Decal");

        if(decalPrefab==null){
            decalPrefab=Decal.defaultPrefab;
        }

        if(Decal.defaultPrefab==null&&decalPrefab!=null){
            Decal.defaultPrefab=decalPrefab;
        }



        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        
        if (Physics.Raycast(ray, out hit)) {
            
            Quaternion q=Quaternion.LookRotation(hit.normal);
            //Debug.Log("Instantiate");
            GameObject decal=Object.Instantiate(decalPrefab, hit.point+(q*Vector3.forward*0.002f), q);
            Quaternion.LookRotation(hit.normal);



            Animation anim = decal.AddComponent<Animation>();
            AnimationCurve curve;

            // create a new AnimationClip
            AnimationClip clip = new AnimationClip();
            clip.legacy = true;

            // create a curve to move the GameObject and assign to the clip
            Keyframe[] keys;
            keys = new Keyframe[3];
            keys[0] = new Keyframe(0.0f, 0.01f);
            keys[1] = new Keyframe(0.3f, 0.04f);
            keys[2] = new Keyframe(0.4f, 0.03f);

            foreach(string prop in new string[]{
                "localScale.x","localScale.y","localScale.z"
            }){

                curve = new AnimationCurve(keys);
                clip.SetCurve("", typeof(Transform), prop, curve);

            }


            keys = new Keyframe[2];
            keys[0] = new Keyframe(0.0f, 1f);
            keys[1] = new Keyframe(0.4f, 1f);

            foreach(string prop in new string[]{
              //  "m_Color.r", "m_Color.g", "m_Color.b"
            }){

                curve = new AnimationCurve(keys);
                clip.SetCurve("", typeof(SpriteRenderer), prop, curve);

            }

            keys = new Keyframe[2];
            keys[0] = new Keyframe(0.0f, 1f);
            keys[1] = new Keyframe(0.4f, 0f);

            
            curve = new AnimationCurve(keys);
            clip.SetCurve("", typeof(SpriteRenderer),  "m_Color.a", curve);

            
          
            // now animate the GameObject
            anim.AddClip(clip, clip.name);
            anim.Play(clip.name);
            Object.Destroy(decal, 0.5f);
        }
    }



    public GameObject Create(Vector3 pos, Vector3 normal, GameObject decalPrefab){

        Debug.Log("Decal");
        if(decalPrefab==null){
            decalPrefab=Decal.defaultPrefab;
        }

        if(Decal.defaultPrefab==null&&decalPrefab!=null){
            Decal.defaultPrefab=decalPrefab;
        }
       
            
        Quaternion q=Quaternion.LookRotation(normal);
        GameObject decal=Object.Instantiate(decalPrefab, pos+(normal*0.2f), q);

        return decal;
  
    }

    
}