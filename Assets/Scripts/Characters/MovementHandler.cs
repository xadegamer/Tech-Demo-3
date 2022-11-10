using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    [SerializeField] private Animator animator;

    private Vector2 movementInput;
    private bool canRotate = true;
    private Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Walk();
        HandleAnimation();

        HandleAttack();
    }

    public void Walk()
    {
        movementInput = InputHandler.Instance.GetMoveVector();
        rb2D.velocity = movementInput * moveSpeed;
        if(movementInput != Vector2.zero) Utility.FaceVectorDirection(movementInput, transform);
    }

    public void HandleAnimation()
    {
        animator.SetFloat("MoveX", Mathf.Abs(movementInput.x));
        animator.SetFloat("MoveY", movementInput.y);
    }

    public void PlayerRotation()
    {
        if (!canRotate) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementInput), rotateSpeed * Time.deltaTime);
    }

    public void ToggleRotation(bool newState) => canRotate = newState;

    public void StopMovement() => movementInput = Vector2.zero;

    public void HandleAttack()
    {
        if (InputHandler.Instance.GetAttackInput().WasPressedThisFrame())
        {
            Debug.Log("Attack");
        }
    }
}
