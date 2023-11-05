using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class StateEnterLocomotion : StateMachineBehaviour
    {
        public bool rotation;
        public bool movement;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ThirdPersonController thirdPersonController = animator.GetComponent<ThirdPersonController>();
            if (thirdPersonController)
            {
                thirdPersonController.SetMovement(movement);
                thirdPersonController.SetRotation(rotation);
            }
        }
    }
}
