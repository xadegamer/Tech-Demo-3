using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamager : Damager
{
	public enum HitShape { Circle, Box }

    public enum DamageType { Instant, Constant}

    [SerializeField] private Color color = Color.red;

    [Tooltip("Set the damage type")]
    [SerializeField] private DamageType damageType;

    [Tooltip("Set the damage hitShape")]
    [SerializeField] private HitShape hitShape;

    [Header("Circle Damager")]
	[SerializeField] private float attackRange = 1f;

    [Header("Box Damager")]
	[SerializeField] private Vector3 size;
    [SerializeField] private Vector3 Offset;

    [Header("DamageType Properties")]
    [SerializeField] private float constantDamageDelay;

    float timer;

	private void OnEnable()
	{
        timer = 0;
    }

	private void Update()
	{
		if(damageType == DamageType.Constant)
		{
            if (timer <= 0)
            {
				Attack();
                timer = constantDamageDelay;
            }
            else timer -= Time.deltaTime;
        }
    }

	public void Attack()
	{
		if (hitShape == HitShape.Circle)
		{
			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);
			if (colliders.Length != 0)
			{
				foreach (Collider2D collider in colliders)
                {
                    DealDamage(collider);
                }
			}
		}
		else if (hitShape == HitShape.Box)
		{
			Vector3 colliderPos = transform.position + Offset;

			Collider2D[] colliders = Physics2D.OverlapBoxAll(colliderPos, new Vector2 (transform.localScale.x * size.x,transform.localScale.y * size.y), 0f, targetLayer);

			if (colliders.Length != 0)
			{
				foreach (Collider2D collider in colliders)
				{
					if (collider.TryGetComponent(out HealthHandler healthSystem))
					{
						DealDamage(collider);
					}
				}
			}
		}
        if (destroyOnImpact) Destroy(gameObject);
    }

	public void SetAttackRange(float attackRange)
	{
		this.attackRange = attackRange;
	}
    
    public virtual void OnDrawGizmos()
    {
        if (hitShape == HitShape.Circle)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        else if (hitShape == HitShape.Box)
        {
            Gizmos.color = color;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 pos = Offset;
            Gizmos.DrawWireCube(pos, size);
        }
    }
}
