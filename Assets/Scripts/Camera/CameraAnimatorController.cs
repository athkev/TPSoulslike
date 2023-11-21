using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class CameraAnimatorController : MonoBehaviour
{
    public UnityEvent<Quaternion> onChangeCamera;
    Animator cameraAnimator;
    private int _animID;
    private int _animIDtrigger;

    int id;

    void Start()
    {
        cameraAnimator = GetComponent<Animator>();
        _animID = Animator.StringToHash("ID");
        _animIDtrigger = Animator.StringToHash("ChangeTrigger");
    }

    public void ChangeCamera(int id)
    {
        if (this.id == id) return;
        onChangeCamera.Invoke(Camera.main.transform.rotation);
        cameraAnimator.SetInteger(_animID, id);
        cameraAnimator.SetTrigger(_animIDtrigger);
        this.id = id;
    }
}
