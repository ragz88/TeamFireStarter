using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {
    [SerializeField]
    private Animator animator;
    [SerializeField]
    float directionDampTime = .25f;
    [SerializeField]
    ThirdPersonCamera gameCam;
    [SerializeField]
    float directionSpeed = 3f;
    [SerializeField]
    float rotationDegreePerSecond = 120;
    [SerializeField]
    private float speedDampTime = 0.05f;

    float speed = 0f;
    float direction = 0f;
    float charAngle = 0f;
    float leftX = 0f;
    float leftY = 0f;
    AnimatorStateInfo stateInfo;
    AnimatorTransitionInfo transInfo;

    private int m_LocomotionId = 0;
    private int m_LocomotionPivotLId = 0;
    private int m_LocomotionPivotRId = 0;
    private int m_LocomotionPivotLTransId = 0;
    private int m_LocomotionPivotRTransId = 0;
    public Animator Animator
    {
        get
        {
            return this.animator;
        }
    }

    public float Speed
    {
        get
        {
            return this.speed;
        }
    }

    public float LocomotionThreshold { get { return 0.2f; } }


    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        if(animator.layerCount >= 2)
        {
            animator.SetLayerWeight(1, 1);
        }
        m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
        m_LocomotionPivotLId = Animator.StringToHash("Base Layer.LocomotionPivotL");
        m_LocomotionPivotRId = Animator.StringToHash("Base Layer.LocomotionPivotR");
        m_LocomotionPivotLTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotL");
        m_LocomotionPivotRTransId = Animator.StringToHash("Base Layer.Locomotion -> Base Layer.LocomotionPivotR");
    }
	
	// Update is called once per frame
	void Update () {
        if (animator && gameCam.CamState != ThirdPersonCamera.CamStates.FirstPerson)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            transInfo = animator.GetAnimatorTransitionInfo(0);

            leftX = Input.GetAxis("Horizontal");
            leftY = Input.GetAxis("Vertical");

            charAngle = 0f;
            direction = 0f;

            StickToWorldspace(this.transform, gameCam.transform, ref direction, ref speed, ref charAngle,  IsInPivot());

            animator.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
            animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);

            if (speed > LocomotionThreshold)
            {
                if (!IsInPivot())
                {
                    Animator.SetFloat("Angle", charAngle);
                }

                if(speed < LocomotionThreshold && Mathf.Abs(leftX)<0.05f)
                {
                    animator.SetFloat("Direction", 0f);
                    animator.SetFloat("Angle", 0f);
                }
            }
        }
	}
    
    private void FixedUpdate()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
      
        if (IsInLocomotion() && (direction >= 0 && leftX >= 0)|| (direction < 0 && leftX < 0))
          { 
            
            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (leftX < 0f ? -1f : 1f), 0f), Mathf.Abs(leftX));
            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            this.transform.rotation = (this.transform.rotation * deltaRotation);
            Debug.Log(rotationAmount);
        }
       
    }

    public void StickToWorldspace(Transform root, Transform camera, ref float directionOut, ref float speedOut, ref float angleOut, bool isPivoting)
    {
        Vector3 rootDirection = root.forward;

        Vector3 stickDirection = new Vector3(leftX, 0, leftY);

        speedOut = stickDirection.sqrMagnitude;

        Vector3 CameraDirection = camera.forward;
        CameraDirection.y = 0.0f;
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, CameraDirection);

        Vector3 moveDirection = referentialShift * stickDirection;
        Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

        Debug.DrawRay(new Vector3 (root.position.x,root.position.y + 2f,root.position.z),moveDirection,Color.green);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), axisSign, Color.red);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);

        float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
        if(!isPivoting)
        {
            angleOut = angleRootToMove;
        }
        angleRootToMove /= 180f;
        directionOut = angleRootToMove * directionSpeed;
    }

    public bool IsInLocomotion()
    {
        return stateInfo.nameHash == m_LocomotionId;
    }

    public bool IsInPivot()
    {
        return stateInfo.nameHash == m_LocomotionPivotLId ||
            stateInfo.nameHash == m_LocomotionPivotRId ||
            transInfo.nameHash == m_LocomotionPivotLTransId ||
            transInfo.nameHash == m_LocomotionPivotRTransId;
    }
}
