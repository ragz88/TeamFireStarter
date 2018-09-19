using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmooth : MonoBehaviour {
    Vector3 oldPos;
    Quaternion oldRot;

    Vector3 newPos;
    Quaternion newRot;

    public float smoothingFactor = .1f;

    public Transform target; 
	// Use this for initialization
	void Start () {
		if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("PlayerBody").transform;
        }
        this.transform.SetParent(null);
	}
	
	// Update is called once per frame
	void Update () {

        oldPos = transform.position;
        oldRot = transform.rotation;
        newPos = target.position;
        newRot = target.rotation;

        transform.position = Vector3.Lerp(oldPos, newPos, smoothingFactor);
        transform.rotation = Quaternion.Lerp(oldRot, newRot, smoothingFactor);
        
		
	}
}
