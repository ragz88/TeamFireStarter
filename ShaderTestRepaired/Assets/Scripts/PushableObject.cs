using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour {

    GameObject player;
    bool beingPushed = false;
    Vector3 lerpPoint;

	// Use this for initialization
	void Start () {
        //player = GameObject.Find("ThirdPersonController");
	}
	
	// Update is called once per frame
	void Update () {

        RaycastHit rayHit;
        if (Physics.Raycast(transform.position,Vector3.forward,out rayHit,((transform.lossyScale.z/2) + 1)))
        {
            //lerpPoint = new Vector3(transform.position.x, player.transform.position.y, transform.position.z + (transform.lossyScale.z / 2) + 0.8f);
        }

        //beingPushed = false;
	}

}
