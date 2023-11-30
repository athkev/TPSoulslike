using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject hitEffect;
    public float dmg = 1f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Health>(out Health hp))
        {
            hp.Damage(dmg);
            if (hitEffect != null) GameObject.Instantiate(hitEffect, other.transform.position, gameObject.transform.rotation * Quaternion.Euler(0,90,0));
        }
        else if (other.gameObject.GetComponentInChildren<Health>())
        {
            other.gameObject.GetComponentInChildren<Health>().Damage(dmg);
        }
    }
}
