using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {
    [SerializeField]
    private Animator animator;
    [SerializeField]
    float directionDampTime = .25f;

    float speed = 0;
    float h = 0;
    float v = 0;

	
    
    // Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        if(animator.layerCount >= 2)
        {
            animator.SetLayerWeight(1, 1);
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (animator)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            speed = new Vector2(h, v).sqrMagnitude;

            animator.SetFloat("Speed", speed);
            animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
        }
	}
}
