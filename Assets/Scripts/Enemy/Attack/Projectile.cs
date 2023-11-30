using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]public Rigidbody _rb;
    public float projectileLifeTime;
    [SerializeField] public float _ProjectileSpeed;
    [SerializeField] GameObject _HitEffect;
    private float timer = 0f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 60);
    }
    void Update()
    {
        ProjectileMovement();
        HandleExpiredProjectile();
    }

    public virtual void ProjectileMovement()
    {
        Vector3 delta = (transform.forward * _ProjectileSpeed * Time.deltaTime);
        transform.position += delta;
    }

    private void HandleExpiredProjectile()
    {
        timer += Time.deltaTime;
        if (timer > projectileLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Hit(collision);
    }
    void Hit(Collision collision)
    {
        if (_HitEffect != null)
        {
            GameObject hit = Instantiate(_HitEffect, collision.contacts[0].point, Quaternion.identity);
            hit.transform.forward = collision.contacts[0].normal;
        }
        Destroy(gameObject);
    }
}
public static class LayerMaskExtensions
{
    public static bool ContainsLayer(this LayerMask layerMask, int layer)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}