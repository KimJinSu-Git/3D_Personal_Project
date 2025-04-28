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

    private int currentIdleAnimationIndex = 0;
    private int currentAttackAnimationIndex = 0;

    void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError("MonsterController에서 Error 발생 (enemyData가 없서여)");
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

        // enemyData의 타겟 설정을 안해 줬다면 플레이어를 찾아서 할당해놓도록 !
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
        // 몬스터가 죽었다면 리턴
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
            currentIdleAnimationIndex = Random.Range(0, idleAnimations.Length);
            return idleAnimations[currentIdleAnimationIndex];
        }
        return null;
    }

    public string GetWalkAnimation() => walkAnimation;
    public string GetRunAnimation() => runAnimation;

    public string GetRandomAttackAnimation()
    {
        if (attackAnimations.Length > 0)
        {
            currentAttackAnimationIndex = Random.Range(0, attackAnimations.Length);
            return attackAnimations[currentAttackAnimationIndex];
        }
        return null;
    }

    public string GetDieAnimation() => dieAnimation;
}