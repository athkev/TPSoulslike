using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenshinImpactMovementSystem
{
    public class FaceInput : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ThirdPersonController thirdPersonController = animator.GetComponent<ThirdPersonController>();
            if (thirdPersonController)
            {
                if (thirdPersonController.GetInput().magnitude != 0)
                {
                    animator.rootRotation = Quaternion.Euler(0, thirdPersonController.GetInputAngle(), 0);
                }
            }
        }
    }
}
