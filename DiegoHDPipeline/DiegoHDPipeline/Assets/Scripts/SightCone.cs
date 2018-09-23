using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightCone : MonoBehaviour {

    [HideInInspector]
    public Transform[] sightPoints;

    public Seeker seeker;
    public LayerMask sightMask;

    Transform Eye;
    ObjectInteractions objInteraction;

	// Use this for initialization
	void Start () {
        Eye = transform.parent;
        objInteraction = GameObject.Find("ThirdPersonController").GetComponentInChildren<ObjectInteractions>();                                     //change when it become Diego!!!!!!!!!!!!

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        if ((seeker.seekState == Seeker.seekerState.Patrol || seeker.seekState == Seeker.seekerState.Returning) && other.gameObject.tag == "PlayerBody")
        {
            //Debug.DrawRay(Eye.position, sightPoints[0].position - Eye.position, Color.cyan);
            for (int i = 0; i < sightPoints.Length; i++)
            {
                RaycastHit rayHit;
                if (Physics.Raycast(Eye.position, sightPoints[i].position - Eye.position, out rayHit, 2f, sightMask))
                {
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
