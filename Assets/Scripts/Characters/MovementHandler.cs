using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    private Vector3 move;
    private bool canRotate = true;

    void Update()
    {
        Walk();
    }

    public void Walk()
    {
        if (MovementJoystick.Instance.IsStickMoving())
        {
            move = MovementJoystick.Instance.Input();
            transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
            Utility.FaceVectorDirection(-move, transform);
        }
    }

    public void PlayerRotation()
    {
        if (!canRotate) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), rotateSpeed * Time.deltaTime);
    }

    public void ToggleRotation(bool newState) => canRotate = newState;

    public void StopMovement() => move = Vector3.zero;
}
