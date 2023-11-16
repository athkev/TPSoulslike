using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class FreeLookController : MonoBehaviour
{
    CinemachineFreeLook freeLook;
    [SerializeField] InputActionReference XYAxis;
    public float sensitivity = 2f;

    private Vector2 xy;

    void Start()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        xy = XYAxis.action.ReadValue<Vector2>();

        freeLook.m_XAxis.Value += xy.x * sensitivity;
        freeLook.m_YAxis.Value += xy.y * sensitivity * 1/180;
    }
}
