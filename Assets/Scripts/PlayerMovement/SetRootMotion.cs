using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRootMotion : StateMachineBehaviour
{
    public bool rootmotion;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = rootmotion;
    }
}
