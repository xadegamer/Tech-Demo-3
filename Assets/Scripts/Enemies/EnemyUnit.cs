using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Rendering;
using System;

public class EnemyUnit : GameUnit
{
    public StatBase StatHandler { get => _statHandler; }

    [Header("Enemy Handlers")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float patrolRange;
    [SerializeField] private StatBase _statHandler;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] private Radar2D scanRadar;

    [Header("Ability Info")]
    [SerializeField] private int attackIndex;
    [SerializeField] private float abilityChance = 40;
    [SerializeField] private int lastAbilityIndex;

    private float currentAbilityChace;

    ObservableList<GameObject> _targets = new();
    private Vector2 startPos;

    protected override void Awake()
    {
        base.Awake();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        startPos = transform.position;
    }

    protected override void Start()
    {
        base.Start();
        _statHandler.SetUp(characterClassSO);
        currentAbilityChace = abilityChance;
    }


    protected override void Update()
    {
        base.Update();
    }

    public override void HandleMovement()
    {
        if (state == State.Wandering)
        {
            Patrol();
        }
        else if (state == State.Targetting || state == State.Combat)
        {
            Agro();
        }
    }

    protected override void OnHealthChanged()
    {
        UIManager.Instance.GetTargetUI().SetHealthBar(healthHandler.GetNormalisedHealth());
    }

    protected override void OnDeath()
    {

    }

    public override bool TryUseAbility(AbilitySO abilitySO)
    {
        abilitySO.UseAbility();
        return true;
    }

    public override bool TrySetTarget(Transform target)
    {
        if (target != base.target && target.TryGetComponent(out PlayerUnit playerUnit) && !playerUnit.IsDead())
        {
            state = State.Targetting;
            this.target = playerUnit;
            playerUnit.Targetted(true);

            UIManager.Instance.GetTargetOfTargetUI().SetUp(playerUnit);

            return true;
        }
        return false;
    }

    public override void HandleCombat()
    {
        AttackTimer(_statHandler);
    }

    public override void Targetted(bool status)
    {
        if (status)
        {
            UIManager.Instance.GetTargetUI().SetUp(this);
            healthHandler.OnHealthChange.AddListener(OnHealthChanged);
            UIManager.Instance.GetTargetUI().SetHealthBar(healthHandler.GetNormalisedHealth());
        }
        else
        {
            UIManager.Instance.GetTargetUI().SetUp(null);
            healthHandler.OnHealthChange.RemoveListener(OnHealthChanged);
        }
    }

    public void Patrol()
    {
        if (agent.velocity.magnitude <= 0)
        {
            Utility.RandomPosition(startPos, patrolRange, out Vector2 newPos);
            agent.SetDestination(canMove ? newPos : transform.position);
        }

        Utility.FaceVectorDirection(agent.velocity.normalized, transform);
        animator.SetBool("IsMoving", Mathf.Abs(agent.velocity.normalized.x) != 0 || Mathf.Abs(agent.velocity.normalized.y) != 0);
        animator.SetFloat("MoveY", agent.velocity.normalized.y);

        if (scanRadar.TargetInRange())
        {
           if( TrySetTarget(scanRadar.TargetObjectInRange())) state = State.Targetting;
        }
    }

    public void Agro()
    {
        if (!target.IsDead())
        {
            if (!attackRadar.TargetInRange()) //Check Distance between enemy and Player
            {
                if (target != null)
                {
                    agent.SetDestination(canMove ? target.transform.position : transform.position);
                    Utility.LookAtPosition(transform, target.transform.position);
                    animator.SetBool("IsMoving", Mathf.Abs(agent.velocity.normalized.x) != 0 || Mathf.Abs(agent.velocity.normalized.y) != 0);
                    animator.SetFloat("MoveY", agent.velocity.normalized.y);
                }
            }
            else
            {
                state = State.Combat;
                agent.velocity = Vector2.zero;
                HandleCombat();
            }
        }
        else
        {
            state = State.Wandering;
        }
    }

    public void ScanForTargets()
    {
        _targets.Clear();
    }

    public override void StartStun()
    {
        base.StartStun();
    }

    public override void EndStun()
    {
        base.EndStun();
    }

    public override void CanMove(bool status)
    {
        base.CanMove(status);
    }

    public override void Attack()
    {
        attackIndex++;

        if (attackIndex % 2 == 0) //Random Ability
        {
            if ((currentAbilityChace / 100f) >= UnityEngine.Random.value) 
            {
                AbilitySetSO abilitySet = unitAbilityController.GetAbilitySetSO(0);

                int abilityIndex = UnityEngine.Random.Range(0, abilitySet.abilities.Length);

                while (lastAbilityIndex == abilityIndex) abilityIndex = UnityEngine.Random.Range(0, abilitySet.abilities.Length);

                lastAbilityIndex = abilityIndex;

                abilitySet.abilities[lastAbilityIndex].UseAbility();

                currentAbilityChace = abilityChance;
            }
            else
            {
                currentAbilityChace += 20;
            }
        }
        
        animator.SetTrigger("Melee");
    }

    protected float RandomCriticalDamage(float chance, float criticalDamage)
    {
        if ((chance / 100f) >= UnityEngine.Random.value) return criticalDamage;
        else return 0;
    }

    public override StatBase GetStat()
    {
        return _statHandler;
    }
}
