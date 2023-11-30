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
public class DefenseStance : MonoBehaviour, IParryDamageable
{
    private static DefenseStance _instance;
    public static DefenseStance Instance
    {
        get
        {
            if (_instance == null) { Debug.Log("no defense stance controller"); return null; }
            return _instance;
        }
    }
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
    private int _animIDspecialAttack;
    private int _animIDHit;
    private int _animIDBlocked;

    private int _animLayerUpperbody;
    private float _layerWeight = 0;

    private float deltaX;
    private float deltaY;

    private float angle;

    [SerializeField] float dragThreshold = .2f;
    [SerializeField] float lerpSpeed = .1f;

    public UnityEvent onDefense = new UnityEvent();
    public UnityEvent offDefense = new UnityEvent();

    public UnityEvent onSpecialAttack = new UnityEvent();
    public UnityEvent onEndSpecialAttack = new UnityEvent();

    public UnityEvent<float> onTakeDamage = new UnityEvent<float>();


    bool ableToDefend = true;
    bool ableToAttack = true;
    bool attackBuffer;
    float bufferTimer;
    float bufferTimerMax = .3f;

    bool defending;
    bool specialAttacking;

    public int currentSpecialCounter = 1;
    public int maxSpecialCounter = 1;

    private void Awake()
    {
        _instance = this;
    }
    private void OnEnable()
    {
        _animator = GetComponentInParent<Animator>();
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
        _animIDspecialAttack = Animator.StringToHash("SpecialAttack");

        _animIDBlocked = Animator.StringToHash("Blocked");
        _animIDHit = Animator.StringToHash("GotHit");
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
    private void OnRestart(InputValue value)
    {
        FinishManager.Instance.RestartLevel();
    }
    private void OnBlock(InputValue value)
    {
        _defenseActive = value.isPressed;
    }

    private void OnSpecialAttack(InputValue value)
    {
        if (defending && currentSpecialCounter > 0)
        {
            onSpecialAttack.Invoke();
            _animator.SetTrigger(_animIDspecialAttack);
            _animator.SetBool(_animIDattackBuffer, true);
            specialAttacking = true;

            currentSpecialCounter--;
            //character 
        }
    }
    //attach to end of special attack so off-defense can be invoked
    public void EndSpecialAttack()
    {
        if (!specialAttacking) return;
        specialAttacking = false;
        onEndSpecialAttack.Invoke();
        _animator.SetBool(_animIDattackBuffer, false);
    }
    public void AddSpecialAttackCounter(int count)
    {
        currentSpecialCounter += count;
        if (currentSpecialCounter > maxSpecialCounter) currentSpecialCounter = maxSpecialCounter;
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
        if (!enable && !specialAttacking) offDefense.Invoke();
        else onDefense.Invoke();
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

    [Header("Parry")]
    public float blockAngleThreshold = 15f;
    public void TakeDamage(float damage, float angle)
    {
        if (defending && CalculateSuccessfulBlock(angle))
        {
            //no dmg
            Debug.Log("successful block");
            _animator.SetTrigger(_animIDBlocked);
        }
        else if (defending)
        {
            //reduced dmg
            Debug.Log("missed block");
            _animator.SetTrigger(_animIDHit);
            onTakeDamage.Invoke(damage * .8f);
        }
        else if (!defending)
        {
            //full dmg
            Debug.Log("took dmg");
            _animator.SetTrigger(_animIDHit);
            onTakeDamage.Invoke(damage * 1f);
        }

    }

    public bool CalculateSuccessfulBlock(float projAngle)
    {
        float blockAngle = ((this.angle - 90) % 360 + 360) % 360;
        float projAngleA = (projAngle % 360 + 360) % 360;
        float projAngleB = ((projAngle + 180) % 360 + 360) % 360;

        float deltaA = Mathf.Abs(Mathf.DeltaAngle(blockAngle, projAngleA));
        float deltaB = Mathf.Abs(Mathf.DeltaAngle(blockAngle, projAngleB));

        //Debug.Log("Block angle: " + blockAngle + "         Proj angle: " + projAngleA + " annnnd : "+projAngleB);
        //Debug.Log("Dif A : " + deltaA + "                 Dif B : " + deltaB);

        return deltaA < blockAngleThreshold || deltaB < blockAngleThreshold;
    }
}
