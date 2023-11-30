using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Health : MonoBehaviour
{
    Animator _animator;
    public float initialHP = 10f;
    public float currentHP;
    private int _animIDdeath;

    public UnityEvent onDeath;
    bool dead;

    private void Start()
    {
        currentHP = initialHP;
        _animator = GetComponentInParent<Animator>();
        _animIDdeath = Animator.StringToHash("Death");
        dead = false;
    }
    public void Damage(float dmg)
    {
        if (!dead)
        {
            currentHP -= dmg;
            CheckHealth();
        }
    }
    private void CheckHealth()
    {
        if (currentHP <= 0)
        {
            if (_animator) _animator.SetTrigger(_animIDdeath);
            onDeath.Invoke();
            dead = true;
        }
    }

}
