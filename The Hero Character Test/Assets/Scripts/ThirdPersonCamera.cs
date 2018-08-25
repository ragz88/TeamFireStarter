using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Vector3 offset = new Vector3(0f, 1f, 0f);

    Vector3 targetPosition;
    Vector3 lookDir;


    Vector3 velocityCanSmooth = Vector3.zero;
    [SerializeField]
    float canSmoothDampTime = .1f;
    
    // Use this for initialization
    void Start() {
        followXform = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update() {

    }

    private void LateUpdate()
    {
        Vector3 characterOffset = followXform.position + offset;

        lookDir = characterOffset - this.transform.position;
        lookDir.y = 0;
        lookDir.Normalize();
        Debug.DrawRay(this.transform.position, lookDir, Color.green);


        targetPosition = characterOffset + followXform.up * distanceUp - lookDir * distanceAway;
        Debug.DrawRay(followXform.position, Vector3.up * distanceUp, Color.red);
        Debug.DrawRay(followXform.position, -1f * followXform.forward, Color.blue);
        Debug.DrawLine(followXform.position, targetPosition, Color.magenta);

       
        smoothPosition(this.transform.position, targetPosition);
       

        transform.LookAt(followXform);
    }

    void smoothPosition (Vector3 fromPos, Vector3 toPos)
    {
        this.transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCanSmooth, canSmoothDampTime);
    }

}
