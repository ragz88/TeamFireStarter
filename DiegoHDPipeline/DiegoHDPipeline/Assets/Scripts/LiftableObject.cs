﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftableObject : MonoBehaviour {

    public Vector3 initialPos;
    public bool beingCarried = false;
    public ObjectInteractions Interactions;
    public float dissolveTime = 1;
    public GameObject dissolveEffects;

    Animator anim;

	// Use this for initialization
	void Start () {
        initialPos = transform.position;
        anim = gameObject.GetComponent<Animator>();
        //dissolveTime = 2 * (anim.);
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.K))
        {
            Dissolve();
        }
	}

    public void Dissolve()
    {
        anim.SetBool("Dissolve", true);
        Invoke("Undissolve", 0.1f);
        Invoke("ReturnToInit", dissolveTime/2);
        GameObject tempEffects = Instantiate(dissolveEffects, transform.position, Quaternion.identity) as GameObject;
        tempEffects.transform.LookAt(initialPos);
    }

    void Undissolve()
    {
        //anim.SetBool("Undissolve", true);
        anim.SetBool("Dissolve", false);
    }

    public void ReturnToInit()
    {
        Invoke("resetBeingCarried", dissolveTime / 2);
        //anim.SetBool("Undissolve", false);
        //Interactions.dropObject();
        transform.position = initialPos;
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    void resetBeingCarried()
    {
        beingCarried = false;
    }

}
