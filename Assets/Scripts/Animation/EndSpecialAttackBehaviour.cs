using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSpecialAttackBehaviour : StateMachineBehaviour
{
    DefenseStance defenseStance;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (defenseStance == null) GetRef(animator.gameObject);

        defenseStance.EndSpecialAttack();
    }

    void GetRef(GameObject obj)
    {
        defenseStance = obj.GetComponent<DefenseStance>();
    }
}
