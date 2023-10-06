using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScrollZoom : MonoBehaviour
{
    CinemachineVirtualCamera _virtualCamera;
    float scroll = 5.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    private void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        HandleScroll();
    }

    private void HandleScroll()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        scroll -= scrollInput;
        scroll = Mathf.Clamp(scroll, minDistance, maxDistance);

        CinemachineComponentBase componentBase = _virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is Cinemachine3rdPersonFollow)
        {
            (componentBase as Cinemachine3rdPersonFollow).CameraDistance = scroll;
        }
    }
}