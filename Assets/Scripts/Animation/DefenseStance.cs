using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;
//while this script is active,
//delta X and Y of camera movement will be sent to animator parameter for parry stance blendtree
public class DefenseStance : MonoBehaviour
{
    ThirdPersonController tpsController;
    bool _defenseActive;
    Animator _animator;
    InputManager _input;
    public Image crossHairImage;
    private int _animIDdeltaX;
    private int _animIDdeltaY;

    private int _animLayerUpperbody;
    private float _layerWeight = 0;

    private float deltaX;
    private float deltaY;

    private float angle;
    private float imageAngle;
    [SerializeField] float dragThreshold = .2f;
    [SerializeField] float lerpSpeed = .1f;

    public UnityEvent onDefense = new UnityEvent();
    public UnityEvent offDefense = new UnityEvent();

    private void OnEnable()
    {
        if (!TryGetComponent<Animator>(out _animator))
        {
            Debug.Log("NO ANIMATOR SET ON DEFENSESTANCE");
        }
        if (!TryGetComponent<InputManager>(out _input))
        {
            Debug.Log("NO INPUT ON DEFENSESTANCE");
        }
        if (!TryGetComponent<ThirdPersonController>(out tpsController))
        {
            Debug.Log("No tps controller");
        }
    }
    private void Start()
    {
        _animIDdeltaX = Animator.StringToHash("DragX");
        _animIDdeltaY = Animator.StringToHash("DragY");
        _animLayerUpperbody = _animator.GetLayerIndex("Strafe_UpperSword");
    }

    private void Update()
    {
        SetDefenseLayer(_defenseActive);
        SetEvent(_defenseActive);

        if (_defenseActive)
        {
            CheckDrag(_input.look);
            UpdateImage();
            UpdateAnimation();
        }
    }
    private void OnBlock(InputValue value)
    {
        _defenseActive = value.isPressed;
    }
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
        imageAngle = 180 - angle;
        crossHairImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, imageAngle));
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

}
