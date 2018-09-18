using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class ObjectInteraction : MonoBehaviour {

    public string[] pickupTags;
    public GameObject prompt;
    public Transform holdPos;
    public float pickUpSpeed = 0.1f;

    public bool nextToPickup = false;
    public bool nextToPushable = false;
    public bool holdingPickup = false;
    public bool pushingObject = false;
    public bool lerpingHeldBlock = false;
    public bool droppingPickup = false;

    public const float bufferDist = 0.75f;

    public GameObject pickupObj;
    public GameObject pushObject;

    ThirdPersonUserControl characterControl;

    // Use this for initialization
    void Start () {
        characterControl = GameObject.Find("ThirdPersonController").GetComponent<ThirdPersonUserControl>();
	}
	
	// Update is called once per frame
	void Update () {

        if (pushingObject)
        {
            if (Input.GetKeyDown(KeyCode.E) && !droppingPickup)
            {
                pushingObject = false;
                droppingPickup = true;
                characterControl.isPushing = false;
                pushObject.GetComponent<Rigidbody>().mass = 1000;
            }

        }

        if (Input.GetKeyDown(KeyCode.E) && nextToPushable && !holdingPickup && !pushingObject && !droppingPickup)
        {
            pushingObject = true;
            characterControl.isPushing = true;
            pushObject.GetComponent<Rigidbody>().mass = 5;
        }

        if (Input.GetKeyDown(KeyCode.E) && holdingPickup)
        {
            holdingPickup = false;
            pickupObj.GetComponent<Rigidbody>().useGravity = true;
            pickupObj.GetComponent<Rigidbody>().isKinematic = false;
            droppingPickup = true;
            lerpingHeldBlock = false;
            //Invoke("endDrop", 0.1f);
        }

        if (nextToPickup && !holdingPickup && !droppingPickup && !pushingObject)
        {
            prompt.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                holdingPickup = true;
                pickupObj.GetComponent<Rigidbody>().useGravity = false;
                pickupObj.GetComponent<Rigidbody>().isKinematic = true;
            }
        }

   
        if (holdingPickup)
        {
            prompt.SetActive(false);
            if (Vector3.Distance(holdPos.position, pickupObj.transform.position) > 0.05f && holdingPickup)
            {
                lerpingHeldBlock = true;
            }
            else
            {
                lerpingHeldBlock = false;
            }

        }

        if (lerpingHeldBlock)
        {
            pickupObj.transform.position = Vector3.Lerp(pickupObj.transform.position, holdPos.position, pickUpSpeed);
        }


        droppingPickup = false;

        nextToPickup = false;
        nextToPushable = false;
        //prompt.SetActive(false);
    }

    private void OnTriggerStay(Collider hit)
    {
        if (!holdingPickup)
        {
            bool foundPickUp = false;
            for (int i = 0; i < pickupTags.Length; i++)
            {
                if (hit.tag == pickupTags[i])
                {
                    nextToPickup = true;
                    prompt.SetActive(true);
                    foundPickUp = true;
                    pickupObj = hit.gameObject;
                }
                /*if (foundPickUp)
                {
                    prompt.SetActive(true);
                    nextToPickup = true;
                }
                else
                {
                    prompt.SetActive(false);
                    nextToPickup = false;
                }*/
            }

            if (!foundPickUp && hit.tag == "Pushable")
            {
                nextToPushable = true;
                print("boop");
                pushObject = hit.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider hit)
    {
        if (!holdingPickup)
        {
            for (int i = 0; i < pickupTags.Length; i++)
            {
                if (hit.tag == pickupTags[i])
                {
                    prompt.SetActive(false);
                    nextToPickup = false;
                }
            }
        }
    }

}
