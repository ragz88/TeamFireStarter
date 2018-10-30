using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalPushable : MonoBehaviour {

    Animator anim;

    [HideInInspector]
    public bool animPlayed = false;
    public GameObject[] objectsToActivate;

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
                PlayAnim();
            }
        }
        else if (other.gameObject.tag == "Pushable")
        {
            PlayAnim();
        }
    }

    public void PlayAnim()
    {
        if (!animPlayed)
        {
            animPlayed = true;
            anim.SetBool("playAnim", true);
            for (int i = 0; i < objectsToActivate.Length; i++)
            {
                objectsToActivate[i].SetActive(true);
            }
            Destroy(this);
        }
    }
}
