using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPointUI : MonoBehaviour
{
    public Transform followTarget;
    private RectTransform canvasRect; // Reference to the canvas rect.

    private void Start()
    {
        // Get the RectTransform component of the canvas (assuming the canvas is a child of this GameObject).
        canvasRect = GetComponent<RectTransform>();

        if (canvasRect == null)
        {
            Debug.LogError("No RectTransform component found. Make sure the canvas is a child of the GameObject with this script.");
            return;
        }
    }

    private void Update()
    {
        // Convert the world position to screen position.
        Vector3 screenPos = Camera.main.WorldToScreenPoint(followTarget.position);

        // Update the position of the canvas or image.
        canvasRect.position = screenPos;
    }
}
