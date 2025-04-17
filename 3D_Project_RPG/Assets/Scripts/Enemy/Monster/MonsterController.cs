using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public Enemy enemyData; // Inspector에서 할당할 Enemy 데이터 스크립트

    private MonsterBaseState currentState;

    // 상태 정의
    public MonsterIdleState IdleState { get; private set; }
    public MonsterPatrolState PatrolState { get; private set; }
    public MonsterChaseState ChaseState { get; private set; }
    public MonsterAttackState AttackState { get; private set; }
    public MonsterDieState DieState { get; private set; }

    // 애니메이션 관련 변수 (몬스터 종류에 따라 다를 수 있음)
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

    // 현재 재생중인 애니메이션 인덱스 관리
    private int currentIdleAnimIndex = 0;
    private int currentAttackAnimIndex = 0;

    void Awake()
    {
        // Enemy 컴포넌트가 없을 때 에러 발생 방지
        if (enemyData == null)
        {
            Debug.LogError("MonsterController가 Enemy 데이터 스크립트를 참조하지 않습니다!");
            enabled = false;
            return;
        }

        // 상태 객체 생성 및 초기화
        IdleState = new MonsterIdleState(this, enemyData);
        PatrolState = new MonsterPatrolState(this, enemyData);
        ChaseState = new MonsterChaseState(this, enemyData);
        AttackState = new MonsterAttackState(this, enemyData);
        DieState = new MonsterDieState(this, enemyData);

        // 초기 상태 설정
        currentState = IdleState;
        currentState.Enter();
    }

    void Update()
    {
        if (enemyData.currentHealth <= 0 && currentState != DieState)
        {
            ChangeState(DieState);
            return;
        }

        if (enemyData.isSurvive)
        {
            currentState.Update();
        }
    }

    public void ChangeState(MonsterBaseState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter();
    }

    // 애니메이션 선택 함수 (몬스터 종류에 따라 다르게 구현 가능)
    public string GetRandomIdleAnimation()
    {
        if (idleAnimations.Length > 0)
        {
            currentIdleAnimIndex = Random.Range(0, idleAnimations.Length);
            return idleAnimations[currentIdleAnimIndex];
        }
        return null;
    }

    public string GetWalkAnimation()
    {
        return walkAnimation;
    }

    public string GetRunAnimation()
    {
        return runAnimation;
    }

    public string GetRandomAttackAnimation()
    {
        if (attackAnimations.Length > 0)
        {
            currentAttackAnimIndex = Random.Range(0, attackAnimations.Length);
            return attackAnimations[currentAttackAnimIndex];
        }
        return null;
    }

    public string GetDieAnimation()
    {
        return dieAnimation;
    }
}