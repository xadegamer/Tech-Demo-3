using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar2D : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float scanSize;
    [SerializeField] private int maxScanAtOnce;
    [SerializeField] private LayerMask scanLayer;
    [SerializeField] private Color scanColor = Color.red;

    [Header("Sight Properties")]
    [SerializeField] private LayerMask obstacleLayer;
    public float angle = 30;

    [Header("Checks")]
    [SerializeField] private bool storeTargets = false;
    [SerializeField] private bool searchForTarget = false;
    [SerializeField] private bool checkIfTargetInSite = false;

    [Space]

    [Header("Debug")]
    [SerializeField] private bool targetInRange = false;
    [SerializeField] private bool targetInSite = false;

    [SerializeField] private List<GameObject> objectsInRange = new List<GameObject>();

    [SerializeField] Collider2D[] colliders;

    private int count;


    void Start()
    {
        colliders = new Collider2D[maxScanAtOnce];
    }

    void Update()
    {
        if (!searchForTarget) return;
        Scan();
    }

    public GameObject ClosesetTargetInRange() => transform.FindClosestObject(objectsInRange);
    public GameObject TargetObjectInRange() => colliders[0].gameObject;

    public void ClearTargetList() => objectsInRange.Clear();
    public void SetUp(int size) { scanSize = size; }
    public float GetScanSize() => scanSize; 
    public void ToggleSearch(bool newState) { searchForTarget = newState; }
    public bool TargetInRange() { return targetInRange; }
    public bool TargetInSight() { return targetInSite; }

    public void Scan()
    {
        count = Physics2D.OverlapCircleNonAlloc(transform.position, scanSize, colliders, scanLayer);

        targetInRange = count > 0;

        targetInSite = (checkIfTargetInSite && targetInRange) ? IsInSight(colliders[0]) : false;

        if (storeTargets) StoreTargets();
    }

    bool IsInSight(Collider2D targetCol)
    {
        Vector3 directionToTarget = Vector3.Normalize(targetCol.bounds.center - transform.position);
        float angleToTarget = Vector3.Angle(transform.right, directionToTarget);

        if(angleToTarget < angle)
        {
            if(!Physics.Linecast(transform.position, targetCol.bounds.center, out RaycastHit hit, obstacleLayer))
            {
                Debug.DrawLine(transform.position, targetCol.bounds.center, Color.red);
                return true;
            }
            else
            {
                Debug.DrawLine(transform.position, hit.point, Color.green);
            }
        }
        return false;
    }

    void StoreTargets()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            objectsInRange.Add(obj);
        }
    }

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
