using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementHandler 
{
    [SerializeField] private float defaultSpeed;
    private float currentSpeed;
    private Vector2 movementInput;

    private Animator animator;
    private Rigidbody2D rb2D;
    private Transform owner;

    public void SetUp(Transform owner, Rigidbody2D rb2D, Animator animator)
    {
        this.owner = owner;
        this.rb2D = rb2D;
        this.animator = animator;
        currentSpeed = defaultSpeed;
    }
    public void UpdateMovementAndAnimation(Vector2 movementInput)
    {
        this.movementInput = movementInput;
        HandleMovement();
        HandleAnimation();
    }

    public void HandleMovement()
    {
        rb2D.velocity = movementInput * currentSpeed;
        if(movementInput != Vector2.zero) Utility.FaceVectorDirection(movementInput, owner);
    }

    public void HandleAnimation()
    {
        animator.SetFloat("MoveX", Mathf.Abs(movementInput.x));
        animator.SetFloat("MoveY", movementInput.y);
    }
    
    public void StopMovement() => movementInput = Vector2.zero;

    public void SetSpeed(float speed) => currentSpeed = speed;

    public void ResetSpeed() => currentSpeed = defaultSpeed;

    public float GetDefaultSpeed() => defaultSpeed;

    public float GetCurrentSpeed() => currentSpeed;
}
