using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseAttack : StateMachineBehaviour
{
    public GameObject slashPrefab;
    public Vector3 offset;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (slashPrefab == null) return;

        float dragX = animator.GetFloat("DragAttackX");
        float dragY = animator.GetFloat("DragAttackY");
        float angle = Mathf.Atan2(dragY, -dragX) * Mathf.Rad2Deg;

        GameObject.Instantiate(slashPrefab, animator.transform.position + offset, animator.transform.rotation * Quaternion.Euler(Camera.main.transform.eulerAngles.x,0,180 - angle) , animator.transform);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
