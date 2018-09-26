using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalPushable : MonoBehaviour {

    Animator anim;

    bool animPlayed = false;

	// Use this for initialization
	void Start () {
        anim = gameObject.GetComponent<Animator>();
        //anim.StopPlayback();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "PlayerBody")
        {
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Interact")) && !animPlayed)
            {
                animPlayed = true;
                anim.SetBool("playAnim", true);
                //anim.StartPlayback();
            }
        }
        else if (other.gameObject.tag == "Pushable")
        {
            animPlayed = true;
            anim.SetBool("playAnim", true);
        }
    }
}
