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

    float speed = 0f;
    float direction = 0f;
    float horizontal = 0f;
    float vertical = 0f;
    AnimatorStateInfo stateInfo;

    int m_LocomotionId = 0;



    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        if(animator.layerCount >= 2)
        {
            animator.SetLayerWeight(1, 1);
        }
        m_LocomotionId = Animator.StringToHash("Base layer.Locomotion");
	}
	
	// Update is called once per frame
	void Update () {
		if (animator)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0); 
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            StickToWorldspace(this.transform, gameCam.transform, ref direction, ref speed);

            animator.SetFloat("Speed", speed);
            animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);

          
        }
	}
    
    private void FixedUpdate()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (IsInLocomotion() && (direction >= 0 && horizontal >= 0)|| (direction < 0 && horizontal < 0))
          {
            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (horizontal < 0f ? -1f : 1f), 0f), Mathf.Abs(horizontal));
            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            this.transform.rotation = (this.transform.rotation * deltaRotation);
          }
    }

    public void StickToWorldspace(Transform root, Transform camera, ref float directionOut, ref float speedOut)
    {
        Vector3 rootDirection = root.forward;

        Vector3 stickDirection = new Vector3(horizontal, 0, vertical);

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
        angleRootToMove /= 180f;
        directionOut = angleRootToMove * directionSpeed;
    }

    public bool IsInLocomotion()
    {
        return stateInfo.nameHash == m_LocomotionId;
    }
}
