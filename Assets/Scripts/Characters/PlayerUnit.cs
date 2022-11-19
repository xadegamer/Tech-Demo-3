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

    [Header("Player Handlers")]
    [SerializeField] private MovementHandler _movementHandler;
    [SerializeField] private PlayerStatHandler _playerStatHandler;
    [SerializeField] private GameUnitBuffController gameUnitBuffController;


    private RaycastHit2D hitInfo;
    private Rigidbody2D rb2D;

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

        UIManager.Instance.GetPlayerrUI().SetHealthBar(healthHandler.GetNormalisedHealth());
        UIManager.Instance.GetPlayerrUI().SetUp(characterClassSO.characterIcon);

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

        if (target) Utility.LookAtPosition(transform, target.transform);

        HandleMeleeAbilities();
    }

    protected override void OnHealthChanged()
    {
        UIManager.Instance.GetPlayerrUI().SetHealthBar(healthHandler.GetNormalisedHealth());
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

    public override void TrySetTarget(Transform target)
    {
        if (target != base.target && target.TryGetComponent(out EnemyUnit enemyUnit))
        {
            this.target = enemyUnit;
            enemyUnit.Targetted();
        }
    }

    public override void Targetted()
    {

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
        else if (Input.GetMouseButtonDown(0)) hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
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
        AbilityUIManager.Instance.ToggleMeleeAbilities(target != null && radar.TargetInRange());
    }

    public override StatBase GetStat()
    {
        return _playerStatHandler;
    }

    public void Respawn(Vector2 startPos)
    {
        gameUnitBuffController.RemoveBuffs();
        transform.position = startPos;
        healthHandler.SetHealth(characterClassSO.health);
        _playerStatHandler.SetUp(characterClassSO);
    }
}
