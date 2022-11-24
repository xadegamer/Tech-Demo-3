using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameUnit : MonoBehaviour
{
    public enum State { Wandering, Targetting, Combat, Casting, Stun, Dead }

    public event EventHandler<float> OnHealthChangedEvent;

    public Action<bool> OnStun;

    public Damager Damager { get => damager; }
    public HealthHandler HealthHandler { get => healthHandler; }
    public GameUnitAbilityController gameUnitAbilityController { get => unitAbilityController; }
    public GameUnitBuffController gameUnitBuffController { get => unitBuffController; }

    [Header("Abstract Properties")]
    [SerializeField] protected CharacterClassSO characterClassSO;
    [SerializeField] protected State state;

    [Header("Abstract Attacking Properties")]
    [SerializeField] protected GameUnit target;
    [SerializeField] protected Radar2D attackRadar;
    [SerializeField] protected Damager damager;

    protected GameUnitAbilityController unitAbilityController;

    protected GameUnitBuffController unitBuffController;

    protected Animator animator;
    protected HealthHandler healthHandler;

    protected float attackSpeed;
    protected float attackTimer;

    protected State lastState;
    protected bool canMove = true;

    protected bool isTargetted;

    protected virtual void Awake()
    {
        healthHandler = GetComponent<HealthHandler>();
        animator = GetComponentInChildren<Animator>();
        unitAbilityController = GetComponent<GameUnitAbilityController>();
        unitBuffController = GetComponent<GameUnitBuffController>();
    }

    protected virtual void Start()
    {
        healthHandler.SetHealth(characterClassSO.health);

        healthHandler.ModifyPhysicalDamageResistance(characterClassSO.physicalDamageReduction);

        healthHandler.OnHealthChange.AddListener(OnHealthChanged);

        healthHandler.OnDeath.AddListener(OnDeath);

        damager.SetUp(this, characterClassSO.minbaseDamage, characterClassSO.maxbaseDamage, characterClassSO.chanceToHit, characterClassSO.chanceToCrit, characterClassSO.criticalDamageMultipier);
    }

    protected virtual void Update()
    {
        if (state == State.Stun || state == State.Dead) return;
        HandleMovement();
        HandleCombat();
    }

    public abstract void HandleMovement();

    protected virtual void OnHealthChanged(float normalisedValue)
    {
        OnHealthChangedEvent?.Invoke(this, normalisedValue);
    }

    protected virtual void OnDeath(DamageInfo arg0)
    {
        ChangeState(State.Dead);
        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("Dead");
    }

    public abstract bool TryUseAbility(Ability ability);

    public abstract bool TrySetTarget(Transform target);

    public virtual void Targetted(bool status)
    {
        isTargetted = status;
    }

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
        if (state == State.Dead) return;    
        lastState = state;
        state = newState;
        animator.SetInteger("State", (int)state);
    }
    
    public GameUnit GetTarget()
    {
        return target;
    }

    public virtual void SetTarget(GameUnit newTarget)
    {
        target = newTarget;
    }

    public void Stun(float stunDuration)
    {
        StartCoroutine(StunCoroutine(stunDuration));
    }

    IEnumerator StunCoroutine(float stunDuration)
    {
        StartStun();
        yield return new WaitForSeconds(stunDuration);
        EndStun();
    }

    public virtual void StartStun()
    {
        OnStun?.Invoke(true);
        PopUpTextManager.Instance.PopUpText(transform, "Stunned", Color.red);
        ChangeState(State.Stun);
    }

    public virtual void EndStun()
    {
        ChangeState(lastState);
        OnStun?.Invoke(false);
    }

    public bool IsDead()
    {
        return state == State.Dead;
    }

    public virtual void CanMove(bool status)
    {
        canMove = status;
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
