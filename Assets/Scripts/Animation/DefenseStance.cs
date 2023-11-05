using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;
using System;
//while this script is active,
//delta X and Y of camera movement will be sent to animator parameter for parry stance blendtree
public class DefenseStance : MonoBehaviour
{
    [SerializeField]
    InputActionReference look, attack;
    bool _defenseActive;
    bool _attackActive;
    Animator _animator;
    public Image crossHairImage;

    private int _animIDdeltaX;
    private int _animIDdeltaY;
    private int _animIDdeltaAttackX;
    private int _animIDdeltaAttackY;
    private int _animIDableToAttack;
    private int _animIDattackBuffer;
    private int _animIDattack;
    private int _animIDdefending;

    private int _animLayerUpperbody;
    private float _layerWeight = 0;

    private float deltaX;
    private float deltaY;

    private float angle;

    [SerializeField] float dragThreshold = .2f;
    [SerializeField] float lerpSpeed = .1f;

    public UnityEvent onDefense = new UnityEvent();
    public UnityEvent offDefense = new UnityEvent();

    bool ableToDefend = true;
    bool ableToAttack = true;
    bool attackBuffer;
    float bufferTimer;
    float bufferTimerMax = .3f;

    bool defending;

    private void OnEnable()
    {
        if (!TryGetComponent<Animator>(out _animator))
        {
            Debug.Log("NO ANIMATOR SET ON DEFENSESTANCE");
        }
    }
    private void Start()
    {
        _animIDdeltaX = Animator.StringToHash("DragX");
        _animIDdeltaY = Animator.StringToHash("DragY");
        _animIDdeltaAttackX = Animator.StringToHash("DragAttackX");
        _animIDdeltaAttackY = Animator.StringToHash("DragAttackY");
        _animIDableToAttack = Animator.StringToHash("AbleToAttack");
        _animIDattack = Animator.StringToHash("Attack");
        _animIDattackBuffer = Animator.StringToHash("AttackBuffer");
        _animIDdefending = Animator.StringToHash("Defending");
        _animLayerUpperbody = _animator.GetLayerIndex("Strafe_UpperSword");
    }

    private void Update()
    {
        SetDefenseLayer(_defenseActive);
        SetEvent(_defenseActive);
        _animator.SetBool(_animIDableToAttack, ableToAttack);

        if (ableToDefend && _defenseActive)
        {
            defending = true;
            _animator.SetBool(_animIDdefending, true);
            CheckDrag(look.action.ReadValue<Vector2>());
            UpdateImage();
            UpdateAnimation();
        }
        else
        {
            defending = false;
            _animator.SetBool(_animIDdefending, false);
        }


        if (attack.action.WasPressedThisFrame())
        {
            if (defending)
            {
                SetParry();
                Attack();
            }
            else
            {
                if (!ableToAttack)
                {
                    Debug.Log("buffer");

                    //initialize attack buffer
                    attackBuffer = true;
                    bufferTimer = 0;
                }
                else
                {
                    Debug.Log("no buffer, just attack");
                    Attack();
                }
            }
        }

        HandleBuffer();
    }

    //Input
    private void OnBlock(InputValue value)
    {
        _defenseActive = value.isPressed;
    }

    //Attack
    private void Attack()
    {
        //initialize attack
        //ableToAttack = false;
        //ableToDefend = false;
        _animator.SetTrigger(_animIDattack);
    }
    private void HandleBuffer()
    {
        if (attackBuffer)
        {
            if (bufferTimer < bufferTimerMax)
            {
                bufferTimer += Time.deltaTime;
            }
            else
            {
                attackBuffer = false;
            }
        }
        _animator.SetBool(_animIDattackBuffer, attackBuffer);
    }

    //Parry direction
    private void SetParry()
    {
        _animator.SetFloat(_animIDdeltaAttackX, deltaX);
        _animator.SetFloat(_animIDdeltaAttackY, deltaY);
    }


    //Defense
    private void CheckDrag(Vector2 look)
    {
        if (look.magnitude > dragThreshold)
        {
            angle = Mathf.Atan2(look.x, -look.y) * Mathf.Rad2Deg;
        }
    }
    private void UpdateImage() 
    {
        if (crossHairImage == null) return;
        crossHairImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 - angle));
    }
    private void UpdateAnimation()
    {
        deltaX = Mathf.Lerp(deltaX, Mathf.Sin(angle * Mathf.Deg2Rad), lerpSpeed * Time.deltaTime);
        deltaY = Mathf.Lerp(deltaY, Mathf.Cos(angle*Mathf.Deg2Rad), lerpSpeed * Time.deltaTime);
        _animator.SetFloat(_animIDdeltaX, deltaX);
        _animator.SetFloat(_animIDdeltaY, deltaY);
    }
    private void SetDefenseLayer(bool enable)
    {
        if (enable && _layerWeight < 1)
        {
            _layerWeight += lerpSpeed * Time.deltaTime;
        }
        else if (!enable && _layerWeight > 0)
        {
            _layerWeight -= lerpSpeed * Time.deltaTime;
        }
        _layerWeight = Mathf.Clamp01(_layerWeight);

        _animator.SetLayerWeight(_animLayerUpperbody, _layerWeight);
    }
    private void SetEvent(bool enable)
    {
        if (enable) onDefense.Invoke();
        else offDefense.Invoke();
    }

    //Animation event, raised during attack animation for attack cancel into defense motion

    public void SetAbleToAttack()
    {
        ableToAttack = true;
        if (attackBuffer) Attack();
    }
    public void ClearBuffer()
    {
        attackBuffer = false;
        ableToAttack = false;
    }

}
