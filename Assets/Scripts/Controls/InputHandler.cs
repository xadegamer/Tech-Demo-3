using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    private InputHandlerAction inputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        inputActions = new InputHandlerAction();
        inputActions.Player.Enable();
    }

    public Vector2 GetMoveVector()
    {
        return inputActions.Player.Movement.ReadValue<Vector2>();
    }

    public InputAction GetAttackInput()
    {
        return inputActions.Player.Attack;
    }
}
