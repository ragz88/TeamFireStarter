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
    [SerializeField]
    private float fovDampTime = 3f;
    [SerializeField]
    private float jumpMultiplier = 1f;
    [SerializeField]
    private CapsuleCollider capCollider;
    [SerializeField]
    private float jumpDist = 1f;

    float speed = 0f;
    float direction = 0f;
    float charAngle = 0f;
    float leftX = 0f;
    float leftY = 0f;
    const float SPRINT_SPEED = 2.0f;
    const float SPRINT_FOV = 75.0f;
    const float NORMAL_FOV = 60.0f;
     float capsuleHeight;	
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
        capCollider = GetComponent<CapsuleCollider>();
        capsuleHeight = capCollider.height;
        if (animator.layerCount >= 2)
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

           
            if (Input.GetButton("Jump"))
            {
                animator.SetBool("Jump", true);
            }
            else
            {
                animator.SetBool("Jump", false);
            }

           

            leftX = Input.GetAxis("Horizontal");
            leftY = Input.GetAxis("Vertical");

            charAngle = 0f;
            direction = 0f;
            float charSpeed = 0f;

            StickToWorldspace(this.transform, gameCam.transform, ref direction, ref charSpeed, ref charAngle,  IsInPivot());


            if (Input.GetButton("Sprint"))
            {
                speed = Mathf.Lerp(speed, SPRINT_SPEED, Time.deltaTime);
                gameCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gameCam.GetComponent<Camera>().fieldOfView, SPRINT_FOV, fovDampTime * Time.deltaTime);
            }
            else
            {
                speed = charSpeed;
                gameCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gameCam.GetComponent<Camera>().fieldOfView, NORMAL_FOV, fovDampTime * Time.deltaTime);
            }

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
            // Debug.Log(rotationAmount);

            if (IsInJump())
            {

                float oldY = transform.position.y;
                transform.Translate(Vector3.up * jumpMultiplier * animator.GetFloat("JumpCurve"));
                if (IsInLocomotionJump())
                {
                    transform.Translate(Vector3.forward * Time.deltaTime * jumpDist);
                }

                capCollider.height = capsuleHeight + (animator.GetFloat("CapsuleCurve") * 0.5f);
                Debug.Log(capCollider.height);
                if (gameCam.CamState != ThirdPersonCamera.CamStates.Free)
                {
                    gameCam.transform.Translate(Vector3.up * (gameCam.transform.position.y - oldY));
                }
            }
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
    public bool IsInJump()
    {
        return (IsInIdleJump() || IsInLocomotionJump());
    }

    public bool IsInIdleJump()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.IdleJump");
    }

    public bool IsInLocomotionJump()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LocomotionJump");
    }
}
