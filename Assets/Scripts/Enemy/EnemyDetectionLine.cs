using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    GameObject target;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
    }

    public void DetectPlayer(GameObject player)
    {
        lineRenderer.enabled = true;
        target = player;
    }

    private void Update()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, target.transform.position);
        }
    }

    public void OffDetect()
    {
        lineRenderer.enabled = false;
    }

}
