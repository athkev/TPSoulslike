using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockAway : MonoBehaviour
{
    Rigidbody rb;
    public float knockbackForce = 10f;
    public float upwardForce = 5f;
    public float rotationSpeed = 500f;
    public float disableTime = 3f; // Time in seconds before disabling the object

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Knockback()
    {
        if (gameObject.activeInHierarchy)
        {
            if (rb.isKinematic) rb.isKinematic = false;
            rb.useGravity = true;
            // Apply knockback force
            rb.AddForce(-transform.forward * knockbackForce, ForceMode.Impulse);

            // Apply upward force
            rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);

            // Rotate rapidly in the X-axis
           rb.angularVelocity = new Vector3(rotationSpeed, 0, 0);

            // Disable the object after a certain amount of time
            Invoke("DisableObject", disableTime);
        }
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
