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
    private int _animIDHit;

    public UnityEvent onHit;
    public UnityEvent onDeath;
    public bool dead;

    private void Start()
    {
        currentHP = initialHP;
        _animator = GetComponentInParent<Animator>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        _animIDdeath = Animator.StringToHash("Death");
        _animIDHit = Animator.StringToHash("Hit");
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
        else
        {
            _animator.SetTrigger(_animIDHit);
            onHit.Invoke();
            Debug.Log("hit");
        }
    }

}
