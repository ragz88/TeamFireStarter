﻿using UnityEngine;

// MoveBehaviour inherits from GenericBehaviour. This class corresponds to basic walk and run behaviour, it is the default behaviour.
public class MoveBehaviour : GenericBehaviour
{
	public float walkSpeed = 0.15f;                 // Default walk speed.
	public float runSpeed = 1.0f;                   // Default run speed.
	public float sprintSpeed = 2.0f;                // Default sprint speed.
	public float speedDampTime = 0.1f;              // Default damp time to change the animations based on current speed.
	public string jumpButton = "Jump";              // Default jump button.
	public float jumpHeight = 1.5f;                 // Default jump height.
	public float jumpIntertialForce = 10f;          // Default horizontal inertial force when jumping.
    public GameObject cam;
    public bool lifting = false;
    public bool pushing = false;
    bool donePushing = false;
    public GameObject pushCollider;
    CapsuleCollider collider;
    float colliderHeight;
    float colliderOffset;
    public float jumpColliderModifier = .5f;
    public float jumpColliderOffset = .5f;
    private float speed, speedSeeker;               // Moving speed.
	private int jumpBool;                           // Animator variable related to jumping.
	private int groundedBool;                       // Animator variable related to whether or not the player is on ground.
	private bool jump;                              // Boolean to determine whether or not the player started a jump.
	private bool isColliding;                       // Boolean to determine if the player has collided with an obstacle.
    private int angle;

    // Start is always called after any Awake functions.
    void Start() 
	{
        collider = GetComponent<CapsuleCollider>();
        colliderHeight = collider.height;
        colliderOffset = collider.center.y;
        if(cam == null)
        {
            cam = GameObject.FindObjectOfType<AdvancedCamera>().gameObject;
        }
		// Set up the references.
		jumpBool = Animator.StringToHash("Jump");
		groundedBool = Animator.StringToHash("Grounded");
        angle = Animator.StringToHash("Angle");
        behaviourManager.GetAnim.SetBool (groundedBool, true);

		// Subscribe and register this behaviour as the default behaviour.
		behaviourManager.SubscribeBehaviour (this);
		behaviourManager.RegisterDefaultBehaviour (this.behaviourCode);
		speedSeeker = runSpeed;
	}

	// Update is used to set features regardless the active behaviour.
	void Update ()
	{

        if(jump)
        {
            collider.height = colliderHeight * jumpColliderModifier;
            collider.center.Set(0f, (colliderOffset + jumpColliderOffset), 0f);
        }
        else
        {
            collider.height = colliderHeight;
            collider.center.Set(0f, (colliderOffset), 0f);
        }
       // print(behaviourManager.IsGrounded());
        if(Input.GetKey("l"))
        {
            lifting = true;
        }
        else
        {
            lifting = false;
        }
        if (Input.GetKey("p"))
        {
            pushing = true;
            pushCollider.SetActive(true);
        }
        else
        {
            pushing = false;
            pushCollider.SetActive(false);
        }
        behaviourManager.GetAnim.SetBool("Lifting", lifting);
        behaviourManager.GetAnim.SetBool("Pushing", pushing);

        if(!pushing)
        {
            if((this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base Layer.ExitPush") == false))
            {
                donePushing = true;
            }
            else
            {
                donePushing = false;
            }
        }

        // Get jump input.
        if (!jump && Input.GetButtonDown(jumpButton) && behaviourManager.IsCurrentBehaviour(this.behaviourCode) && !behaviourManager.IsOverriding())
		{
			jump = true;
		}
	}

	// LocalFixedUpdate overrides the virtual function of the base class.
	public override void LocalFixedUpdate()
	{
		// Call the basic movement manager.
		MovementManagement(behaviourManager.GetH, behaviourManager.GetV);

		// Call the jump manager.
		JumpManagement();
	}

	// Execute the idle and walk/run jump movements.
	void JumpManagement()
	{
		// Start a new jump.
		if (jump && !behaviourManager.GetAnim.GetBool(jumpBool) && behaviourManager.IsGrounded())
		{
			// Set jump related parameters.
			behaviourManager.LockTempBehaviour(this.behaviourCode);
			behaviourManager.GetAnim.SetBool(jumpBool, true);
            
			// Is a locomotion jump?
			if(behaviourManager.GetAnim.GetFloat(speedFloat) > 0.1)
			{
				// Temporarily change player friction to pass through obstacles.
				GetComponent<CapsuleCollider>().material.dynamicFriction = 0f;
				GetComponent<CapsuleCollider>().material.staticFriction = 0f;
				// Set jump vertical impulse velocity.
				float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
				velocity = Mathf.Sqrt(velocity);
				behaviourManager.GetRigidBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
			}
		}
		// Is already jumping?
		else if (behaviourManager.GetAnim.GetBool(jumpBool))
		{
			// Keep forward movement while in the air.
			if (!behaviourManager.IsGrounded() && !isColliding && behaviourManager.GetTempLockStatus())
			{
				behaviourManager.GetRigidBody.AddForce(transform.forward * jumpIntertialForce * Physics.gravity.magnitude * sprintSpeed, ForceMode.Acceleration);
			}
			// Has landed?
			if ((behaviourManager.GetRigidBody.velocity.y < 0) && behaviourManager.IsGrounded())
			{
				behaviourManager.GetAnim.SetBool(groundedBool, true);
              
                // Change back player friction to default.
                GetComponent<CapsuleCollider>().material.dynamicFriction = 0.6f;
				GetComponent<CapsuleCollider>().material.staticFriction = 0.6f;
				// Set jump related parameters.
				jump = false;
				behaviourManager.GetAnim.SetBool(jumpBool, false);
				behaviourManager.UnlockTempBehaviour(this.behaviourCode);
			}
		}
	}

	// Deal with the basic player movement
	void MovementManagement(float horizontal, float vertical)
	{
		// On ground, obey gravity.
		if (behaviourManager.IsGrounded())
			behaviourManager.GetRigidBody.useGravity = true;

        // Call function that deals with player orientation.
        //if(Vector3.Angle(transform.forward, Rotating(horizontal, vertical)) > 90)
        {
            Rotating(horizontal, vertical);
        } 

		// Set proper speed.
		Vector2 dir = new Vector2(horizontal, vertical);
		speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
		// This is for PC only, gamepads control speed via analog stick.
		speedSeeker += Input.GetAxis("Mouse ScrollWheel");
		speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
		speed *= speedSeeker;
		if (behaviourManager.IsSprinting())
		{
			speed = sprintSpeed;
		}

		behaviourManager.GetAnim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
	}

	// Rotate the player to match correct orientation, according to camera and key pressed.
	Vector3 Rotating(float horizontal, float vertical)
	{
		// Get camera forward direction, without vertical component.
		Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);

		// Player is moving on ground, Y component of camera facing is not relevant.
		forward.y = 0.0f;
		forward = forward.normalized;

		// Calculate target direction based on camera forward and direction key.
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		Vector3 targetDirection;
		targetDirection = forward * vertical + right * horizontal;
        float targetAngle = Vector3.Angle(cam.transform.forward, targetDirection);
        float playerAngle = Vector3.Angle(cam.transform.forward, this.transform.forward);
        //behaviourManager.GetAnim.SetFloat(angle, playerAngle - targetAngle);
        // Lerp current direction to calculated target direction.
        if ((behaviourManager.IsMoving() && targetDirection != Vector3.zero))
		{
			Quaternion targetRotation = Quaternion.LookRotation (targetDirection);

			Quaternion newRotation = Quaternion.Slerp(behaviourManager.GetRigidBody.rotation, targetRotation, behaviourManager.turnSmoothing);
            //if(Vector3.Angle(transform.forward, targetDirection) > 90)
            {
                if (!pushing)
                {
                    if (donePushing)
                    {
                     behaviourManager.GetRigidBody.MoveRotation(newRotation);
                     behaviourManager.SetLastDirection(targetDirection);
                    }
                }

            }
		}
		// If idle, Ignore current camera facing and consider last moving direction.
		if(!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
		{
			behaviourManager.Repositioning();
		}

        //behaviourManager.GetAnim.SetFloat(angle, playerAngle - targetAngle);
        //print(playerAngle - targetAngle);
        return targetDirection;
	}

	// Collision detection.
	private void OnCollisionStay(Collision collision)
	{
		isColliding = true;
	}
	private void OnCollisionExit(Collision collision)
	{
		isColliding = false;
	}
}