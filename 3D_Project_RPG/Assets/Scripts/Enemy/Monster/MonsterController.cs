using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MonsterController : MonoBehaviour
{
    public Enemy enemyData; // Inspector에서 할당할 Enemy 데이터 스크립트

    private MonsterBaseState currentState;

    public MonsterIdleState IdleState { get; private set; }
    public MonsterPatrolState PatrolState { get; private set; }
    public MonsterChaseState ChaseState { get; private set; }
    public MonsterAttackState AttackState { get; private set; }
    public MonsterDieState DieState { get; private set; }

    [Header("Idle Animations")]
    public string[] idleAnimations = { "Idle1" };
    [Header("Walk Animations")]
    public string walkAnimation = "Walk";
    [Header("Run Animations")]
    public string runAnimation = "Run";
    [Header("Attack Animations")]
    public string[] attackAnimations = { "Attack1" };
    [Header("Die Animation")]
    public string dieAnimation = "Die";

    private int currentIdleAnimIndex = 0;
    private int currentAttackAnimIndex = 0;

    void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError("MonsterController가 Enemy 데이터 스크립트를 참조하지 않습니다!");
            enabled = false;
            return;
        }

        IdleState = new MonsterIdleState(this, enemyData);
        PatrolState = new MonsterPatrolState(this, enemyData);
        ChaseState = new MonsterChaseState(this, enemyData);
        AttackState = new MonsterAttackState(this, enemyData);
        DieState = new MonsterDieState(this, enemyData);

        currentState = IdleState;
        currentState.Enter();

        if (enemyData.target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                enemyData.target = player.transform;
            }
        }
    }

    void Update()
    {
        if (!enemyData.isSurvive) return;
        currentState.Update();
    }

    public void ChangeState(MonsterBaseState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public string GetRandomIdleAnimation()
    {
        if (idleAnimations.Length > 0)
        {
            currentIdleAnimIndex = Random.Range(0, idleAnimations.Length);
            return idleAnimations[currentIdleAnimIndex];
        }
        return null;
    }

    public string GetWalkAnimation() => walkAnimation;
    public string GetRunAnimation() => runAnimation;

    public string GetRandomAttackAnimation()
    {
        if (attackAnimations.Length > 0)
        {
            currentAttackAnimIndex = Random.Range(0, attackAnimations.Length);
            return attackAnimations[currentAttackAnimIndex];
        }
        return null;
    }

    public string GetDieAnimation() => dieAnimation;
}