using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar2D : MonoBehaviour
{  
    [Header("Properties")]
    [SerializeField] private float scanSize;
    [SerializeField] private int maxScanCount;
    [SerializeField] private LayerMask scanLayer;

    [Header("Sight Properties")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float angle = 30;
    
    [Header("Visual")]
    [SerializeField] private Color scanColor = Color.red;

    [Header("Debug")]
    [SerializeField] private bool targetInRange = false;
    [SerializeField] private bool targetInSite = false;
    [SerializeField] private Collider2D[] colliders;
    [SerializeField] private int count;
    [SerializeField] private bool obstacleInSight;
    [SerializeField] private bool clearPastTargets;

    void Start() => colliders = new Collider2D[maxScanCount];

    private void Update() => Scan();

    public void Scan()
    {
        count = Physics2D.OverlapCircleNonAlloc(transform.position, scanSize, colliders, scanLayer);
        targetInRange = count > 0;

        if (targetInRange && clearPastTargets)clearPastTargets = false;
        if (!targetInRange && !clearPastTargets) ClearTargets();
        
        targetInSite = targetInRange ? IsInSight(colliders[0]) : false;
    }

    public bool SearchForTarget()
    {
        Scan();
        return targetInRange;
    }

    public bool IsInSight(Collider2D targetCol)
    {
        Vector3 directionToTarget = Vector3.Normalize(targetCol.bounds.center - transform.position);
        float angleToTarget = Vector3.Angle(transform.right, directionToTarget);

        if(angleToTarget < angle)
        {
            obstacleInSight = Physics.Linecast(transform.position, targetCol.bounds.center, out RaycastHit hit, obstacleLayer);
            Debug.DrawLine(transform.position, obstacleInSight ? hit.point : targetCol.bounds.center, Color.red);
            return !obstacleInSight;
        }
        return false;
    }

    public void ClearTargets()
    {
        clearPastTargets = true;
        for (int i = 0; i < colliders.Length; i++) colliders[i] = null;
    }

    public Transform TargetObjectInRange() => colliders[0].gameObject.transform;
    public void SetScanSize(int size) => scanSize = size;
    public float GetScanSize() => scanSize;
    public bool TargetInRange() => targetInRange;
    public bool TargetInSight() => targetInSite;

    private void OnDrawGizmos()
    {
        Gizmos.color = targetInRange ? Color.red : scanColor ;

        Gizmos.DrawWireSphere(transform.position, scanSize);

        Vector3 rightDirection = Quaternion.Euler(0, 0, angle) * transform.right;
        Gizmos.DrawRay(transform.position, rightDirection * scanSize);

        Vector3 leftDirection = Quaternion.Euler(0, 0, -angle) * transform.right;
        Gizmos.DrawRay(transform.position, leftDirection * scanSize);
    }
}
