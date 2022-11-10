using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    [SerializeField] private Animator animator;

    private Vector3 move;
    private bool canRotate = true;

    void Update()
    {
        Walk();
        HandleAnimation();
    }

    public void Walk()
    {
        if (MovementJoystick.Instance.IsDragingStick())
        {
            move = MovementJoystick.Instance.Input();
            transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
            Utility.FaceVectorDirection(move, transform);
        }
        else move = Vector3.zero;
    }

    public void HandleAnimation()
    {
        animator.SetFloat("MoveX", Mathf.Abs(move.x));
        animator.SetFloat("MoveY", move.y);
    }

    public void PlayerRotation()
    {
        if (!canRotate) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), rotateSpeed * Time.deltaTime);
    }

    public void ToggleRotation(bool newState) => canRotate = newState;

    public void StopMovement() => move = Vector3.zero;
}
