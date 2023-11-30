using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryableProjectile : MonoBehaviour
{
    public GameObject hitEffect;
    [SerializeField] GameObject projectileModel;
    public bool directional;
    public float angle;
    public float dmg =1f;
    public GameObject origin;
    public bool lockedAngle = true;
    
    public void SetAngle(float z)
    {
        angle = z;
    }

    private void Update()
    {
        if (lockedAngle)
        {
            Vector3 modelAngle = projectileModel.transform.eulerAngles;
            modelAngle.z = angle;
            projectileModel.transform.eulerAngles = modelAngle;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        IParryDamageable damage;
        if (collision.gameObject.TryGetComponent<IParryDamageable>(out damage))
        {
            damage.TakeDamage(dmg, angle);
        }
        else if (collision.gameObject.TryGetComponent<Health>(out Health hp))
        {
            hp.Damage(dmg);
            GameObject.Instantiate(hitEffect, collision.transform.position, gameObject.transform.rotation);
        }
    }

    //put on trigger enter on parry collider and if hit this destroy
}
