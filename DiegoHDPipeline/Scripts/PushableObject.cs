using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour {

    GameObject player;
    public bool beingPushed = false;
    //public Transform[] pushTransforms;
    public Transform[] pushPoints;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("ThirdPersonController");
	}
	
	// Update is called once per frame
	void Update () {
        
	}

}
