using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private InputActionReference XYAxis;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float maxVerticalAngle = 80f;

    private Vector2 currentRotation = Vector2.zero;

    private void OnEnable()
    {
        XYAxis.action.Enable();
    }

    private void OnDisable()
    {
        XYAxis.action.Disable();
    }

    public void SetRotation()
    {
        currentRotation = new Vector2(Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y);
    }

    private void Update()
    {
        Vector2 input = XYAxis.action.ReadValue<Vector2>();

        // Update the vertical rotation (pitch)
        currentRotation.x += input.y * sensitivity;
        currentRotation.x = ClampAngle(currentRotation.x, -maxVerticalAngle, maxVerticalAngle);

        // Update the horizontal rotation (yaw)
        currentRotation.y += input.x * sensitivity;

        // Apply rotation to the transform
        transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        float start = (min + max) * 0.5f - 180;
        float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
        return Mathf.Clamp(angle, min + floor, max + floor);
    }
}
