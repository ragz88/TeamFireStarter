using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CleanerEnemy2_0 : MonoBehaviour
{

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
    ObjectInteractions interactionController;
    int turnDirectionInt = 0;
    //public Quaternion initDir;
    public Vector3 initHerbPos;

    // Use this for initialization
    void Start()
    {
        gyroscope.transform.parent = null;
        interactionController = gameObject.GetComponent<ObjectInteractions>();
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

        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, transform.forward + new Vector3(0, -0.5f, 0), out rayHit, 2f, rayMask))
        {
            if (rayHit.collider.gameObject.tag == "Bar" && !isCleaning)
            {
                bar = rayHit.collider.gameObject.GetComponent<LoadingBar>();
                if (bar.currentFillNum > 0.25f)
                {
                    initHerbPos = transform.position;
                    isCleaning = true;
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
                    if ((rayHit.collider.gameObject.tag == "Wall" || rayHit.collider.gameObject.tag == "Pushable") && !isTurning && (Vector3.Angle(transform.forward, moveTransforms[currentPos].position - transform.position) < 1f))
                    {
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
                            isCleaning = true;
                            interactionController.objectToLift = rayHit.collider.gameObject;
                            interactionController.LiftObject();
                            agent.speed = 0.1f;
                            isTurning = true;
                            Invoke("resetSpeed", 0.5f);
                            agent.SetDestination(source.initialPos);
                            holdingSource = true;
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


        if (!isCleaning)
        {
            //agent.SetDestination(movePoint);
            if (Mathf.Abs(agent.destination.x - initHerbPos.x) < 0.01f && Mathf.Abs(agent.destination.z - initHerbPos.z) < 0.01f)   //y value will differ
            {

                if (Vector3.Distance(transform.position, initHerbPos) < 1)
                {
                    agent.SetDestination(moveTransforms[currentPos].position);
                    initHerbPos = new Vector3(0, 25, 0);                           //this is a dummy number as null is not assignable to a V3
                }
            }
            else
            {
                agent.SetDestination(moveTransforms[currentPos].position);
            }
        }
        else
        {
            if (!holdingSource && bar.currentFillNum < 0.25f)
            {
                isCleaning = false;
                bar = null;
                source = null;
                agent.speed = 0.1f;
                isTurning = true;
                agent.SetDestination(initHerbPos);
                Invoke("resetSpeed", 0.5f);
                //agent.SetDestination(movePoint);
            }
            else if (holdingSource == true && Vector3.Distance(transform.position, source.initialPos) < 2f && !isTurning)
            {
                //put source down
                interactionController.objectToLift = source.gameObject;
                agent.SetDestination(initHerbPos);
                interactionController.dropObject();
                interactionController.objectToLift = null;
                holdingSource = false;
                isCleaning = false;
                bar = null;
                source = null;
                agent.speed = 0.1f;
                isTurning = true;
                //print(initHerbPos);
                //print (agent.destination);
                Invoke("resetSpeed", 0.5f);
                //agent.SetDestination(movePoint);
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
