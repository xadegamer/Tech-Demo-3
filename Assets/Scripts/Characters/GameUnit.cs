using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameUnit : MonoBehaviour
{
    public enum State { Wandering, Targetting, Combat, Casting, Stun, Dead }

    public Damager Damager { get => damager; }
    public HealthHandler HealthHandler { get => healthHandler; }

    [Header("Abstract Properties")]
    [SerializeField] protected CharacterClassSO characterClassSO;
    [SerializeField] protected State state;

    [Header("Abstract Attacking Properties")]
    [SerializeField] protected GameUnit target;
    [SerializeField] protected Radar2D attackRadar;
    [SerializeField] protected Damager damager;

    protected GameUnitAbilityController unitAbilityController;

    protected Animator animator;
    protected HealthHandler healthHandler;

    protected float attackSpeed;
    protected float attackTimer;

    protected State lastState;
    protected bool canMove = true;

    protected virtual void Awake()
    {
        healthHandler = GetComponent<HealthHandler>();
        animator = GetComponentInChildren<Animator>();
        unitAbilityController = GetComponent<GameUnitAbilityController>();
    }

    protected virtual void Start()
    {
        healthHandler.SetHealth(characterClassSO.health);
        
        healthHandler.OnDeath.AddListener(OnDeath);
        
        damager.SetUp(this, characterClassSO.minbaseDamage, characterClassSO.maxbaseDamage);
    }

    protected virtual void Update()
    {
        if (state == State.Stun) return;
        HandleMovement();
        HandleCombat();
    }

    public abstract void HandleMovement();
    
    protected abstract void OnHealthChanged();

    protected abstract void OnDeath();

    public abstract bool TryUseAbility(AbilitySO abilitySO);

    public abstract bool TrySetTarget(Transform target);

    public abstract void Targetted(bool status);

    public abstract void HandleCombat();

    public abstract void Attack();

    public  void AttackTimer(StatBase statBase)
    {
        if (state != State.Combat || !attackRadar.TargetInRange()) return;

        if (attackTimer < statBase.currentattackSpeed) attackTimer += Time.deltaTime;
        else
        {
            Attack();
            attackTimer = 0;
        }
    }

    public abstract StatBase GetStat();

    public void ChangeState(State newState)
    {
        lastState = state;
        state = newState;
        animator.SetInteger("State", (int)state);
    }

    public GameUnit GetTarget()
    {
        return target;
    }
    
    public virtual void StartStun()
    {
        ChangeState(State.Stun);
    }

    public virtual void EndStun()
    {
        ChangeState(lastState);
    }

    public bool IsDead()
    {
        return state == State.Dead;
    }

    public virtual void CanMove(bool status)
    {
        canMove = status;
    }

    protected virtual void SetResistance()
    {
        
    }

    public GameUnitAbilityController GetUnitAbilityController()
    {
        return unitAbilityController;
    }

    public CharacterClassSO GetCharacterClassSO()
    {
        return characterClassSO;
    }
}
