using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion_Movement : StateMachineBehaviour {

    public float Damping = 0.15f;

    private readonly int HashHorizontalParamater = Animator.StringToHash("Horizontal");
    private readonly int HashVerticalParamater = Animator.StringToHash("Vertical");

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 input = new Vector2(horizontal, vertical).normalized;

        animator.SetFloat(HashHorizontalParamater, input.x, Damping, Time.deltaTime);
        animator.SetFloat(HashVerticalParamater, input.y, Damping, Time.deltaTime);

    }
}
