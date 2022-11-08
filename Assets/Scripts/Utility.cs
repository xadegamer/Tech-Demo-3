using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static void FaceVelocityDirection(Rigidbody2D rigidbody2D)
    {
        rigidbody2D.transform.localScale = new Vector2(rigidbody2D.velocity.x > 0 ? 1 : -1, 1);
    }
    public static int ScaleToDirection(float scale)
    {
        return scale > 0 ? 1 : -1;
    }

    public static int VectorToDirection(Vector3 vector3)
    {
        return vector3.x > 0 ? 1 : -1;
    }

    public static void FaceVectorDirection(Vector3 vector3, Transform transform)
    {
        transform.localScale = new Vector2(vector3.x > 0 ? 1 : -1, 1);
    }

    public static GameObject FindClosestObject(this Transform ownerPos, List<GameObject> gameObjects)
    {
        float distanceToClosestObject = Mathf.Infinity;
        GameObject closestObject = null;

        foreach (GameObject currentObject in gameObjects)
        {
            float distanceToEnemy = (currentObject.transform.position - ownerPos.position).sqrMagnitude;
            if (distanceToEnemy < distanceToClosestObject)
            {
                distanceToClosestObject = distanceToEnemy;
                closestObject = currentObject;
            }
        }

        return closestObject;
    }

    public static bool ObstacleDetectedRaycast(Transform startTransfrom, float normalisedDirection, float distanceCheck, LayerMask detectLayer, bool showDebug = true)
    {
        if (showDebug) Debug.DrawRay(startTransfrom.position, new Vector2(normalisedDirection * distanceCheck, 0), Color.yellow);

        RaycastHit2D hit = Physics2D.Raycast(startTransfrom.position, new Vector2(normalisedDirection, 0), distanceCheck, detectLayer);

        if (hit.collider != null)
        {
            if (showDebug) Debug.DrawLine(startTransfrom.position, hit.point, Color.red);
            return true;
        }
        else return false;
    }

    public static bool ObstacleDetectedBoxcast(Collider2D collider2D, float normalisedDirection, float distanceCheck, LayerMask detectLayer, bool showDebug = true)
    {
        if (showDebug) DrawBoxcast2D(collider2D.bounds.center, collider2D.bounds.size, normalisedDirection, distanceCheck, Color.yellow);

        RaycastHit2D hit = Physics2D.BoxCast(collider2D.bounds.center, collider2D.bounds.size, 0f, new Vector2(normalisedDirection, 0), distanceCheck, detectLayer);

        if (hit.collider != null)
        {
            if (showDebug) DrawBoxcast2D(collider2D.bounds.center, collider2D.bounds.size, normalisedDirection, distanceCheck, Color.red);
            return true;
        }
        else return false;
    }

    public static bool TargetInRange(this Transform self, Transform target, float distance)
    {
        return Vector2.Distance(self.transform.position, target.position) <= distance;
    }

    public static bool TargetInRangOnX(this Transform self, Transform target, float distance)
    {
        return Mathf.Abs(self.transform.position.x - target.transform.position.x) <= distance;
    }

    public static float GetNormalisedDirection(Vector2 start, Vector2 target)
    {
        return -(start - target).normalized.x > 0 ? 1 : -1;
    }

    public static void MoveToPosition(Rigidbody2D rigidbody2D, Vector2 targetPostion, float moveSpeed)
    {
        Vector2 target = new Vector2(targetPostion.x, rigidbody2D.position.y);
        Vector2 newPos = Vector2.MoveTowards(rigidbody2D.position, target, moveSpeed * Time.fixedDeltaTime);
        rigidbody2D.MovePosition(newPos);
    }

    public static void MoveToDirection(Rigidbody2D rigidbody2D, float direction, float moveSpeed)
    {
        rigidbody2D.velocity = new Vector2(direction * moveSpeed, rigidbody2D.velocity.y);
    }

    public static void LookAt2D(this Transform self, Transform target)
    {
        Vector2 direction = (self.position - target.position).normalized;
        int newX = Mathf.RoundToInt(direction.x);
        if (newX == 0) newX = 1;
        self.localScale = new Vector3(newX, 1, 1);
    }

    public static void Jump(Rigidbody2D rigidbody2D, float jumpForce)
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
    }

    public static IEnumerator KnockBack(Rigidbody2D rigidbody2D, float knockDownDuration, float knockDownPower, Transform obj)
    {
        float timer = 0;

        while (knockDownDuration > timer)
        {
            timer += Time.deltaTime;
            Vector2 direction = (obj.position - rigidbody2D.transform.position).normalized;
            rigidbody2D.AddForce(-direction * knockDownPower, ForceMode2D.Impulse);
        }
        yield return null;
    }

    public static IEnumerator KnockBack(this Rigidbody2D rigidbody2D, Vector2 direction, float force, float duration, Action OnBegin = null, Action OnEnd = null)
    {
        OnBegin?.Invoke();
        rigidbody2D.AddForce(direction * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        rigidbody2D.velocity = Vector2.zero;
        OnEnd?.Invoke();
    }

    public static IEnumerator AddVelocity(this Rigidbody2D rigidbody2D, Vector2 direction, float force, float duration, Action OnBegin = null, Action OnEnd = null)
    {
        OnBegin?.Invoke();

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            rigidbody2D.AddForce(direction * force, ForceMode2D.Impulse);
            yield return null;
        }
        OnEnd?.Invoke();
    }


    public static IEnumerator LerpPosition(this Transform transform, Vector3 targetPosition, float duration, Action OnStart, Action OnFinish)
    {
        if (OnStart != null) OnStart.Invoke();
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        yield return null;
        if (OnFinish != null) OnFinish.Invoke();
    }

    public static IEnumerator AnimateMove(this Transform transform, AnimationCurve dashCurve, Vector3 target, float duration, Action OnStart, Action OnFinish)
    {
        if (OnStart != null) OnStart.Invoke();
        Vector3 startPosition = transform.position;
        float journey = 0f;
        while (journey <= duration)
        {
            journey = journey + Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);

            float curvePercent = dashCurve.Evaluate(percent);
            transform.position = Vector3.LerpUnclamped(startPosition, target, curvePercent);

            yield return null;
        }
        transform.position = target;

        if (OnFinish != null) OnFinish.Invoke();
    }



    public static void DrawBoxcast2D(Vector2 position, Vector2 size, float direction, float distance, Color color)
    {
        Vector2 directionVector = new Vector2(direction, 0);
        Vector2 origin = position + (directionVector * size.x * 0.5f);
        Vector2 sizeVector = new Vector2(size.x, size.y);
        Debug.DrawRay(origin, directionVector * distance, color);
        Debug.DrawRay(origin + (Vector2.up * sizeVector.y * 0.5f), directionVector * distance, color);
        Debug.DrawRay(origin + (Vector2.down * sizeVector.y * 0.5f), directionVector * distance, color);
        Debug.DrawRay(origin + (Vector2.up * sizeVector.y * 0.5f) + (directionVector * distance), Vector2.down * sizeVector.y, color);
        Debug.DrawRay(origin + (Vector2.down * sizeVector.y * 0.5f) + (directionVector * distance), Vector2.up * sizeVector.y, color);
    }

    public static void DebugDrawBox(Vector2 center, Vector2 size, Color color)
    {
        float halfWidth = size.x / 2f;
        float halfHeight = size.y / 2f;
        Debug.DrawLine(new Vector3(center.x + halfWidth, center.y - halfHeight, 0), new Vector3(center.x + halfWidth, center.y + halfHeight, 0), color);
        Debug.DrawLine(new Vector3(center.x - halfWidth, center.y - halfHeight, 0), new Vector3(center.x - halfWidth, center.y + halfHeight, 0), color);
        Debug.DrawLine(new Vector3(center.x - halfWidth, center.y + halfHeight, 0), new Vector3(center.x + halfWidth, center.y + halfHeight, 0), color);
        Debug.DrawLine(new Vector3(center.x - halfWidth, center.y - halfHeight, 0), new Vector3(center.x + halfWidth, center.y - halfHeight, 0), color);
    }
}
