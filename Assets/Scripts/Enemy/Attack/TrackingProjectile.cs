using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingProjectile : Projectile
{
    GameObject _target;
    Rigidbody _targetRB;

    [Header("PREDICTION")]
    [SerializeField] private float _maxDistancePredict = 100;
    [SerializeField] private float _minDistancePredict = 5;
    [SerializeField] private float _maxTimePrediction = 5;

    [Header("MOVEMENT")]
    [SerializeField] private float _rotateSpeed = 95;
    Vector3 _prediction;

    public void SetTarget(GameObject target)
    {
        _target = target;
        _targetRB = target.GetComponentInParent<Rigidbody>();
    }

    public override void ProjectileMovement()
    {
        _rb.velocity = transform.forward * _ProjectileSpeed;

        if (_target != null && _target.activeInHierarchy)
        {
            var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.transform.position));
            PredictMovement(leadTimePercentage);
            Rotate();
        }
    }

    private void PredictMovement(float leadTimePercentage)
    {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

        _prediction = _target.transform.position + _targetRB.velocity * predictionTime;
    }

    private void Rotate()
    {
        var heading = _prediction - transform.position;
        var rotation = Quaternion.LookRotation(heading);

        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
    }
}
