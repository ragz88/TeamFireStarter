using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CleanerEnemy : MonoBehaviour {

    public Transform[] moveTransforms;
    public float standardSpeed;
    public float cleaningSpeed;
    public LayerMask rayMask;

    NavMeshAgent agent;
    bool isCleaning = false;
    bool isTurning = false;
    public bool holdingSource = false;
    Vector3[] movePoints;
    int currentPos = 0;
    public LoadingBar bar;
    LiftableObject source;
    ObjectInteractions interactionController;

    // Use this for initialization
    void Start () {
        interactionController = gameObject.GetComponent<ObjectInteractions>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        standardSpeed = agent.speed;
        cleaningSpeed = agent.speed * 1.2f;

        movePoints = new Vector3[moveTransforms.Length];
		for (int i = 0; i < moveTransforms.Length; i++)
        {
            movePoints[i] = moveTransforms[i].position;
        }
	}
	
	// Update is called once per frame
	void Update () {

        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, transform.forward + new Vector3(0, -0.5f, 0), out rayHit, 2f, rayMask))
        {
            if (rayHit.collider.gameObject.tag == "Bar" && !isCleaning)
            {
                bar = rayHit.collider.gameObject.GetComponent<LoadingBar>();
                if (bar.currentFillNum > 0.25f)
                {
                    isCleaning = true;
                    agent.speed = 0.1f;
                    isTurning = true;
                    Invoke("resetSpeed", 0.75f);
                    agent.SetDestination(bar.initialSource.position);
                }
            }
            else
            {
                if (rayHit.collider.gameObject.tag == "EnergySource" && isCleaning)
                {
                    //pick up block here
                    interactionController.objectToLift = rayHit.collider.gameObject;
                    interactionController.LiftObject();
                    source = rayHit.collider.gameObject.GetComponent<LiftableObject>();
                    agent.speed = 0.1f;
                    isTurning = true;
                    Invoke("resetSpeed", 0.5f);
                    agent.SetDestination(source.initialPos);
                    holdingSource = true;
                }
            }
        }
        Debug.DrawRay(transform.position, (transform.forward + new Vector3(0, -0.5f, 0)).normalized * 2f, Color.cyan);


        if (!isCleaning)
        {
            if (Vector3.Distance(transform.position, movePoints[currentPos]) < 0.1f && !isTurning)
            {
                currentPos = (currentPos + 1)%movePoints.Length;
                agent.speed = 0.1f;
                isTurning = true;
                Invoke("resetSpeed", 0.5f);
            }
            agent.SetDestination(movePoints[currentPos]);
        }
        else
        {
            if (bar.currentFillNum < 0.25f && !holdingSource)
            {
                isCleaning = false;
                bar = null;
                source = null;
                agent.speed = 0.1f;
                isTurning = true;
                Invoke("resetSpeed", 0.5f);
                agent.SetDestination(movePoints[currentPos]);
            }
            else if (holdingSource == true && Vector3.Distance(transform.position, source.initialPos) < 2f && !isTurning)
            {
                //put source down
                interactionController.objectToLift = source.gameObject;
                interactionController.dropObject();
                interactionController.objectToLift = null;
                holdingSource = false;
                isCleaning = false;
                bar = null;
                source = null;
                agent.speed = 0.1f;
                isTurning = true;
                Invoke("resetSpeed", 0.5f);
                agent.SetDestination(movePoints[currentPos]);
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
