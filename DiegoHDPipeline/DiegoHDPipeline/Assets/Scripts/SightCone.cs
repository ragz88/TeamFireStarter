using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightCone : MonoBehaviour {

    [HideInInspector]
    public Transform[] sightPoints;

    public Seeker seeker;
    public LayerMask sightMask;
    float loseSightTime;

    float noLineOfSightTime = 0;

    Transform Eye;
    ObjectInteractions objInteraction;

	// Use this for initialization
	void Start () {
        Eye = transform.parent;
        objInteraction = GameObject.Find("Diego").GetComponent<ObjectInteractions>();                                     //change when it become Diego!!!!!!!!!!!!
        loseSightTime = seeker.loseSightTime;
    }
	
	// Update is called once per frame
	void Update () {
        bool canSeePlayer = false;
		if (seeker.seekState == Seeker.seekerState.Chasing)
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
                    seeker.seekState = Seeker.seekerState.Returning;
                }
            }
        }
	}

    private void OnTriggerStay(Collider other)
    {
        if ((seeker.seekState == Seeker.seekerState.Patrol || seeker.seekState == Seeker.seekerState.Returning) && other.gameObject.tag == "PlayerBody")
        {
            //Debug.DrawRay(Eye.position, Vector3.Normalize(sightPoints[0].position - Eye.position) * 4, Color.cyan);
            //Debug.DrawRay(Eye.position, Vector3.Normalize(sightPoints[1].position - Eye.position) * 4, Color.cyan);
            //Debug.DrawRay(Eye.position, Vector3.Normalize(sightPoints[2].position - Eye.position) * 4, Color.cyan);
            //Debug.DrawRay(Eye.position, Vector3.Normalize(sightPoints[3].position - Eye.position) * 4, Color.cyan);
            //Debug.DrawRay(Eye.position, Vector3.Normalize(sightPoints[4].position - Eye.position) * 4, Color.cyan);

            for (int i = 0; i < sightPoints.Length; i++)
            {
                RaycastHit rayHit;
                if (Physics.Raycast(Eye.position, sightPoints[i].position - Eye.position, out rayHit, 4f, sightMask))
                {
                    //print(rayHit.collider.gameObject.name);
                    if (rayHit.collider.gameObject.tag == "PlayerBody")
                    {
                        //objInteraction = rayHit.collider.gameObject.GetComponentInChildren<ObjectInteractions>();
                        if (objInteraction.holdingPickup == true)
                        {
                            seeker.seekState = Seeker.seekerState.Chasing;
                            seeker.energyCubeTarget = objInteraction.objectToLift.GetComponent<LiftableObject>();
                            seeker.initPatrolPos = seeker.transform.position;
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
