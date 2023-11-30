using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EnemyDetection : MonoBehaviour
{
    public LayerMask ignoreMask;
    public UnityEvent<GameObject> onDetect;
    public UnityEvent offDetect;
    bool locked;
    GameObject lockedObject;

    bool hit;

    SphereCollider detectionCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            DetectionTarget target = other.gameObject.GetComponentInChildren<DetectionTarget>();
            lockedObject = target.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            lockedObject = null;
            locked = false;
            offDetect.Invoke();
        }
    }

    private void Update()
    {
        if (lockedObject != null)
        {
            Ray ray = new Ray(transform.position, lockedObject.transform.position - transform.position);

            hit = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ~ignoreMask);

            if (hit && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Player") && !locked)
            {
                locked = true;
                onDetect.Invoke(lockedObject);
                Debug.Log("hit and locked");
            }
            else if ((!hit || hitInfo.collider.gameObject.layer != LayerMask.NameToLayer("Player")) && locked)
            {
                locked = false;
                offDetect.Invoke();
                Debug.Log("not hit and unlocked");
            }
        }
    }


    private void Start()
    {
        detectionCollider = GetComponent<SphereCollider>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = locked ? Color.green : Color.red;

        if (lockedObject != null)
        {
            Gizmos.DrawRay(transform.position, lockedObject.transform.position - transform.position);
        }

        if (detectionCollider)
        {
            Gizmos.DrawWireSphere(transform.position + detectionCollider.center, detectionCollider.radius);
        }
    }
}
