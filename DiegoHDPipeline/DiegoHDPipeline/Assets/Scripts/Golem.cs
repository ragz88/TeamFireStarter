using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Golem: MonoBehaviour {

    //public Collider cone;

    public enum golemState
    {
        Patrol,
        Chasing,
        Lifting,
        Holding,
        Dropping,
        Returning
    }

    public enum EyeState
    {
        LockedOn,
        Passive
    }

    EyeState eyeState = EyeState.Passive;

    bool blockPresent = false;
    bool isActive = false;
    GameObject energySource;
    LiftableObject energySourceLO;
    public Transform energySourceFinPos;
    public float sourceSpeed = 1;

    public GameObject PointHolder;
    public Transform[] patrolPoints;
    int currentPatrolPoint = 0;
    NavMeshAgent agent;
    bool turning = false;

    public golemState golState = golemState.Patrol;
    public Vector3 initPatrolPos;

    public Transform Eye;
    public Transform[] sightPoints;
    //public float eyeSwaySpeed = 0.01f;
    //public float maxEyeSwayAngle = 30;
    public Color[] lockedOnEyeColours;
    public Material lockedOnEyeMat;
    public Light[] eyeLights;
    Color[] PassiveEyeColours;
    Material passiveEyeMat;
    public Image rend;
    SightCone cone;
    float eyeRot = 0;
    Vector3 initEyeRot;

    float stdSpeed;
    float stdAngularSpeed;
    float chaseAngularSpeed;
    float chaseSpeed;
    float turnSpeed;
    public float chaseSpeedMultiplier;
    public float angularSpeedMultiplier = 2f;
    public float turnSpeedMultiplier;
    public float loseSightTime = 1;

    public Transform liftPos;
    public Transform returnPos;

    [HideInInspector]
    public GameObject diegoTarget;
    MoveBehaviour diegoMoveBehav;
    public float liftSpeed = 1;
    public float stoppingDistRetPos = 1;
    [HideInInspector]
    public float timeSinceDrop = 2;


    public GameObject cage;
    public float finalCageScale = 2;
    public float cageSpeed = 2;
    float initCageScale;

    //public float DestroyDelay = 1f;

    //ObjectInteractions myObjectHandler;
    //bool destroyingObject = false;

    AudioSource beeper;
    public AudioClip alarm;
    public AudioClip wallBeep;
    public float alarmVolume = 0.5f;
    float initPitch;
    float initVolume;

    
    float[] initLightIntensity;

    
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
        initEyeRot = Eye.rotation.eulerAngles;

        PassiveEyeColours = new Color[eyeLights.Length];
        
        for (int i = 0; i < eyeLights.Length; i++)
        {
            //print(eyeLights[i].color);
            PassiveEyeColours[i] = eyeLights[i].color;
        }
        //rend = Eye.GetComponentInParent<Renderer>();
        //print(rend.material);
        passiveEyeMat = rend.material;

        //myObjectHandler = gameObject.GetComponent<ObjectInteractions>();

        beeper = gameObject.GetComponent<AudioSource>();
        initPitch = beeper.pitch;
        initVolume = beeper.volume;
        //prevents the return pos in world space from moving around with the golem.
        returnPos.parent = null;

        //ensures the scale of the energy source is correct by scaling the point it's parented to relative to the overal object's scale
        energySourceFinPos.localScale = new Vector3(1/transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);

        initLightIntensity = new float[eyeLights.Length];
        for (int i = 0; i < eyeLights.Length; i++)
        {
            initLightIntensity[i] = eyeLights[i].intensity;
        }

        initCageScale = cage.transform.localScale.x;

    }
	
	// Update is called once per frame
	void Update () {

        
        if (!blockPresent)
        {
            isActive = false;
        }

        if (isActive)
        {

            energySource.transform.position = Vector3.Lerp(energySource.transform.position, energySourceFinPos.position,
                    sourceSpeed * Time.deltaTime);
            energySource.transform.rotation = Quaternion.Slerp(energySource.transform.rotation, energySourceFinPos.rotation, 20 * Time.deltaTime);
            if (golState == golemState.Patrol)
            {
                if (Vector3.Distance(cage.transform.position, Eye.transform.position) < 0.2f)
                {
                    cage.SetActive(false);
                }
                else
                {
                    cage.transform.position = Vector3.Lerp(cage.transform.position, Eye.transform.position, cageSpeed * Time.deltaTime * 5);
                }

                timeSinceDrop = timeSinceDrop + Time.deltaTime;
                if (beeper.isPlaying && beeper.clip == alarm)
                {
                    if (beeper.volume > 0.05f)
                    {
                        beeper.volume -= 0.03f;
                    }
                    else
                    {
                        beeper.Stop();
                        beeper.loop = false;
                    }
                }

                if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 0.3f)
                {
                    currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;

                    turning = true;
                    agent.speed = turnSpeed;
                    Invoke("stopTurning", 3f);
                    if (!beeper.isPlaying || (beeper.clip == alarm && beeper.isPlaying))
                    {
                        beeper.clip = wallBeep;
                        beeper.pitch = initPitch;
                        beeper.volume = initVolume;
                        beeper.Play();
                    }
                }
                agent.SetDestination(patrolPoints[currentPatrolPoint].position);

                /*eyeRot = eyeRot + eyeSwaySpeed;
                if (eyeRot > maxEyeSwayAngle)
                {
                    eyeSwaySpeed = -1 * Mathf.Abs(eyeSwaySpeed); //use sine graph
                }
                else if (eyeRot < -maxEyeSwayAngle)
                {
                    eyeSwaySpeed = Mathf.Abs(eyeSwaySpeed); //use sine graph
                }*/
                //Eye.localEulerAngles = new Vector3(initEyeRot.x, eyeRot, initEyeRot.z);

            }
            else if (golState == golemState.Chasing)
            {
                if (Vector3.Distance(cage.transform.position, Eye.transform.position) < 0.2f)
                {
                    cage.SetActive(false);
                }
                else
                {
                    cage.transform.position = Vector3.Lerp(cage.transform.position, Eye.transform.position, cageSpeed * Time.deltaTime * 5);
                }

                if (!beeper.isPlaying)
                {
                    beeper.clip = alarm;
                    beeper.pitch = 1;
                    beeper.volume = alarmVolume;
                    beeper.Play();
                    beeper.loop = true;
                }
                //Eye.LookAt(diegoTarget.transform);
                // = Eye.eulerAngles.y;
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
                if (Vector3.Distance(diegoTarget.transform.position, transform.position) < 2.85f)
                {
                    golState = golemState.Lifting;
                    diegoMoveBehav = diegoTarget.GetComponent<MoveBehaviour>();
                    diegoMoveBehav.lockMovement = true;
                    diegoTarget.GetComponent<Animator>().enabled = false ;
                    agent.SetDestination(transform.position);
                    diegoTarget.GetComponent<Rigidbody>().isKinematic = true;
                }
                else
                {
                    agent.SetDestination(diegoTarget.transform.position);
                }
                //}
                //else
                //{
                //    seekState = seekerState.Destroying;
                //}

            }
            else if (golState == golemState.Lifting)
            {
                //show our cage, move it and expand it
                cage.SetActive(true);
                if (cage.transform.localScale.x < finalCageScale)
                {
                    cage.transform.localScale = cage.transform.localScale * (1 + (cageSpeed*2*Time.deltaTime));
                }
                cage.transform.position = Vector3.Lerp(cage.transform.position, diegoTarget.transform.position + new Vector3(0,1,0), cageSpeed * Time.deltaTime);


                if (Vector3.Distance(diegoTarget.transform.position, liftPos.position) > 0.3f)
                {
                    diegoTarget.transform.position = Vector3.Lerp(diegoTarget.transform.position, liftPos.position, liftSpeed * Time.deltaTime);
                }
                else
                {
                    golState = golemState.Holding;
                }

            }
            else if (golState == golemState.Holding)
            {
                cage.transform.position = Vector3.Lerp(cage.transform.position, diegoTarget.transform.position + new Vector3(0, 1, 0), cageSpeed * Time.deltaTime * 5);

                agent.SetDestination(returnPos.position);
                diegoTarget.transform.position = Vector3.Lerp(diegoTarget.transform.position, liftPos.position, 20f * Time.deltaTime);
                if (Vector3.Distance(returnPos.position, transform.position) < stoppingDistRetPos)
                {
                    golState = golemState.Dropping;
                    agent.SetDestination(transform.position);
                }
            }
            else if (golState == golemState.Dropping)
            {
                cage.transform.position = Vector3.Lerp(cage.transform.position, diegoTarget.transform.position + new Vector3(0, 1, 0), cageSpeed * Time.deltaTime * 5);

                agent.SetDestination(transform.position);
                diegoTarget.transform.position = Vector3.Lerp(diegoTarget.transform.position, returnPos.position, liftSpeed * Time.deltaTime);
                if (Vector3.Distance(returnPos.position, diegoTarget.transform.position) < 0.5f)
                {
                    diegoTarget.GetComponent<Rigidbody>().isKinematic = false;
                    timeSinceDrop = 0;
                    golState = golemState.Returning;
                    diegoMoveBehav.lockMovement = false;
                    diegoTarget.GetComponent<Animator>().enabled = true;

                }
            }
            else if (golState == golemState.Returning)
            {
                if (cage.transform.localScale.x > initCageScale)
                {
                    cage.transform.localScale = cage.transform.localScale * (1 - (cageSpeed * 2 * Time.deltaTime));
                }
                if (Vector3.Distance(cage.transform.position, Eye.transform.position) < 0.2f && cage.transform.localScale.x <= initCageScale)
                {
                    cage.SetActive(false);
                }
                else
                {
                    cage.transform.position = Vector3.Lerp(cage.transform.position, Eye.transform.position, cageSpeed * Time.deltaTime * 5);
                }

                timeSinceDrop = timeSinceDrop + Time.deltaTime;
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

                if (beeper.isPlaying)
                {
                    if (beeper.clip == alarm && beeper.volume > 0.05f)
                    {
                        beeper.volume -= 0.03f;
                    }
                    else
                    {
                        beeper.Stop();
                        beeper.loop = false;
                    }
                }

                if (Vector3.Distance(transform.position, initPatrolPos) < 0.2f)
                {
                    golState = golemState.Patrol;
                }
                else
                {
                    agent.SetDestination(initPatrolPos);
                }
            }

        }
        else
        {
            if (beeper.isPlaying && beeper.clip == alarm)
            {
                if (beeper.volume > 0.05f)
                {
                    beeper.volume -= 0.03f;
                }
                else
                {
                    beeper.Stop();
                    beeper.loop = false;
                }
            }

            if (eyeState == EyeState.LockedOn)
            {
                eyeState = EyeState.Passive;
                for (int i = 0; i < eyeLights.Length; i++)
                {
                    eyeLights[i].color = PassiveEyeColours[i];
                }
                rend.material = passiveEyeMat;
            }

            golState = golemState.Patrol;

            agent.SetDestination(transform.position);
            if (blockPresent)
            {
                energySource.transform.position = Vector3.Lerp(energySource.transform.position, energySourceFinPos.position,
                    sourceSpeed * Time.deltaTime);
                energySource.transform.rotation = Quaternion.Slerp(energySource.transform.rotation, energySourceFinPos.rotation, 20 * Time.deltaTime);
                if (Vector3.Distance(energySource.transform.position, energySourceFinPos.position) < 0.03f)
                {
                    energySource.transform.parent = energySourceFinPos;
                    isActive = true;
                }

                for (int i = 0; i < eyeLights.Length; i++)
                {
                    if (eyeLights[i].intensity < initLightIntensity[i])
                    {
                        eyeLights[i].intensity += Time.deltaTime*30;
                    }
                }
                if (rend.fillAmount < 1)
                {
                    rend.fillAmount += Time.deltaTime;
                }
            }
            else
            {
                for (int i = 0; i < eyeLights.Length; i++)
                {
                    if (eyeLights[i].intensity > 0)
                    {
                        eyeLights[i].intensity -= Time.deltaTime*30;
                    }
                }
                if (rend.fillAmount > 0)
                {
                    rend.fillAmount -= Time.deltaTime;
                }
            }
        }
	}

    void stopTurning()
    {
        agent.speed = stdSpeed;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "EnergySource" && blockPresent == false && !isActive)
        {
            energySourceLO = other.gameObject.GetComponent<LiftableObject>();
            if (energySourceLO.beingCarried == false)
            {
                energySource = other.gameObject;
                blockPresent = true;
                energySource.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "EnergySource")
        {
            if (energySource != null)
            {
                energySource.transform.parent = null;
                energySource.GetComponent<Rigidbody>().isKinematic = false;
                energySource = null;
            }
            
            blockPresent = false;
        }
    }

    /*void stopDestroying()
    {
        golState = golemState.Returning;
        myObjectHandler.dropObject();
        destroyingObject = false;
    }*/


}
