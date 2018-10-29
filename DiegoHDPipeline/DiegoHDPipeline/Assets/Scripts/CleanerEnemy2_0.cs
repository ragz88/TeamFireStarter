using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CleanerEnemy2_0 : MonoBehaviour
{

    public enum botState
    {
        Patrol,
        Cleaning,
        Lifting,
        Holding,
        Dropping,
        Returning
    }

    public botState cleanBotState = botState.Patrol;

    public Transform[] moveTransforms;
    public float standardSpeed;
    public float cleaningSpeed;
    public LayerMask rayMask;
    public LayerMask wallRayMask;
    public bool preferLeft = true;

    public Vector3 currentDest;

    public GameObject gyroscope;

    NavMeshAgent agent;
    bool isCleaning = false;
    bool isTurning = false;
    public bool holdingSource = false;
    //public Vector3[] movePoints;
    int currentPos = 0;
    public LoadingBar bar;
    LiftableObject source;
    //ObjectInteractions interactionController;
    public Transform liftPoint, dropPoint;
    public float liftSpeed = 1;
    GameObject objectToLift;
    int turnDirectionInt = 0;
    //public Quaternion initDir;
    public Vector3 initHerbPos;

    AudioSource beeper;
    public AudioClip wallBeep;
    public AudioClip barBeep;

    // Use this for initialization
    void Start()
    {
        gyroscope.transform.parent = null;
        //interactionController = gameObject.GetComponent<ObjectInteractions>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        standardSpeed = agent.speed;
        cleaningSpeed = agent.speed * 1.2f;
        //initDir = new Quaternion(0,0,95,0);
        if (preferLeft)
        {
            turnDirectionInt = -1;
        }
        else
        {
            turnDirectionInt = 1;
        }

        beeper = gameObject.GetComponent<AudioSource>();

        initHerbPos = new Vector3(0, 25, 0);

        /*movePoints = new Vector3[moveTransforms.Length];
		for (int i = 0; i < moveTransforms.Length; i++)
        {
            movePoints[i] = moveTransforms[i].position;
        }*/
    }

    // Update is called once per frame
    void Update()
    {

        currentDest = agent.destination;

        gyroscope.transform.position = transform.position;


        if (cleanBotState == botState.Patrol)
        {
            agent.SetDestination(moveTransforms[currentPos].position);

            RaycastHit rayHit;
            if (Physics.Raycast(transform.position, transform.forward + new Vector3(0, -0.5f, 0), out rayHit, 2f, rayMask))
            {
                if (rayHit.collider.gameObject.tag == "Bar")
                {
                    bar = rayHit.collider.gameObject.GetComponent<LoadingBar>();
                    if (bar.currentFillNum > 0.25f)
                    {
                        beeper.clip = barBeep;
                        beeper.Play();
                        initHerbPos = transform.position;
                        //isCleaning = true;
                        cleanBotState = botState.Cleaning;
                        agent.speed = 0.1f;
                        isTurning = true;
                        Invoke("resetSpeed", 0.5f);
                        agent.SetDestination(bar.initialSource.position);
                    }
                }
                else
                {
                    if (Physics.Raycast(transform.position, transform.forward, out rayHit, 2f, rayMask))
                    {
                        if ((rayHit.collider.gameObject.tag == "Wall" || rayHit.collider.gameObject.tag == "Pushable") && !isTurning && (Vector3.Angle(transform.forward, moveTransforms[currentPos].position - transform.position) < 5f))
                        {
                            beeper.clip = wallBeep;
                            beeper.Play();
                            isTurning = true;
                            currentPos = (currentPos + turnDirectionInt) % 4;
                            if (currentPos == -1)
                            {
                                currentPos = moveTransforms.Length - 1;
                            }
                            agent.SetDestination(moveTransforms[currentPos].position);
                            //initDir = transform.rotation;
                            agent.speed = 0.1f;
                            Invoke("resetSpeed", 1f);
                        }
                        else
                        if (rayHit.collider.gameObject.tag == "EnergySource" /*&& isCleaning*/)
                        {
                            //pick up block here
                            source = rayHit.collider.gameObject.GetComponent<LiftableObject>();
                            if (Vector3.Distance(source.initialPos, source.transform.position) > 3)
                            {
                                cleanBotState = botState.Lifting;
                                //isCleaning = true;
                                //interactionController.objectToLift = rayHit.collider.gameObject;
                                //interactionController.LiftObject();
                                objectToLift = rayHit.collider.gameObject;

                                agent.speed = 0.1f;
                                isTurning = true;
                                Invoke("resetSpeed", 0.5f);
                                //agent.SetDestination(source.initialPos);
                                //holdingSource = true;                             ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                if (initHerbPos == new Vector3(0, 25, 0))
                                {
                                    initHerbPos = transform.position;
                                }
                            }
                        }
                    }
                }
            }
            Debug.DrawRay(transform.position, (transform.forward + new Vector3(0, -0.5f, 0)).normalized * 2f, Color.cyan);

        }
        else if (cleanBotState == botState.Cleaning)
        {
            if (bar.currentFillNum < 0.25f)
            {
                cleanBotState = botState.Returning;
                bar = null;
                source = null;
                agent.speed = 0.1f;
                isTurning = true;
                Invoke("resetSpeed", 0.5f);
                //agent.SetDestination(movePoint);
            }
            else
            {
                RaycastHit rayHit;
                if (Physics.Raycast(transform.position, transform.forward, out rayHit, 2f, rayMask))
                {
                    if (rayHit.collider.gameObject.tag == "EnergySource")
                    {
                        //pick up block here
                        source = rayHit.collider.gameObject.GetComponent<LiftableObject>();
                        if (Vector3.Distance(source.initialPos, source.transform.position) > 3)
                        {
                            cleanBotState = botState.Lifting;
                            //isCleaning = true;
                            //interactionController.objectToLift = rayHit.collider.gameObject;
                            //interactionController.LiftObject();
                            objectToLift = rayHit.collider.gameObject;

                            agent.speed = 0.1f;
                            isTurning = true;
                            Invoke("resetSpeed", 0.5f);
                            //agent.SetDestination(source.initialPos);
                            //holdingSource = true;                             ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            if (initHerbPos == new Vector3(0, 25, 0))
                            {
                                initHerbPos = transform.position;
                            }
                        }
                    }
                }
            }
        }
        else if (cleanBotState == botState.Lifting)
        {
            agent.SetDestination(transform.position);
            objectToLift = source.gameObject;
            objectToLift.GetComponent<Rigidbody>().isKinematic = true;
            source.beingCarried = true;
                if (Vector3.Distance(objectToLift.transform.position, liftPoint.position) > 0.4f)
                {
                    objectToLift.transform.position = Vector3.Lerp(objectToLift.transform.position, liftPoint.position, liftSpeed * Time.deltaTime);
                }
                else
                {
                    cleanBotState = botState.Holding;
                }
        }
        else if (cleanBotState == botState.Holding)
        {
            objectToLift = source.gameObject;
            objectToLift.transform.position = Vector3.Lerp(objectToLift.transform.position, liftPoint.position, liftSpeed * 10 * Time.deltaTime);
            agent.SetDestination(source.initialPos);

            if (Vector3.Distance(transform.position, source.initialPos) < 1.25f && !isTurning)
            {
                //put source down
                cleanBotState = botState.Dropping;
                //agent.SetDestination(initHerbPos);
                //interactionController.dropObject();
                //interactionController.objectToLift = null;
                //holdingSource = false;
                //isCleaning = false;
                //bar = null;
                //source = null;
                //agent.speed = 0.1f;
                //isTurning = true;
                //print(initHerbPos);
                //print (agent.destination);
                //Invoke("resetSpeed", 0.5f);
                //agent.SetDestination(movePoint);
            }
        }
        else if (cleanBotState == botState.Dropping)
        {
            agent.SetDestination(transform.position);
            if (objectToLift != null)
            {
                if (Vector3.Distance(objectToLift.transform.position, dropPoint.position) > 0.4f)
                {
                    objectToLift.transform.position = Vector3.Lerp(objectToLift.transform.position, dropPoint.position, liftSpeed * Time.deltaTime);
                }
                else
                {
                    objectToLift.GetComponent<Rigidbody>().isKinematic = false;
                    source.beingCarried = false;

                    objectToLift = null;
                    bar = null;
                    source = null;
                    agent.speed = 0.1f;
                    isTurning = true;
                    Invoke("resetSpeed", 0.5f);
                    
                    cleanBotState = botState.Returning;
                }
            }
        }
        else if (cleanBotState == botState.Returning)
        {
            
            if (Mathf.Abs(agent.destination.x - initHerbPos.x) < 0.01f && Mathf.Abs(agent.destination.z - initHerbPos.z) < 0.01f)   //y value will differ
            {

                if (Vector3.Distance(transform.position, initHerbPos) < 1)
                {
                    agent.SetDestination(moveTransforms[currentPos].position);
                    initHerbPos = new Vector3(0, 25, 0);                           //this is a dummy number as null is not assignable to a V3
                    cleanBotState = botState.Patrol;
                }
            }
            else
            {
                //agent.SetDestination(moveTransforms[currentPos].position);
                agent.SetDestination(initHerbPos);
            }
        }

        

        
    }

    void resetSpeed()
    {
        if (isCleaning)
        {
            agent.speed = cleaningSpeed;
        }
        else
        {
            agent.speed = standardSpeed;
        }
        Invoke("resetIsTurning", 0.3f);
    }

    void resetIsTurning()
    {
        isTurning = false;
    }
}
