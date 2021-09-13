using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{


	public GameObject DoorObject;
	public GameObject Hinge;


	public bool open=false;

	public bool opening=false;
	public bool closing=false;

	public float speed=150f;

	public AudioClip openSound;
	public AudioClip closeSound;



	void Start(){


	}

	void Update(){


		if(open){


			if((!opening)&&DoorObject.transform.localEulerAngles.y<100f-.01f){



				AudioSource audio=GetComponent<AudioSource>();
				if(openSound!=null){
					audio.clip=openSound;
					audio.Play();
				}

				Debug.Log("start opening");
				opening=true;
			}


			if(opening){


				if(DoorObject.transform.localEulerAngles.y>=100f){
					Debug.Log("stop opening");
					opening=false;

					float r = DoorObject.transform.localEulerAngles.y;
					r-=100f;
					DoorObject.transform.RotateAround(Hinge.transform.position, DoorObject.transform.up, -r);

					return;
				}

				DoorObject.transform.RotateAround(Hinge.transform.position, DoorObject.transform.up, speed*Time.deltaTime);
				return;
			}


			return;
		}
		

		if((!closing)&&DoorObject.transform.localEulerAngles.y>0.01f){
			Debug.Log("start closing");
			closing=true;
		}

		if(closing){

			if((DoorObject.transform.localEulerAngles.y+180)%360<=180f){

				AudioSource audio=GetComponent<AudioSource>();
				if(closeSound!=null){
					audio.clip=closeSound;
					audio.Play();
				}


				closing=false;
				Debug.Log("stop closing");
				float r = DoorObject.transform.localEulerAngles.y;
				DoorObject.transform.RotateAround(Hinge.transform.position, DoorObject.transform.up, -r);
				return;
			}

			DoorObject.transform.RotateAround(Hinge.transform.position, DoorObject.transform.up, -speed*Time.deltaTime);
			return;


		}
	}

		



	public bool Open(){
		return false;
	}

	public bool Close(){
		return false;
	}
	

}