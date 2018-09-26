using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Seeker : MonoBehaviour {

    //public Collider cone;

    public enum seekerState
    {
        Patrol,
        Chasing,
        Destroying,
        Returning
    }

    public enum EyeState
    {
        LockedOn,
        Passive
    }

    EyeState eyeState = EyeState.Passive;

    public GameObject PointHolder;
    public Transform[] patrolPoints;
    int currentPatrolPoint = 0;
    NavMeshAgent agent;
    bool turning = false;

    public seekerState seekState = seekerState.Patrol;
    public Vector3 initPatrolPos;

    public Transform Eye;
    public Transform[] sightPoints;
    public float eyeSwaySpeed = 0.01f;
    public float maxEyeSwayAngle = 30;
    public Color[] lockedOnEyeColours;
    public Material lockedOnEyeMat;
    public Light[] eyeLights;
    Color[] PassiveEyeColours;
    Material passiveEyeMat;
    Renderer rend;
    SightCone cone;
    float eyeRot = 0;

    float stdSpeed;
    float stdAngularSpeed;
    float chaseAngularSpeed;
    float chaseSpeed;
    float turnSpeed;
    public float chaseSpeedMultiplier;
    public float angularSpeedMultiplier = 2f;
    public float turnSpeedMultiplier;
    public float loseSightTime = 1;

    public float DestroyDelay = 1f;

    ObjectInteractions myObjectHandler;
    bool destroyingObject = false;

    [HideInInspector]
    public LiftableObject energyCubeTarget;

	// Use this for initialization
	void Start () {
        agent = gameObject.GetComponent<NavMeshAgent>();
        PointHolder.transform.parent = null;
        cone = Eye.gameObject.GetComponentInChildren<SightCone>();
        cone.sightPoints = sightPoints;
        stdSpeed = agent.speed;
        chaseSpeed = agent.speed * chaseSpeedMultiplier;
        turnSpeed = agent.speed * turnSpeedMultiplier;
        stdAngularSpeed = agent.angularSpeed;
        chaseAngularSpeed = stdAngularSpeed * angularSpeedMultiplier;

        PassiveEyeColours = new Color[eyeLights.Length];

        for (int i = 0; i < eyeLights.Length; i++)
        {
            //print(eyeLights[i].color);
            PassiveEyeColours[i] = eyeLights[i].color;
        }
        rend = Eye.GetComponent<Renderer>();
        //print(rend.material);
        passiveEyeMat = rend.material;

        myObjectHandler = gameObject.GetComponent<ObjectInteractions>();
    }
	
	// Update is called once per frame
	void Update () {
        if (seekState == seekerState.Patrol)
        {
            //here we do our raycasting
            if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 0.3f)
            {
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;

                turning = true;
                agent.speed = turnSpeed;
                Invoke("stopTurning", 1f);
            }
            agent.SetDestination(patrolPoints[currentPatrolPoint].position);

            eyeRot = eyeRot + eyeSwaySpeed;
            if (eyeRot > maxEyeSwayAngle)
            {
                eyeSwaySpeed = -1 * Mathf.Abs(eyeSwaySpeed); //use sine graph
            }
            else if (eyeRot < -maxEyeSwayAngle)
            {
                eyeSwaySpeed = Mathf.Abs(eyeSwaySpeed); //use sine graph
            }
            Eye.localEulerAngles = new Vector3(0, eyeRot, 0);

        }
        else if (seekState == seekerState.Chasing)
        {
            Eye.LookAt(energyCubeTarget.transform);
            //eyeRot = Eye.eulerAngles.y;
            //Eye.localEulerAngles = new Vector3(0, eyeRot, 0);

            if (eyeState == EyeState.Passive)
            {
                eyeState = EyeState.LockedOn;
                for (int i = 0; i < eyeLights.Length; i++)
                {
                    eyeLights[i].color = lockedOnEyeColours[i];
                }
                rend.material = lockedOnEyeMat;
            }

            agent.speed = chaseSpeed;
            agent.angularSpeed = chaseAngularSpeed;

            //if (Vector3.Distance(energyCubeTarget.transform.position, transform.position) < 15f)   //Change to hide out of sight
            //{
                if (Vector3.Distance(energyCubeTarget.transform.position, transform.position) < 2.85f)
                {
                    seekState = seekerState.Destroying;
                    agent.SetDestination(transform.position);
                }
                else
                {
                    agent.SetDestination(energyCubeTarget.transform.position);
                }
            //}
            //else
            //{
            //    seekState = seekerState.Destroying;
            //}

        }
        else if (seekState == seekerState.Destroying)
        {
            if (!destroyingObject)
            {
                DestroyDelay = energyCubeTarget.dissolveTime / 2;
                Invoke("stopDestroying", DestroyDelay);
                if (energyCubeTarget.Interactions != null)
                {
                    energyCubeTarget.Interactions.dropObject();
                }
                myObjectHandler.objectToLift = energyCubeTarget.gameObject;
                myObjectHandler.LiftObject();
                destroyingObject = true;
                energyCubeTarget.Dissolve();
            }

        }
        else if (seekState == seekerState.Returning)
        {
            agent.speed = stdSpeed;
            agent.angularSpeed = stdAngularSpeed;
            //Here we go back on patrol
            if (eyeState == EyeState.LockedOn)
            {
                eyeState = EyeState.Passive;
                for (int i = 0; i < eyeLights.Length; i++)
                {
                    eyeLights[i].color = PassiveEyeColours[i];
                }
                rend.material = passiveEyeMat;
            }

            if (Vector3.Distance(transform.position, initPatrolPos) < 0.2f)
            {
                seekState = seekerState.Patrol;
            }
            else
            {
                agent.SetDestination(initPatrolPos);
            }
        }
	}

    void stopTurning()
    {
        agent.speed = stdSpeed;
    }

    void stopDestroying()
    {
        seekState = seekerState.Returning;
        myObjectHandler.dropObject();
        destroyingObject = false;
    }

    
}
