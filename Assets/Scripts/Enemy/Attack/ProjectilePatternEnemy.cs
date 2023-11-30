using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ProjectilePatternEnemy : EnemyAttackController, IAttack
{
    [Header("Attack Patterns")]
    [SerializeField] private List<AttackPattern> patterns;
    public UnityEvent<float> indicatorStart;
    public UnityEvent indicatorEnd;

    public void SpawnProjectile(Projectile projectile, float zAngle)
    {
        Projectile proj = Instantiate(projectile, transform.position, transform.rotation);
        if (proj.TryGetComponent<ParryableProjectile>(out ParryableProjectile parryable))
        {
            parryable.origin = gameObject;
        }

        ChangeZAngle(proj.gameObject, zAngle);
    }
    public void SpawnTrackingProjectile(TrackingProjectile trackingProjectile, float zAngle, GameObject target)
    {
        TrackingProjectile proj = Instantiate(trackingProjectile, transform.position, transform.rotation);
        proj.SetTarget(target);
        if (proj.TryGetComponent<ParryableProjectile>(out ParryableProjectile parryable))
        {
            parryable.origin = gameObject;
        }

        ChangeZAngle(proj.gameObject, zAngle);
    }

    public void Damage()
    {
        throw new System.NotImplementedException();
    }   
    public override IEnumerator Attacking(GameObject target)
    {
        yield return StartCoroutine(Shooting(target));
    }


    IEnumerator Shooting(GameObject target) 
    {
        for (int i = 0; i< patterns.Count; i++)
        {
            //yield return new WaitForSecondsRealtime(patterns[i].intervalForNext);
            yield return StartCoroutine(HandleIndicatorEvent(patterns[i].indicatorDuration, patterns[i].intervalForNext));

            if (patterns[i].projectile is TrackingProjectile)
            {
                SpawnTrackingProjectile((TrackingProjectile)patterns[i].projectile, patterns[i].zAngle, target);
            }
            else
            {
                SpawnProjectile(patterns[i].projectile, patterns[i].zAngle);
            }
        }
    }

    private void ChangeZAngle(GameObject target, float angle)
    {
        ParryableProjectile parryable;
        if (target.TryGetComponent<ParryableProjectile>(out parryable))
        {
            parryable.SetAngle(angle);
        }
    }

    IEnumerator HandleIndicatorEvent(float indicatorTimer, float intervalTimer)
    {
        if (indicatorTimer < intervalTimer)
        {
            yield return new WaitForSeconds(intervalTimer - indicatorTimer);
            indicatorStart.Invoke(indicatorTimer);
            yield return new WaitForSeconds(indicatorTimer);
            indicatorEnd.Invoke();
        }
        else
        {
            yield return new WaitForSeconds(intervalTimer);
        }
    }

    [Serializable]
    public class AttackPattern
    {
        [SerializeField] public Projectile projectile;
        [SerializeField] public float intervalForNext = 2;
        [SerializeField] public float indicatorDuration = 1f;
        [SerializeField] public float zAngle;
    }
}
