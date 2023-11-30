using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    GameObject target;
    public float rotationSpeed = 5f;

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
    public void Stop()
    {
        target = null;
    }

    void Update()
    {
        if (target != null)
        {
            RotateTowardsTarget();
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 directionToTarget = target.transform.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

}
