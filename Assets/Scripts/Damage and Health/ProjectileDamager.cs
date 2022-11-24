using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Collider2D))]
public class ProjectileDamager : Damager
{
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float rotationSpeed = 1f;

    [SerializeField] UnityEvent<Collider2D> OnCollide;

    [Header("AOE")]
    [SerializeField] private float areaOfEffectSize;
    
    private Rigidbody2D rb;
    private Transform target;
    private Quaternion rotateToTarget;
    private Vector2 dir;

    private void Update()
    {
        LockOnTarget();
    }

    public void SetUp(Transform target)
    {
        this.target = target;
    }

    public void LockOnTarget()
    {
        if (target == null) return;
        dir = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotateToTarget = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateToTarget, Time.deltaTime * rotationSpeed);
        rb.velocity = dir * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (areaOfEffectSize == 0) DealDamage(collision);

            OnCollide?.Invoke(collision);

            if (destroyOnImpact) Destroy(gameObject);
        }
    }

    public void AOEDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, areaOfEffectSize, targetLayer);

        if (colliders.Length != 0)
        {
            foreach (Collider2D collider in colliders)
            {
                DealDamage(collider);
            }
        }
    }

    private void OnDestroy()
    {
        if (areaOfEffectSize != 0) AOEDamage();
    }

    private void OnDrawGizmos()
    {
        if (areaOfEffectSize != 0)
        {
            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawWireSphere(transform.position, areaOfEffectSize);
        }
    }

    public void SetAOESize(float size)
    {
        areaOfEffectSize = size;
    }
}
