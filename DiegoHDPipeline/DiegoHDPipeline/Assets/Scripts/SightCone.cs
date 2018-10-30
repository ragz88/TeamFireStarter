using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightCone : MonoBehaviour {

    [HideInInspector]
    public Transform[] sightPoints;

    public Seeker seeker;
    public Golem golem;
    public LayerMask sightMask;
    float loseSightTime;

    float noLineOfSightTime = 0;

    Transform Eye;
    ObjectInteractions objInteraction;

	// Use this for initialization
	void Start () {
        Eye = transform.parent;
        objInteraction = GameObject.Find("Diego").GetComponent<ObjectInteractions>();
        if (seeker != null)
        {
            loseSightTime = seeker.loseSightTime;
        }
        else
        {
            loseSightTime = golem.loseSightTime;
        }
    }
	
	// Update is called once per frame
	void Update () {
        bool canSeePlayer = false;
		if ((seeker != null && seeker.seekState == Seeker.seekerState.Chasing) || (golem != null && golem.golState == Golem.golemState.Chasing))
        {
            for (int i = 0; i < sightPoints.Length; i++)
            {
                RaycastHit rayHit;
                if (Physics.Raycast(Eye.position, sightPoints[i].position - Eye.position, out rayHit, 12f, sightMask))
                {
                    //print(rayHit.collider.gameObject.name);
                    if (rayHit.collider.gameObject.tag == "PlayerBody")
                    {
                        canSeePlayer = true;
                        break;
                    }
                }
            }

            if (canSeePlayer)
            {
                noLineOfSightTime = 0;
            }
            else
            {
                noLineOfSightTime += Time.deltaTime;
                if (noLineOfSightTime > loseSightTime)
                {
                    if (seeker != null)
                    {
                        seeker.seekState = Seeker.seekerState.Returning;
                    }
                    else
                    {
                        golem.golState = Golem.golemState.Returning;
                    }
                }
            }
        }
	}

    private void OnTriggerStay(Collider other)
    {
        if ((seeker != null && (seeker.seekState == Seeker.seekerState.Patrol || seeker.seekState == Seeker.seekerState.Returning) && other.gameObject.tag == "PlayerBody") ||
           (golem != null && ((golem.golState == Golem.golemState.Patrol || golem.golState == Golem.golemState.Returning) && golem.timeSinceDrop > 1.5f) && other.gameObject.tag == "PlayerBody")   )
        {
            for (int i = 0; i < sightPoints.Length; i++)
            {
                RaycastHit rayHit;
                if (Physics.Raycast(Eye.position, sightPoints[i].position - Eye.position, out rayHit, 4f, sightMask))
                {
                    //print(rayHit.collider.gameObject.name);
                    if (rayHit.collider.gameObject.tag == "PlayerBody")
                    {
                        //objInteraction = rayHit.collider.gameObject.GetComponentInChildren<ObjectInteractions>();
                        if (objInteraction.holdingPickup == true && seeker != null)
                        {
                             seeker.seekState = Seeker.seekerState.Chasing;
                             seeker.energyCubeTarget = objInteraction.objectToLift.GetComponent<LiftableObject>();
                             seeker.initPatrolPos = seeker.transform.position;
 
                        }
                        else if (golem != null)
                        {
                            golem.golState = Golem.golemState.Chasing;
                            golem.diegoTarget = rayHit.collider.gameObject;
                            golem.initPatrolPos = golem.transform.position;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
