using System.Collections;
using System.Collections.Generic;
using Suntail;
using UnityEngine;
using UnityEngine.AI;

// 몬스터의 Base 추상 상태
public abstract class MonsterBaseState
{
    // 현재 몬스터 상태를 관리하는 컨트롤러
    protected MonsterController controller;
    // 몬스터의 기본적인 데이터 스크립트 (몬스터마다 갖고 있는 애니메이션들을 따로 담고 있음)
    protected Enemy monster; // Enemy 스크립트 참조

    // 생성자
    public MonsterBaseState(MonsterController controller, Enemy monster)
    {
        this.controller = controller;
        this.monster = monster;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

    protected void PlayAnimation(string animationName)
    {
        monster.animation.Play(animationName);
    }

    protected void LookAtTarget()
    {
        if (monster.target == null) return;

        Vector3 direction = (monster.target.position - monster.transform.position).normalized;
        direction.y = 0; // y축 고정 (수직으로 고개를 꺾는 걸 방지)

        if (direction.magnitude == 0) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        monster.transform.rotation = Quaternion.Slerp(monster.transform.rotation, targetRotation, Time.deltaTime * 5f); // 회전 속도 조절
    }

}

// 몬스터 대기 상태
public class MonsterIdleState : MonsterBaseState
{
    public MonsterIdleState(MonsterController controller, Enemy monster) : base(controller, monster) { }

    public override void Enter()
    {
        // MonsterController에서 랜덤한 Idle 애니메이션 이름 호출
        string idleAnim = controller.GetRandomIdleAnimation();
        // 가져온 애니메이션 이름의 애니메이션 재생
        if (!string.IsNullOrEmpty(idleAnim))
        {
            PlayAnimation(idleAnim);
        }
    }

    public override void Update()
    {
        if (monster.currentHealth <= 0 && monster.isSurvive)
        {
            controller.ChangeState(controller.DieState);
            return;
        }
        // 타겟이 존재하고, 타겟과의 거리가 감지 범위 이내라면 Chase(추적) 상태로 전환 !
        if (monster.target != null && Vector3.Distance(monster.transform.position, monster.target.position) <= monster.detectionRange)
        {
            controller.ChangeState(controller.ChaseState);
            return;
        }

        // 마지막 순찰 시간으로부터 patrolTime(순찰시간)이 지났다면 Patrol(순찰) 상태로 전환
        if (Time.time - monster.lastPatrolTime >= monster.patrolTime)
        {
            controller.ChangeState(controller.PatrolState);
        }
    }

    public override void Exit() { }
}

// 몬스터 순찰 상태
public class MonsterPatrolState : MonsterBaseState
{
    // 순찰 목표 위치
    private Vector3 targetPosition;

    public MonsterPatrolState(MonsterController controller, Enemy monster) : base(controller, monster) { }

    public override void Enter()
    {
        // NavMeshAgent 이동 정지 해제
        monster.nav.isStopped = false;
        // 마지막 순찰 시간 갱신
        monster.lastPatrolTime = Time.time;
        // 새로운 순찰 목적지 설정
        SetNewPatrolDestination();

        if (!string.IsNullOrEmpty(controller.walkAnimation))
        {
            PlayAnimation(controller.walkAnimation);
        }
    }

    public override void Update()
    {
        if (monster.currentHealth <= 0 && monster.isSurvive)
        {
            controller.ChangeState(controller.DieState);
            return;
        }
        // 타겟이 존재하고, 타겟과의 거리가 감지 범위 이내라면 Chase(추적) 상태로 전환
        if (monster.target != null && Vector3.Distance(monster.transform.position, monster.target.position) <= monster.detectionRange)
        {
            controller.ChangeState(controller.ChaseState);
            return;
        }

        // NavMeshAgent의 순찰이 끝났다면 새로운 순찰 위치 재설정
        if (monster.nav.remainingDistance <= monster.nav.stoppingDistance)
        {
            SetNewPatrolDestination();

            if (!string.IsNullOrEmpty(controller.walkAnimation))
            {
                PlayAnimation(controller.walkAnimation);
            }
        }
    }

    public override void Exit()
    {
        // 얼음 !!!!!!!!!!!!!!!! 나는 이 게임을 해봤어요 ~~~
        monster.nav.isStopped = true;
    }

    private void SetNewPatrolDestination()
    {
        // 초기 배치된 위치를 기준으로 patrolRange 범위 내의 랜덤한 위치를 생성
        Vector3 randomDirection = Random.insideUnitSphere * monster.patrolRange;
        randomDirection += monster.initialPosition;

        NavMeshHit hit;
        // 생성된 랜덤 위치 주변의 NavMesh 상의 유효한 위치를 찾음 (Baked 되지 않은 위치가 랜덤한 위치로 잡히면 안되기 때문에 예외처리)
        if (NavMesh.SamplePosition(randomDirection, out hit, monster.patrolRange, NavMesh.AllAreas))
        {
            // NavMeshAgent의 목표 지점을 찾은 유효한 위치로 설정
            monster.nav.SetDestination(hit.position);
        }
    }
}

// 몬스터 추격 상태
public class MonsterChaseState : MonsterBaseState
{
    private Vector3 chaseStartPosition; // 추격 시작 위치 저장
    public MonsterChaseState(MonsterController controller, Enemy monster) : base(controller, monster) { }

    public override void Enter()
    {
        // NavMeshAgent 이동 정지 해제
        monster.nav.isStopped = false;

        chaseStartPosition = monster.transform.position; // 추격 시작 위치 저장
        if (!string.IsNullOrEmpty(controller.runAnimation))
        {
            PlayAnimation(controller.runAnimation);
        }
        else if (!string.IsNullOrEmpty(controller.walkAnimation))
        {
            PlayAnimation(controller.walkAnimation);
        }
    }

    public override void Update()
    {
        if (monster.currentHealth <= 0 && monster.isSurvive)
        {
            controller.ChangeState(controller.DieState);
            return;
        }
        // 초기 위치에서 너무 멀리 벗어났다면 초기 위치로 돌아감 (최우선 순위)
        float distanceToInitial = Vector3.Distance(monster.transform.position, monster.initialPosition);
        if (distanceToInitial > monster.maxDistance)
        {
            monster.nav.SetDestination(monster.initialPosition);
            if (!string.IsNullOrEmpty(controller.walkAnimation))
            {
                PlayAnimation(controller.walkAnimation);
            }
            // 초기 위치에 거의 도착하면 Idle 상태로 전환
            if (distanceToInitial <= monster.nav.stoppingDistance)
            {
                controller.ChangeState(controller.IdleState);
            }
            return;
        }

        // 타겟이 없거나, 타겟과의 거리가 추격 범위를 벗어나면 추격 시작 위치로 이동
        if (monster.target == null || Vector3.Distance(monster.transform.position, monster.target.position) > monster.chaseRange)
        {
            monster.nav.SetDestination(chaseStartPosition); // 추격 시작 위치로 이동
            if (!string.IsNullOrEmpty(controller.walkAnimation))
            {
                PlayAnimation(controller.walkAnimation);
            }
            // 추격 시작 위치에 도착하면 Idle 상태로 전환
            if (Vector3.Distance(monster.transform.position, chaseStartPosition) <= monster.nav.stoppingDistance)
            {
                controller.ChangeState(controller.IdleState);
            }
            return;
        }

        float distanceToTarget = Vector3.Distance(monster.transform.position, monster.target.position);

        // 타겟과의 거리가 공격 가능 거리 이내라면 Attack 상태로 전환하고 이동을 멈춤
        if (distanceToTarget <= 2.0f) // 공격 가능 거리
        {
            monster.nav.isStopped = true; // 이동 멈춤
            controller.ChangeState(controller.AttackState);
            return;
        }

        // 타겟의 위치를 NavMeshAgent의 목표 지점으로 설정하여 추격
        monster.nav.SetDestination(monster.target.position);

        if (!string.IsNullOrEmpty(controller.runAnimation))
        {
            PlayAnimation(controller.runAnimation);
        }
        else if (!string.IsNullOrEmpty(controller.walkAnimation))
        {
            PlayAnimation(controller.walkAnimation);
        }
    }

    public override void Exit()
    {
        monster.nav.isStopped = true;
    }
}

// 몬스터 공격 상태
public class MonsterAttackState : MonsterBaseState
{
    // 공격 쿨타임 체크
    private float attackTimer = 0f;
    // 공격 쿨타임
    private float attackCooldown = 2f; // 공격 쿨다운
    // 현재 재생 중인 공격 애니메이션의 이름
    private string currentAttackAnimName;
    // 애니메이션 상태 정보
    private AnimatorStateInfo animatorStateInfo;
    // 공격 애니메이션이 시작될 때
    private bool isAttacking = false;
    // 데미지 중복 방지
    private bool hasDealtDamage = false;
    
    public MonsterAttackState(MonsterController controller, Enemy monster) : base(controller, monster) { }

    public override void Enter()
    {
        // NavMeshAgent 이동 정지
        monster.nav.isStopped = true;
        // 공격 쿨타임 체크 초기화
        attackTimer = 0f;
        isAttacking = false;
        hasDealtDamage = false;
        
        currentAttackAnimName = controller.GetRandomAttackAnimation();
        if (!string.IsNullOrEmpty(currentAttackAnimName))
        {
            PlayAnimation(currentAttackAnimName);
        }
    }

    public override void Update()
    {
        if (monster.currentHealth <= 0 && monster.isSurvive)
        {
            controller.ChangeState(controller.DieState);
            return;
        }
        animatorStateInfo = monster.animation.GetCurrentAnimatorStateInfo(0);
        // 타겟이 없거나, 타겟과의 거리가 공격 불가능 거리보다 멀어지면 Chase 상태로 전환
        if (monster.target == null || Vector3.Distance(monster.transform.position, monster.target.position) > 4.0f)
        {
            controller.ChangeState(controller.ChaseState);
            return;
        }

        LookAtTarget();

        // 공격 중이라면 애니메이션 종료 체크 후 Chase 상태로 전환
        if (isAttacking)
        {
            if (!hasDealtDamage && animatorStateInfo.IsName(currentAttackAnimName) && animatorStateInfo.normalizedTime is >= 0.2f and <= 0.7f)
            {
                DealDamageToPlayer();
                hasDealtDamage = true;
            }
            
            if (animatorStateInfo.IsName(currentAttackAnimName) && animatorStateInfo.normalizedTime >= 0.9f)
            {
                controller.ChangeState(controller.ChaseState);
            }
            return; // 공격 애니메이션이 아직 진행 중이므로 더 이상 아래 로직 실행 안함
        }

        // 공격 중이 아닐 때는 Idle 상태
        if (animatorStateInfo.IsName("Idle"))
        {
            return;
        }
        else
        {
            PlayAnimation("Idle");
        }

        // 공격 쿨타임 체크시간 증가
        attackTimer += Time.deltaTime;
        // 공격이 가능한 시간이 되면 공격 애니메이션을 재생시키고 공격 시작 상태로 변경
        if (attackTimer >= attackCooldown)
        {
            currentAttackAnimName = controller.GetRandomAttackAnimation();
            if (!string.IsNullOrEmpty(currentAttackAnimName))
            {
                PlayAnimation(currentAttackAnimName);
                isAttacking = true; // 공격 시작
                hasDealtDamage = false;
                attackTimer = 0f; // 공격 시작 후 쿨타임 다시 초기화
            }
        }
    }

    public override void Exit()
    {
        monster.nav.isStopped = false;
    }

    private void DealDamageToPlayer()
    {
        if (monster.target != null)
        {
            PlayerController player = monster.target.GetComponent<PlayerController>();
            if (player != null && player.playerCurrentHp > 0)
            {
                if (player.currentState == PlayerState.ShieldWait || player.currentState == PlayerState.ShieldWalk || player.currentState == PlayerState.ShieldRun || player.currentState == PlayerState.ShieldAttack)
                {
                    player.playerCurrentHp -= monster.damage/2;
                }
                else
                {
                    player.playerCurrentHp -= monster.damage;
                }

            
                Debug.Log($"몬스터가 플레이어에게 {monster.damage} 데미지를 입혔습니다. 현재 플레이어 HP: {player.playerCurrentHp}");

                if (player.playerCurrentHp <= 0)
                {
                    player.ChangeState(PlayerState.Die);
                }
            }
        }
    }
}

// 몬스터의 사망 상태를 나타내는 클래스
public class MonsterDieState : MonsterBaseState
{
    private float dieTimer = 0f;
    private float dieDuration = 2f;

    public MonsterDieState(MonsterController controller, Enemy monster) : base(controller, monster) { }

    public override void Enter()
    {
        monster.nav.enabled = false;
        monster.collider.enabled = false;
        if (monster.rigid != null) monster.rigid.velocity = Vector3.zero;

        PlayAnimation(controller.GetDieAnimation());
        dieTimer = 0f;
    }

    public override void Update()
    {
        dieTimer += Time.deltaTime;
        if (dieTimer >= dieDuration)
        {
            monster.isSurvive = false;
            MonsterPoolManager.Instance.ReturnPool(monster.GetMonsterId(), monster.gameObject);
        }
    }

    public override void Exit() { }
}