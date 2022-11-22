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
    }


    protected override void Update()
    {
        base.Update();
    }

    public override void HandleMovement()
    {
        Agro();

        //Patrol();
    }

    protected override void OnHealthChanged()
    {
        UIManager.Instance.GetTargetUI().SetHealthBar(healthHandler.GetNormalisedHealth());
    }

    public override bool TryUseAbility(AbilitySO abilitySO)
    {
        abilitySO.UseAbility();
        return true;
    }

    public override void TrySetTarget(Transform target)
    {
        if (target != base.target && target.TryGetComponent(out PlayerUnit playerUnit))
        {
            state = State.Targetting;
            this.target = playerUnit;
            playerUnit.Targetted(true);
        }
    }

    public override void HandleCombat()
    {
        AttackTimer(_statHandler);
    }

    public override void Targetted(bool status)
    {
        if (status)
        {
            UIManager.Instance.GetTargetUI().SetUp(characterClassSO);
            healthHandler.OnReceiveDamage.AddListener(OnHealthChanged);
            UIManager.Instance.GetTargetUI().SetHealthBar(healthHandler.GetNormalisedHealth());
        }
        else
        {
            UIManager.Instance.GetTargetUI().SetUp(null);
            healthHandler.OnReceiveDamage.RemoveListener(OnHealthChanged);
        }
    }

    public void Patrol()
    {
        if (agent.velocity.magnitude <= 0)
        {
            Utility.RandomPosition(startPos, patrolRange, out Vector2 newPos);
            agent.SetDestination(newPos);
        }

        Utility.FaceVectorDirection(agent.velocity.normalized, transform);
        animator.SetBool("IsMoving", Mathf.Abs(agent.velocity.normalized.x) != 0 || Mathf.Abs(agent.velocity.normalized.y) != 0);
        animator.SetFloat("MoveY", agent.velocity.normalized.y);
    }

    public void Agro()
    {
        if (!radar.TargetInRange()) //Check Distance between enemy and Player
        {
            if (target != null)
            {
                agent.SetDestination(target.transform.position);
                Utility.LookAtPosition(transform, target.transform.position);
                animator.SetBool("IsMoving", Mathf.Abs(agent.velocity.normalized.x) != 0 || Mathf.Abs(agent.velocity.normalized.y) != 0);
                animator.SetFloat("MoveY", agent.velocity.normalized.y);

                state = State.Targetting;
            }
        }
        else
        {
            state = State.Combat;
            agent.velocity = Vector2.zero;
            HandleCombat();
        }
    }

    public void ScanForTargets()
    {
        _targets.Clear();
    }

    public override StatBase GetStat()
    {
        return _statHandler;
    }
}
