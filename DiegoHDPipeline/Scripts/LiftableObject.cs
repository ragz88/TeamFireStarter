using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftableObject : MonoBehaviour {

    public Vector3 initialPos;
    public bool beingCarried = false;
    public ObjectInteractions Interactions;

	// Use this for initialization
	void Start () {
        initialPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
