using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    public enum State { Wandering, Combat, Casting, Dead}

    public static PlayerManager Instance { get; private set; }

    public PlayerStatHandler PlayerStatHandler { get => _playerStatHandler; }
    public MovementHandler MovementHandler { get => _movementHandler; }

    [Header("Handlers")]
    [SerializeField] private MovementHandler _movementHandler;
    [SerializeField] private PlayerStatHandler _playerStatHandler;


    [Header("Properties")]
    [SerializeField] private CharacterClassSO characterClassSO;
    [SerializeField] private State  state;

    [Header("Attacking")]
    [SerializeField] private Transform selectedTraget;
    [SerializeField] private Radar2D  radar;
    private RaycastHit2D hitInfo;


    private Rigidbody2D rb2D;
    private Animator animator;
    private HealthHandler healthHandler;
    private float attackTimer;

    private void Awake()
    {
        Instance = this;
        rb2D = GetComponent<Rigidbody2D>();
        healthHandler = GetComponent<HealthHandler>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _movementHandler.SetUp(transform, rb2D, animator);

        healthHandler.SetHealth(characterClassSO.health);
        healthHandler.OnReceiveDamage.AddListener(OnHealthChanged);
        UIManager.Instance.GetPlayerrUI().SetHealthBar(healthHandler.GetNormalisedHealth());
        UIManager.Instance.GetPlayerrUI().SetUp(characterClassSO.characterIcon);
        
        _playerStatHandler.GetManaValue().OnValueChanged += StatHandler_OnManaChanged;
        _playerStatHandler.SetUp(characterClassSO);
    }

    private void Update()
    {
        CheckTargetTouched();
        
        _movementHandler.UpdateMovementAndAnimation(InputHandler.Instance.GetMoveVector());

        _movementHandler.ToggleFaceMovementDir(!selectedTraget);

        if(selectedTraget) Utility.LookAtPosition(transform, selectedTraget);
        
        _playerStatHandler.GetManaValue().ValueRegeneration();

        HandleAttack();
    }

    private void OnHealthChanged()
    {
        UIManager.Instance.GetPlayerrUI().SetHealthBar(healthHandler.GetNormalisedHealth());
    }
    
    void StatHandler_OnManaChanged(object sender, float value)
    {
        UIManager.Instance.GetPlayerrUI().SetManaBar(value);
    }

    public bool TryUseAbility(AbilitySO abilitySO)
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

    public void CheckTargetTouched()
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

    public void TrySetTarget(Transform target)
    {
        if (target.TryGetComponent(out EnemyManager enemyManager))
        {
            selectedTraget = target;
            enemyManager.Targetted();
        }
    }


    public void HandleAttack()
    {
        if (InputHandler.Instance.GetAttackInput().WasPressedThisFrame() && selectedTraget) ChangeState(State.Combat);

        AttackTimer();
    }

    public void AttackTimer()
    {
        if (state != State.Combat || !radar.TargetInRange()) return;

        if (attackTimer < PlayerStatHandler.currentattackSpeed)
        {
            attackTimer += Time.deltaTime;
        }
        else
        {
            attackTimer = 0;
            animator.SetTrigger("Melee");
        }
    }

    public void ChangeState(State newState)
    {
        state = newState;
        animator.SetInteger("State", (int)state);
    }
}
