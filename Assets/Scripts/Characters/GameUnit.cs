using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameUnit : MonoBehaviour
{
    public enum State { Wandering, Combat, Casting, Stun, Dead }

    public Damager Damager { get => damager; }
    public HealthHandler HealthHandler { get => healthHandler; }

    [Header("Abstract Properties")]
    [SerializeField] protected CharacterClassSO characterClassSO;
    [SerializeField] protected State state;

    [Header("Abstract Attacking Properties")]
    [SerializeField] protected GameUnit target;
    [SerializeField] protected Radar2D radar;
    [SerializeField] protected Damager damager;


    protected Animator animator;
    protected HealthHandler healthHandler;
    protected float attackTimer;

    protected virtual void Awake()
    {
        healthHandler = GetComponent<HealthHandler>();
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {
        healthHandler.SetHealth(characterClassSO.health);
        healthHandler.OnReceiveDamage.AddListener(OnHealthChanged);
        damager.SetDamage(characterClassSO.minbaseDamage, characterClassSO.maxbaseDamage);
    }

    protected virtual void Update()
    {
        HandleMovement();
        HandleCombat();
    }

    public abstract void HandleMovement();
    
    protected abstract void OnHealthChanged();

    public abstract bool TryUseAbility(AbilitySO abilitySO);

    public abstract void TrySetTarget(Transform target);

    public abstract void Targetted();

    public abstract void HandleCombat();

    public  void AttackTimer(StatBase statBase)
    {
        if (state != State.Combat || !radar.TargetInRange()) return;

        if (attackTimer < statBase.currentattackSpeed) attackTimer += Time.deltaTime;
        else
        {
            attackTimer = 0;
            animator.SetTrigger("Melee");
        }
    }

    public abstract StatBase GetStat();

    public void ChangeState(State newState)
    {
        state = newState;
        animator.SetInteger("State", (int)state);
    }

    public GameUnit GetTarget()
    {
        return target;
    }

    public IEnumerator StunRoutine(float duration)
    {
        ChangeState(State.Stun);
        yield return new WaitForSeconds(duration);
        ChangeState(State.Combat);
    }

    protected virtual void SetResistance()
    {
        
    }
}