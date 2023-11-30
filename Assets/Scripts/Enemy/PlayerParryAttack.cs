using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParryAttack : MonoBehaviour
{
    Collider c;
    public float colliderUpDuration = .33f;
    public float dmg = 1f;
    public TrackingProjectile deflectedProjectilePrefab;
    private void OnEnable()
    {
        c = GetComponent<Collider>();
        StartCoroutine(HandleCollider());
    }

    IEnumerator HandleCollider()
    {
        c.enabled = true;
        yield return new WaitForSecondsRealtime(colliderUpDuration);
        c.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Health>(out Health hp))
        {
            hp.Damage(dmg);
        }

        ParryableProjectile parryable = other.GetComponentInParent<ParryableProjectile>();
        if (parryable != null)
        {
            //check if the parry was successful with the angle
            //then instantiate the prefab
            //then assign the target tracking to the parryable.origin

            if (DefenseStance.Instance.CalculateSuccessfulBlock(parryable.angle))
            {
                GameObject origin = parryable.origin;
                GameObject.Destroy(parryable.gameObject);
                SpawnParryProjectile(origin);
            }
        }
    }

    private void SpawnParryProjectile(GameObject target)
    {
        Debug.Log("Parry success!");
        TrackingProjectile projectile = GameObject.Instantiate(deflectedProjectilePrefab, transform.position, transform.rotation);
        Debug.Log("Parried towards: " + target);
        if (target != null) projectile.SetTarget(target);
    }
}
