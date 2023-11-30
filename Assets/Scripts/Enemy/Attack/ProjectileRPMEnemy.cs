using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRPMEnemy : EnemyAttackController, IAttack
{
    [Header("Projectile Config")]
    [SerializeField] Projectile projectile;

    private int current_n_projectiles;
    public int n_projectiles;
    public float rpm;
    public void SpawnProjectile(Vector3 direction)
    {
        Projectile proj = Instantiate(projectile);
        proj.transform.rotation = Quaternion.LookRotation(direction);
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
        float rofInSeconds = 1.0f / (rpm / 60.0f);
        while (current_n_projectiles < n_projectiles)
        {
            SpawnProjectile(target.transform.position - transform.position);
            yield return new WaitForSeconds(rpm);
            current_n_projectiles++;
        }
    }
}
