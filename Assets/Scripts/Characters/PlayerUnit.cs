using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerUnit : GameUnit
{
    public static PlayerUnit Instance { get; private set; }
    public PlayerStatHandler PlayerStatHandler { get => _playerStatHandler; }
    public MovementHandler MovementHandler { get => _movementHandler; }

    public Action<bool> OnTargetFound;

    [Header("Player Handlers")]
    [SerializeField] private MovementHandler _movementHandler;
    [SerializeField] private PlayerStatHandler _playerStatHandler;
    [SerializeField] private LayerMask tapDetectlayer;


private GameUnitBuffController gameUnitBuffController;
    private RaycastHit2D hitInfo;
    private Rigidbody2D rb2D;

    private GameUnit killer;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        rb2D = GetComponent<Rigidbody2D>();
        gameUnitBuffController = GetComponent<GameUnitBuffController>();
    }

    protected override void Start()
    {
        base.Start();

        _movementHandler.SetUp(transform, rb2D, animator);

        UIManager.Instance.GetPlayerrUI().SetUpPlayerUI(this);

        _playerStatHandler.GetManaValue().OnValueChanged += StatHandler_OnManaChanged;
        _playerStatHandler.SetUp(characterClassSO);
    }

    protected override void Update()
    {
        base.Update();
        _playerStatHandler.GetManaValue().ValueRegeneration();
    }

    public override void HandleMovement()
    {
        _movementHandler.UpdateMovementAndAnimation(InputHandler.Instance.GetMoveVector());

        _movementHandler.ToggleFaceMovementDir(!target);

        if (target) Utility.LookAtPosition(transform, target.transform.position);

        if (target) animator.SetFloat("MoveY", (target.transform.position.y - transform.position.y));

        HandleMeleeAbilities();
    }

    protected override void OnHealthChanged(float normalisedValue)
    {
        base.OnHealthChanged(normalisedValue);
        UIManager.Instance.GetPlayerrUI().SetHealthBar(healthHandler.GetNormalisedHealth());
    }

    protected override void OnDeath(DamageInfo arg0)
    {
        base.OnDeath(arg0);
        _movementHandler.StopMovement();
        GetComponent<Collider2D>().enabled = false;

        killer = arg0.owner;
        killer.GetComponent<GameUnit>().HealthHandler.ResetHealth();
        GameManager.Instance.RespawnPlayer();
    }

    public override bool TryUseAbility(AbilitySO abilitySO)
    {
        float cost = abilitySO.GetAbilityCost(_playerStatHandler.GetCharacterClassSO().mana);

        if (_playerStatHandler.GetManaValue().GetCurrentValue() >= cost)
        {
            _playerStatHandler.GetManaValue().ReduceValue(cost);
            abilitySO.UseAbility();
            return true;
        }
        
        return false;
    }

    public override bool TrySetTarget(Transform clickedObject)
    {
        if (clickedObject.TryGetComponent(out GameUnit gameUnit))
        {
            if (target == clickedObject) return false;

            if (target is EnemyUnit lastEnemyUnit) lastEnemyUnit.Targetted(false);

            AbilityUIManager.Instance.ToggleActive(gameUnit is EnemyUnit);

            Targetter.SetTarget(gameUnit);

            if (gameUnit is EnemyUnit enemyUnit)
            {
                state = State.Targetting;
                SetTarget(enemyUnit);
                enemyUnit.Targetted(true);
                return true;
            }

            if (gameUnit is PlayerUnit self)
            {
                SetTarget(null);
                return true;
            }
        }

        return false;
    }

    public override void Targetted(bool status)
    {
        base.Targetted(status);
    }

    public override void HandleCombat()
    {
        TryTargetTouchedEnemy();
        
        if (InputHandler.Instance.GetAttackInput().WasPressedThisFrame() && target) ChangeState(State.Combat);

        AttackTimer(PlayerStatHandler);
    }

    public void TryTargetTouchedEnemy()
    {
        if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Moved) hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector3.forward);
        else if (Input.GetMouseButtonDown(0)) hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward, tapDetectlayer);
        else return;

        if (hitInfo.collider != null)
        {
            Debug.Log("Hit: " + hitInfo.collider.name);
            TrySetTarget(hitInfo.collider.transform);
        }
    }

    void StatHandler_OnManaChanged(object sender, float value)
    {
        UIManager.Instance.GetPlayerrUI().SetManaBar(value);
    }

    public void HandleMeleeAbilities()
    {
        AbilityUIManager.Instance.ToggleMeleeAbilities(target != null && attackRadar.TargetInRange());
    }

    public override void StartStun()
    {
        base.StartStun();
        _movementHandler.StopMovement();
    }

    public override StatBase GetStat()
    {
        return _playerStatHandler;
    }

    public override void SetTarget(GameUnit newTarget)
    {
        base.SetTarget(newTarget);
        OnTargetFound?.Invoke(newTarget);
    }

    public void Respawn(Vector2 startPos)
    {
        gameUnitBuffController.RemoveBuffs();
        transform.position = startPos;
        healthHandler.SetHealth(characterClassSO.health);
        _playerStatHandler.SetUp(characterClassSO);

        animator.Play("Idle_Buttom");


        SetTarget(null);
        killer.Targetted(false);
        killer.SetTarget(null);

        GetComponent<Collider2D>().enabled = true;
        state = State.Wandering;
    }

    public override void CanMove(bool status)
    {
        base.CanMove(status);
    }

    public override void Attack()
    {
        animator.SetTrigger("Melee");
    }
}
