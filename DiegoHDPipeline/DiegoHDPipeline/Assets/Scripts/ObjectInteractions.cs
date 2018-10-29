using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class ObjectInteractions : MonoBehaviour {

    //public string[] pickupTags;
    public bool enemy = false;
    public GameObject prompt;
    public Transform liftPos;
    public Transform pushPos;
    public float liftSpeed = 0.1f;
    float currentLiftSpeed = 0.1f;

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

    public MoveBehaviour characterControl;
    //ThirdPersonCharacter character;

    //Text promptText;

    PushableObject pushBoxController;
    Transform pushPoint;
    //bool lockMovement = false;

    // Use this for initialization
    void Start () {
        //promptText = prompt.GetComponent<Text>();
        characterControl = gameObject.GetComponent<MoveBehaviour>();
        //character = gameObject.GetComponentInParent<ThirdPersonCharacter>();
    }
	
	// Update is called once per frame
	void Update () {

        //PushableObject pushScript;
        LiftableObject liftScript;

        RaycastHit rayHit;
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward + new Vector3(0, -0.5f, 0), out rayHit, 0.75f, rayMask))
        {
            pushBoxController = rayHit.collider.gameObject.GetComponent<PushableObject>();
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

                if (pushBoxController != null)
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


        if (objectToLift != null)
        {
            if (nextToPickup && !holdingPickup && !pushingObject && !objectToLift.GetComponent<LiftableObject>().beingCarried)
            {
                prompt.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Interact"))
                {
                    //lerpingPickup = true;

                    LiftObject();
                }
            }
        }

        if (nextToPushable && !holdingPickup && !pushingObject)
        {
            prompt.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Interact"))
            {
                transitioning = true;
                pushingObject = true;
                characterControl.lockMovement = true;
                characterControl.pushing = true;
                //character.pushingObject = true;
                objectToPush.GetComponent<Rigidbody>().mass = 200f;
                //gameObject.GetComponent<MoveBehaviour>().pushing = true;

                //objectToPush.GetComponent<Rigidbody>().isKinematic = true;
                //Physics.IgnoreCollision(objectToPush.GetComponent<Collider>(), gameObject.transform.parent.GetComponent<Collider>());
                //pushPos.position = objectToPush.transform.position;
                pushBoxController.beingPushed = true;
                float tempDist = Vector3.Distance(transform.position, pushBoxController.pushPoints[0].position);
                int nearestPoint = 0;
                for (int i = 0; i < pushBoxController.pushPoints.Length; i++)
                {
                    if (Vector3.Distance(transform.position, pushBoxController.pushPoints[i].position) < tempDist)
                    {
                        tempDist = Vector3.Distance(transform.position, pushBoxController.pushPoints[i].position);
                        nearestPoint = i;
                    }
                }
                pushPoint = pushBoxController.pushPoints[nearestPoint];
            }
        }
        

        if (!nextToPickup && !nextToPushable)
        {
            prompt.SetActive(false);
        }

        if (holdingPickup && !transitioning)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Interact"))
            {
                //lerpingPickup = false; 
                dropObject();
                
            }
        }

        if (pushingObject && !transitioning)
        {
            //print(pushPos.position);

            if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Interact"))
            {
                pushingObject = false;
                //objectToPush.transform.parent = null;
                characterControl.pushing = false;
                //character.pushingObject = false;
                characterControl.lockMovement = false;

                objectToPush.GetComponent<Rigidbody>().mass = 100000;
                //gameObject.GetComponent<MoveBehaviour>().pushing = false;

                //Physics.IgnoreCollision(objectToPush.GetComponent<Collider>(), gameObject.transform.parent.GetComponent<Collider>(), false);
            }
            else
            {
                if (Vector3.Distance(transform.position, new Vector3(pushPoint.position.x, transform.position.y, pushPoint.position.z)) > 0.03f && characterControl.lockMovement)
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(pushPoint.position.x, transform.position.y, pushPoint.position.z), 0.5f);
                    Physics.IgnoreCollision(objectToPush.GetComponent<Collider>(), gameObject.transform.GetComponent<Collider>());
                    //play moving anim here
                }
                else
                {
                    Physics.IgnoreCollision(objectToPush.GetComponent<Collider>(), gameObject.transform.GetComponent<Collider>(), false);
                    if (characterControl.lockMovement)
                    {
                        characterControl.lockMovement = false;
                        transform.LookAt(new Vector3(objectToPush.transform.position.x, transform.position.y, objectToPush.transform.position.z));
                        //objectToPush.transform.parent = transform.parent;
                    }
                    else
                    {
                        transform.LookAt(new Vector3(objectToPush.transform.position.x, transform.position.y, objectToPush.transform.position.z));
                        if (Vector3.Distance(pushPos.position, new Vector3(pushPoint.position.x, pushPos.position.y, pushPoint.position.z)) > 1.5f)
                        {
                            pushingObject = false;
                            characterControl.pushing = false;
                            characterControl.lockMovement = false;
                            //print(Vector3.Distance(pushPos.position, new Vector3(pushPoint.position.x, pushPos.position.y, pushPoint.position.z)));
                            

                            objectToPush.GetComponent<Rigidbody>().mass = 100000;
                        }
                    }
                }
            }
        }

        if (holdingPickup && lerpingPickup)
        {
            objectToLift.transform.LookAt(liftPos.position + transform.forward);
            objectToLift.transform.position = Vector3.Lerp(objectToLift.transform.position, liftPos.position, liftSpeed);
        }

        /*if (pushingObject)
        {
            //objectToPush.transform.position = Vector3.Lerp(objectToPush.transform.position, pushPos.position, 5);
        }*/

        transitioning = false;
    }
    public void LiftObject()
    {
        Invoke("finishLift", 1.3f);
        holdingPickup = true;
        objectToLift.GetComponent<Rigidbody>().useGravity = false;
        objectToLift.GetComponent<Rigidbody>().isKinematic = true;
        
        transitioning = true;
        objectToLift.GetComponent<LiftableObject>().beingCarried = true;
        objectToLift.GetComponent<LiftableObject>().Interactions = this;
        if (!enemy)
        {
            Physics.IgnoreCollision(objectToLift.GetComponent<Collider>(), gameObject.transform.GetComponent<Collider>());
        }
        if (characterControl != null)
        {
            characterControl.lifting = true;
        }
    }

    void finishLift()
    {
        //holdingPickup = true;
        lerpingPickup = true;
        //objectToLift.GetComponent<Rigidbody>().useGravity = false;
        //objectToLift.GetComponent<Rigidbody>().isKinematic = true;
        currentLiftSpeed = 4f;
        //transitioning = true;
        //objectToLift.GetComponent<LiftableObject>().beingCarried = true;
        //objectToLift.GetComponent<LiftableObject>().Interactions = this;
        //Physics.IgnoreCollision(objectToLift.GetComponent<Collider>(), gameObject.transform.GetComponent<Collider>());
        //if (characterControl != null)
        //{
        //    characterControl.lifting = true;
        //}
    }

    public void dropObject()
    {
        holdingPickup = false;
        lerpingPickup = false;
        currentLiftSpeed = liftSpeed;
        objectToLift.GetComponent<Rigidbody>().isKinematic = false;
        objectToLift.GetComponent<Rigidbody>().useGravity = true;
        objectToLift.GetComponent<LiftableObject>().beingCarried = false;
        objectToLift.GetComponent<LiftableObject>().Interactions = null;
        if (!enemy)
        {
            Physics.IgnoreCollision(objectToLift.GetComponent<Collider>(), gameObject.transform.GetComponent<Collider>(), false);
        }
            if (characterControl != null)
        {
            characterControl.lifting = false;
        }
        CancelInvoke("finishLift");
    }
}
