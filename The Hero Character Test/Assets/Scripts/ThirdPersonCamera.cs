using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BarsEffect))]
public class ThirdPersonCamera : MonoBehaviour {

    [SerializeField]
    float distanceAway;
    [SerializeField]
    float distanceUp;
    [SerializeField]
    float smooth;
    [SerializeField]
    Transform followXform;
    [SerializeField]
    float widescreen = .2f;
    [SerializeField]
    float targetingTime = .5f;

    Vector3 targetPosition;
    Vector3 lookDir;
    //BarsEffect barEffect;
    CamStates camState = CamStates.Behind;


    public enum CamStates
    {
        Behind,
        FirstPerson,
        Target,
        Free
    }


    Vector3 velocityCanSmooth = Vector3.zero;
    [SerializeField]
    float canSmoothDampTime = .1f;
    
    // Use this for initialization
    void Start() {
        followXform = GameObject.FindWithTag("Player").transform;
        lookDir = followXform.forward;

        //barEffect = GetComponent<BarsEffect>();
        //if (barEffect == null)
        //{
        //    Debug.LogError("Attach a widescreen BarsEffect script to the camera.", this);
        //}
    }

    // Update is called once per frame
    void Update() {

    }

    private void LateUpdate()
    {
        Vector3 characterOffset = followXform.position + new Vector3(0f, distanceUp, 0f);

        if(Input.GetAxis("Target") > 0.01f)
        {
            //barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, widescreen, targetingTime);
           
            camState = CamStates.Target;
        }
        else
        {
            //barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, 0f, targetingTime);

            camState = CamStates.Behind;
        }
        switch(camState)
        {
            case CamStates.Behind:
                lookDir = characterOffset - this.transform.position;
                lookDir.y = 0;
                lookDir.Normalize();
                Debug.DrawRay(this.transform.position, lookDir, Color.green);


                targetPosition = characterOffset + followXform.up * distanceUp - lookDir * distanceAway;
                //Debug.DrawRay(followXform.position, Vector3.up * distanceUp, Color.red);
                //Debug.DrawRay(followXform.position, -1f * followXform.forward, Color.blue);
                Debug.DrawLine(followXform.position, targetPosition, Color.magenta);
                break;

            case CamStates.Target:
                lookDir = followXform.forward;

               
                break;
        }

        targetPosition = characterOffset + followXform.up * distanceUp - lookDir * distanceAway;

        CompensateForWalls(characterOffset, ref targetPosition);

        smoothPosition(this.transform.position, targetPosition);
       

        transform.LookAt(followXform);
    }

    void smoothPosition (Vector3 fromPos, Vector3 toPos)
    {
        this.transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCanSmooth, canSmoothDampTime);
    }

    void CompensateForWalls (Vector3 fromObject, ref Vector3 toTarget)
    {
        Debug.DrawLine(fromObject, toTarget, Color.cyan);
        RaycastHit wallHit = new RaycastHit();
        if(Physics.Linecast(fromObject,toTarget, out wallHit))
        {
            Debug.DrawRay(wallHit.point, Vector3.left, Color.red);
            toTarget = new Vector3(wallHit.point.x, toTarget.y, wallHit.point.z);
              
        }
    }

}
