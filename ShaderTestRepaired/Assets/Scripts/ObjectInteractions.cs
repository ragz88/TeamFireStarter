using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class ObjectInteractions : MonoBehaviour {

    //public string[] pickupTags;
    public GameObject prompt;
    public Transform liftPos;
    public Transform pushPos;
    public float liftSpeed = 0.1f;

    bool nextToPickup = false;
    bool nextToPushable = false;
    public bool holdingPickup = false;
    public bool pushingObject = false;


    public bool lerpingPickup = false;
    //public bool droppingPickup = false;
    bool transitioning = false;  //true on the first frame when e is pressed

    public const float bufferDist = 0.75f;

    public GameObject objectToLift;
    public GameObject objectToPush;

    public LayerMask rayMask;

    ThirdPersonUserControl characterControl;

    Text promptText;

    // Use this for initialization
    void Start () {
        promptText = prompt.GetComponent<Text>();
        characterControl = gameObject.GetComponentInParent<ThirdPersonUserControl>();
	}
	
	// Update is called once per frame
	void Update () {

        PushableObject pushScript;
        LiftableObject liftScript;

        RaycastHit rayHit;
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward + new Vector3(0, -0.5f, 0), out rayHit, 0.75f, rayMask))
        {
            pushScript = rayHit.collider.gameObject.GetComponent<PushableObject>();
            liftScript = rayHit.collider.gameObject.GetComponent<LiftableObject>();

            if (liftScript != null)
            {
                objectToLift = rayHit.collider.gameObject;
                nextToPickup = true;
            }
            else
            {
                if (!holdingPickup)
                {
                    objectToLift = null;
                }

                if (pushScript != null)
                {
                    objectToPush = rayHit.collider.gameObject;
                    nextToPushable = true;
                }
                else
                {
                    if (!pushingObject)
                    {
                        objectToPush = null;
                    }
                }
            }
        }
        else
        {
            if (!holdingPickup)
            {
                objectToLift = null;
            }
            nextToPickup = false;
            if (!pushingObject)
            {
                objectToPush = null;
            }
            nextToPushable = false;
        }
        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), (transform.forward + new Vector3(0, -0.5f, 0)).normalized*0.75f, Color.magenta);
        

        if (nextToPickup && !holdingPickup && !pushingObject)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //lerpingPickup = true;
                holdingPickup = true;
                objectToLift.GetComponent<Rigidbody>().useGravity = false;
                objectToLift.GetComponent<Rigidbody>().isKinematic = true;
                transitioning = true;
            }
        }

        if (nextToPushable && !holdingPickup && !pushingObject)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                transitioning = true;
                pushingObject = true;
                characterControl.isPushing = true;
                //objectToPush.GetComponent<Rigidbody>().mass = 5;
                //objectToPush.GetComponent<Rigidbody>().isKinematic = true;
                Physics.IgnoreCollision(objectToPush.GetComponent<Collider>(), gameObject.transform.parent.GetComponent<Collider>());
                pushPos.position = objectToPush.transform.position;
            }
        }

        if (holdingPickup && !transitioning)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //lerpingPickup = false; 
                holdingPickup = false;
                objectToLift.GetComponent<Rigidbody>().isKinematic = false;
                objectToLift.GetComponent<Rigidbody>().useGravity = true;
            }
        }

        if (pushingObject && !transitioning)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                pushingObject = false;
                characterControl.isPushing = false ;
                //objectToPush.GetComponent<Rigidbody>().mass = 1000;
                Physics.IgnoreCollision(objectToPush.GetComponent<Collider>(), gameObject.transform.parent.GetComponent<Collider>(), false);
            }
        }

        if (holdingPickup)
        {
            objectToLift.transform.position = Vector3.Lerp(objectToLift.transform.position, liftPos.position, liftSpeed);
        }

        if (pushingObject)
        {
            objectToPush.transform.position = Vector3.Lerp(objectToPush.transform.position, pushPos.position, 5);
        }

        transitioning = false;
    }
}
