using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    public float initialCoolDown = 2f;
    public float coolDown = 3f;
    bool active;
    GameObject target;
    Coroutine attackCoroutine;
    private void Start()
    {
        
    }
    private void OnDisable()
    {
        StopAttack();
    }
    public void StartAttack(GameObject target)
    {
        active = true;
        this.target = target;
        attackCoroutine = StartCoroutine(AttackCycle());
    }
    public void StopAttack()
    {
        active = false;
        target = null;
        StopAllCoroutines();
    }
    public virtual IEnumerator Attacking(GameObject target)
    {
        yield return new WaitForSeconds(coolDown);
    }
    IEnumerator AttackCycle()
    {
        yield return new WaitForSeconds(initialCoolDown);

        while (active)
        {
            StartCoroutine(Attacking(target));
            yield return new WaitForSeconds(coolDown);
        }
    }
}
