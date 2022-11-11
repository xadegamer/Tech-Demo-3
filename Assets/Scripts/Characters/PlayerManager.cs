using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public StatHandler StatHandler { get => _statHandler; }
    public MovementHandler MovementHandler { get => _movementHandler; }


    [SerializeField] private CharacterClassSO characterClassSO;
    [SerializeField] private MovementHandler _movementHandler;
    [SerializeField] private StatHandler _statHandler;
   
    private Rigidbody2D rb2D;
    private Animator animator;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _movementHandler.SetUp(transform, rb2D, animator);
        _statHandler.SetUp(characterClassSO);
    }

    private void Update()
    {
        _movementHandler.UpdateMovementAndAnimation(InputHandler.Instance.GetMoveVector());
        _statHandler.Update();
    }
}
