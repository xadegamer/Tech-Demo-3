using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public enum State { Wandering, Combat, Casting, Dead }
    public StatBase StatHandler { get => _statHandler; }
    public MovementHandler MovementHandler { get => _movementHandler; }

    [Header("Handlers")]
    [SerializeField] private MovementHandler _movementHandler;
    [SerializeField] private StatBase _statHandler;

    [Header("Properties")]
    [SerializeField] private CharacterClassSO characterClassSO;
    [SerializeField] private State state;

    private Rigidbody2D rb2D;
    private Animator animator;
    private HealthHandler healthHandler;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        healthHandler = GetComponent<HealthHandler>();
    }

    private void Start()
    {
        healthHandler.SetHealth(characterClassSO.health);
        
        _movementHandler.SetUp(transform, rb2D, animator);

        _statHandler.SetUp(characterClassSO);
    }

    private void Update()
    {
        //_movementHandler.UpdateMovementAndAnimation(InputHandler.Instance.GetMoveVector());
    }

    public void Targetted()
    {
        UIManager.Instance.GetTargetUI().SetUp(characterClassSO.characterIcon);
        healthHandler.OnReceiveDamage.AddListener(OnHealthChanged);
        UIManager.Instance.GetTargetUI().SetHealthBar(healthHandler.GetNormalisedHealth());
    }

    public void UnTargetted()
    {
        healthHandler.OnReceiveDamage.RemoveListener(OnHealthChanged);
    }

    private void OnHealthChanged()
    {
        UIManager.Instance.GetTargetUI().SetHealthBar(healthHandler.GetNormalisedHealth());
    }


    public bool TryUseAbility(AbilitySO abilitySO)
    {
        abilitySO.UseAbility();
        return true;
    }
}
