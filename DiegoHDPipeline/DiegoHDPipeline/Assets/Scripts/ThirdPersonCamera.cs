using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CameraPosition
{
    Vector3 position;
    Transform xform;

    public Vector3 Position { get { return position; } set { position = value; } }
    public Transform Xform { get { return xform; } set { xform = value; } }

    public void Init(string camName, Vector3 pos, Transform transform, Transform parent)
    {
        position = pos;
        xform = transform;
        xform.name = camName;
        xform.parent = parent;
        xform.localPosition = Vector3.zero;
        xform.localPosition = position;
    }



}


    //[RequireComponent(typeof(BarsEffect))]
    public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField]
    private Transform parentRig;
    [SerializeField]
    float distanceAway;
    [SerializeField]
    float distanceUp;
    [SerializeField]
    float distanceAwayMultipler = 1.5f;
    [SerializeField]
    float distanceUpMultiplier = 5f;
    [SerializeField]
    float smooth;
    [SerializeField]
    Transform followXform;
    [SerializeField]
    float widescreen = .2f;
    [SerializeField]
    float targetingTime = .5f;
    [SerializeField]
    float firstPersonThreshold = 0.5f;
    [SerializeField]
    CharacterController follow;
    [SerializeField]
    float firstPersonLookSpeed = 1.5f;
    [SerializeField]
    Vector2 firstPersonXAxisClamp = new Vector2(-70.0f, 90.0f);
    [SerializeField]
    float fPSRotationDegreePerSecond = 120f;
    [SerializeField]
    float freeThreshold = -0.1f;
    Vector2 camMinDistFromChar = new Vector2(1f, -0.5f);
    [SerializeField]
    float rightStickThreshold = 0.1f;
    [SerializeField]
    const float freeRotationDegreePerSecond = -5f;



    Vector3 targetPosition;
    Vector3 lookDir;
    //BarsEffect barEffect;
    CamStates camState = CamStates.Behind;
    CameraPosition firstPersonCameraPos;
    float xAxisRot = 0.0f;
    CameraPosition firstPersonCamPos;
    float lookWeight;
    const float TARGET_THRESHOLD = .01f;
    Vector3 curLookDir;
    Vector3 savedRigToGoal;
    float distanceAwayFree;
    float distanceUpFree;
    Vector2 rightStickPrevFrame = Vector2.zero;
    Vector3 characterOffset;

    public CamStates CamState
    {
        get
        {
            return this.camState;
        }
    }

    public enum CamStates
     {
        Behind,
        FirstPerson,
        Target,
        Free
     }

    public Vector3 rigToGoalDirection
    {
        get
        {
            // Move height and distance from character in separate parentRig transform since RotateAround has control of both position and rotation
            Vector3 rigToGoalDirection = Vector3.Normalize(characterOffset - this.transform.position);
            // Can't calculate distanceAway from a vector with Y axis rotation in it; zero it out
            rigToGoalDirection.y = 0f;

            return rigToGoalDirection;
        }
    }


    Vector3 velocityCanSmooth = Vector3.zero;
      [SerializeField]
      float camSmoothDampTime = .1f;
      private Vector3 velocityLookDir = Vector3.zero;
      [SerializeField]
      private float lookDirDampTime = 0.1f;

    // Use this for initialization
    void Start()
    {
        parentRig = this.transform;//.parent;
        if (parentRig == null)
        {
            Debug.LogError("Parent camera to empty GameObject.", this);
        }

        follow = GameObject.FindWithTag("PlayerBody").GetComponent<CharacterController>();
        followXform = GameObject.FindWithTag("Player").transform;
        lookDir = followXform.forward;
        curLookDir = followXform.forward;

        //barEffect = GetComponent<BarsEffect>();
        //if (barEffect == null)
        //{
        //    Debug.LogError("Attach a widescreen BarsEffect script to the camera.", this);
        //}
        firstPersonCamPos = new CameraPosition();
        firstPersonCamPos.Init
        (
         "First Person Camera",
          new Vector3(0.0f, 1.6f, 0.2f),
          new GameObject().transform,
          follow.transform
        );



    }

    // Update is called once per frame
    void Update() {

    }

    private void LateUpdate()
    {
        float rightX = Input.GetAxis("RightStickX");
        float rightY = Input.GetAxis("RightStickY");
        float leftX = Input.GetAxis("Horizontal");
        float leftY = Input.GetAxis("Vertical");
        characterOffset = followXform.position + new Vector3(0f, distanceUp, 0f);
        Vector3 lookAt = characterOffset;
        targetPosition = Vector3.zero;

        if (Input.GetAxis("Target") > TARGET_THRESHOLD)
        {
            //barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, widescreen, targetingTime);
           
            camState = CamStates.Target;
        }
        else
        {
            //barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, 0f, targetingTime);

            if (rightY > firstPersonThreshold && camState != CamStates.Free && !follow.IsInLocomotion())
            {
                xAxisRot = 0;
                lookWeight = 0f;
                camState = CamStates.FirstPerson;
            }

            if ((rightY < freeThreshold ) /*&& System.Math.Round(follow.Speed, 2) == 0*/)
            {
                camState = CamStates.Free;
                savedRigToGoal = Vector3.zero;
            }

            if ((camState == CamStates.FirstPerson &&Input.GetButton("ExitFPV")) ||
            (camState == CamStates.Target && (Input.GetAxis("Target") <= TARGET_THRESHOLD)))
            {
                camState = CamStates.Behind;
            }
        }

        follow.Animator.SetLookAtWeight(lookWeight);

        switch(camState)
        {
            case CamStates.Behind:
                ResetCamera();

               
                if (follow.Speed > follow.LocomotionThreshold && follow.IsInLocomotion() && !follow.IsInPivot())
                {
                    lookDir = Vector3.Lerp(followXform.right * (leftX < 0 ? 1f : -1f), followXform.forward * (leftY < 0 ? -1f : 1f), Mathf.Abs(Vector3.Dot(this.transform.forward, followXform.forward)));
                    Debug.DrawRay(this.transform.position, lookDir, Color.white);

                    
                    curLookDir = Vector3.Normalize(characterOffset - this.transform.position);
                    curLookDir.y = 0;
                    Debug.DrawRay(this.transform.position, curLookDir, Color.green);

                   
                    curLookDir = Vector3.SmoothDamp(curLookDir, lookDir, ref velocityLookDir, lookDirDampTime);
                }

                targetPosition = characterOffset + followXform.up * distanceUp - Vector3.Normalize(curLookDir) * distanceAway;
                Debug.DrawLine(followXform.position, targetPosition, Color.magenta);

                break;

            case CamStates.Target:
                ResetCamera();

                lookDir = followXform.forward;
                curLookDir = followXform.forward;

                targetPosition = characterOffset + followXform.up * distanceUp - lookDir * distanceAway;
                break;

            case CamStates.FirstPerson:
                xAxisRot += (-leftY * firstPersonLookSpeed);
                xAxisRot = Mathf.Clamp(xAxisRot, firstPersonXAxisClamp.x, firstPersonXAxisClamp.y);
                firstPersonCamPos.Xform.localRotation = Quaternion.Euler(xAxisRot, 0f, 0f);

                Quaternion rotationShift = Quaternion.FromToRotation(this.transform.forward, firstPersonCamPos.Xform.forward);
                this.transform.rotation = rotationShift * this.transform.rotation;

                follow.Animator.SetLookAtPosition(firstPersonCamPos.Xform.position + firstPersonCamPos.Xform.forward);
                lookWeight = Mathf.Lerp(lookWeight, 1.0f, Time.deltaTime * firstPersonLookSpeed);    

                Vector3 rotationAmount = Vector3.Lerp(Vector3.zero,new Vector3(0f, fPSRotationDegreePerSecond * (leftX < 0f ? -1f : 1f),0f),Mathf.Abs(leftX));
                Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
                follow.transform.rotation = (follow.transform.rotation * deltaRotation);

                targetPosition = firstPersonCamPos.Xform.position;
                lookAt = Vector3.Lerp(targetPosition + followXform.forward, this.transform.position + this.transform.forward, camSmoothDampTime * Time.deltaTime);
                Debug.DrawRay(Vector3.zero, lookAt, Color.black);
                Debug.DrawRay(Vector3.zero, targetPosition + followXform.forward, Color.white);
                Debug.DrawRay(Vector3.zero, firstPersonCamPos.Xform.position + firstPersonCamPos.Xform.forward, Color.cyan);


                lookAt = (Vector3.Lerp(this.transform.position + this.transform.forward, lookAt, Vector3.Distance(this.transform.position, firstPersonCamPos.Xform.position)));

                break;

            case CamStates.Free:
                //============================================================================================================================================================
                //lookWeight = Mathf.Lerp(lookWeight, 0.0f, Time.deltaTime * firstPersonLookSpeed);

                //Vector3 rigToGoalDirection = Vector3.Normalize(characterOffset - this.transform.position);
                //rigToGoalDirection.y = 0f;
                //Vector3 rigToGoal = characterOffset - parentRig.position;
                //rigToGoal.y = 0f;
                //Debug.DrawRay(parentRig.transform.position, rigToGoal, Color.red);


                //if (rightY < -1f * rightStickThreshold && rightY <= rightStickPrevFrame.y && Mathf.Abs(rightX) < rightStickThreshold)

                //{
                //    // Zooming out
                //    distanceUpFree = Mathf.Lerp(distanceUp, distanceUp * distanceUpMultiplier, Mathf.Abs(rightY));
                //    distanceAwayFree = Mathf.Lerp(distanceAway, distanceAway * distanceAwayMultipler, Mathf.Abs(rightY));
                //    targetPosition = characterOffset + followXform.up * distanceUpFree - rigToGoalDirection * distanceAwayFree;

                //}
                //else if (rightY > rightStickThreshold && rightY >= rightStickPrevFrame.y && Mathf.Abs(rightX) < rightStickThreshold)
                //{
                //    // Zooming in

                //    distanceUpFree = Mathf.Lerp(Mathf.Abs(transform.position.y - characterOffset.y), camMinDistFromChar.y, rightY);              
                //    distanceAwayFree = Mathf.Lerp(rigToGoal.magnitude, camMinDistFromChar.x, rightY);
                //    targetPosition = characterOffset + followXform.up * distanceUpFree - rigToGoalDirection * distanceAwayFree;

                //}
                //if(rightX !=0 || rightY !=0)
                //{
                //    savedRigToGoal = rigToGoalDirection;
                //}
                //parentRig.RotateAround(characterOffset, followXform.up, freeRotationDegreePerSecond * (Mathf.Abs(rightX) > rightStickThreshold ? rightX : 0f));
                //if(targetPosition == Vector3.zero)
                //{
                //    targetPosition = characterOffset + followXform.up * distanceAway - savedRigToGoal * distanceAway;
                //}
                ////smoothPosition(transform.position, targetPosition);
                ////transform.LookAt(lookAt);
                //==================================================================================================================================================================

                lookWeight = Mathf.Lerp(lookWeight, 0.0f, Time.deltaTime * firstPersonLookSpeed);

                Vector3 rigToGoal = characterOffset - parentRig.position;
                rigToGoal.y = 0f;
                Debug.DrawRay(parentRig.transform.position, rigToGoal, Color.red);

                // Panning in and out
                // If statement works for positive values; don't tween if stick not increasing in either direction; also don't tween if user is rotating
                // Checked against rightStickThreshold because very small values for rightY mess up the Lerp function
                if (rightY /*< lastStickMin && rightY*/ < -1f * rightStickThreshold && rightY <= rightStickPrevFrame.y && Mathf.Abs(rightX) < rightStickThreshold)
                {
                    // Zooming out
                    distanceUpFree = Mathf.Lerp(distanceUp, distanceUp * distanceUpMultiplier, Mathf.Abs(rightY));
                    distanceAwayFree = Mathf.Lerp(distanceAway, distanceAway * distanceAwayMultipler, Mathf.Abs(rightY));
                    targetPosition = characterOffset + followXform.up * distanceUpFree - rigToGoalDirection * distanceAwayFree;
                    //lastStickMin = rightY;
                }
                else if (rightY > rightStickThreshold && rightY >= rightStickPrevFrame.y && Mathf.Abs(rightX) < rightStickThreshold)
                {
                    // Zooming in
                    // Subtract height of camera from height of player to find Y distance
                    distanceUpFree = Mathf.Lerp(Mathf.Abs(transform.position.y - characterOffset.y), camMinDistFromChar.y, rightY);
                    // Use magnitude function to find X distance	
                    distanceAwayFree = Mathf.Lerp(rigToGoal.magnitude, camMinDistFromChar.x, rightY);
                    targetPosition = characterOffset + followXform.up * distanceUpFree - rigToGoalDirection * distanceAwayFree;
                    //lastStickMin = float.PositiveInfinity;
                }

                // Store direction only if right stick inactive
                if (rightX != 0 || rightY != 0)
                {
                    savedRigToGoal = rigToGoalDirection;
                }


                // Rotating around character
                parentRig.RotateAround(characterOffset, followXform.up, (freeRotationDegreePerSecond * (Mathf.Abs(rightX) > rightStickThreshold ? rightX : 0f))*-1);

                // Still need to track camera behind player even if they aren't using the right stick; achieve this by saving distanceAwayFree every frame
                if (targetPosition == Vector3.zero)
                {
                    targetPosition = characterOffset + followXform.up * distanceUpFree - savedRigToGoal * distanceAwayFree;
                }
                break;
        }

        //if (camState != CamStates.Free)
        {
            CompensateForWalls(characterOffset, ref targetPosition);

            smoothPosition(this.transform.position, targetPosition);


            transform.LookAt(lookAt);
        }
        rightStickPrevFrame = new Vector2(rightX, rightY);
        
    }

    void smoothPosition (Vector3 fromPos, Vector3 toPos)
    {
        this.transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCanSmooth, camSmoothDampTime);
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

    void ResetCamera()
    {
        lookWeight = Mathf.Lerp(lookWeight, 0.0f, Time.deltaTime * firstPersonLookSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime);
    }

  }
