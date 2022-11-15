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
    private Vector2 smoothDeltaPosition = Vector2.zero;
    private Vector2 velocity = Vector2.zero;

    protected override void Awake()
    {
        base.Awake();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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
        if (target != null)
        {
            agent.SetDestination(target.position); //Set NavMesh Position


            if (Vector2.Distance(transform.position, target.position) > agent.stoppingDistance) //Check Distance between enemy and Player
            {
                if (target != null)
                {
                    animator.SetBool("Walk", true);
                    agent.speed = moveSpeed;
                }
            }
            else
            {
                agent.speed = 0;
                HandleCombat();
            }
        }
       // else Patrol();
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

    }

    public override void HandleCombat()
    {
        AttackTimer(_statHandler);
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

    public void Patrol()
    {
        if (agent.velocity.magnitude <= 0)
        {
            Utility.RandomPosition(transform.position, patrolRange, out Vector2 newPos);
            agent.SetDestination(newPos);
        }

        Utility.FaceVectorDirection(agent.velocity.normalized, transform);
        animator.SetFloat("MoveX", Mathf.Abs(agent.velocity.normalized.x));
        animator.SetFloat("MoveY", agent.velocity.normalized.y);
    }

    private void LateUpdate()
    {

    }
}
