using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddSpecialAttack : MonoBehaviour
{
    public void AddSpecialCounter(int counter)
    {
        DefenseStance.Instance.AddSpecialAttackCounter(counter);
    }
}
