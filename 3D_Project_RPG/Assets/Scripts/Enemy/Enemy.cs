using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public Transform target;

    public float detectionRange = 10f; // 감지 범위
    public float chaseRange = 15f; // 추격 범위
    public float patrolRange = 5f; // 순찰 범위 
    public float patrolTime = 3f; // 다음 순찰까지 대기 시간
    public float maxDistance = 20f; // 시작 위치로 부터 최대 범위

    private Rigidbody rigid;
    private Collider collider;
    private Material material;
    private NavMeshAgent nav;
    private Animator animation;
    
    private Vector3 initialPosition; // 몬스터의 초기 위치
    private float lastPatrolTime;
    private bool isSurvive;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        material = GetComponentInChildren<SkinnedMeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        animation = GetComponent<Animator>();
        
        initialPosition = transform.position; // 초기 위치 저장
        lastPatrolTime = Time.time;
    }

    private void Start()
    {
        isSurvive = true;
    }

    private void Update()
    {
        if (target == null)
        {
            // 타겟이 없다면 랜덤 위치로 이동(순찰)
            Patrol();
            return;
        }

        if (isSurvive == false)
        {
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float distanceToInitial = Vector3.Distance(transform.position, initialPosition);

        // 초기 위치에서 너무 멀리 벗어났다면 플레이어가 몬스터의 감지 범위 안으로 들어왔더라도 더 이상 따라가지 않게 하기
        if (distanceToInitial > maxDistance)
        {
            nav.SetDestination(initialPosition);
            nav.isStopped = false;
            return; // 아래 행동 불가 !
        }
        
        if (distanceToTarget <= detectionRange)
        {
            // 플레이어가 감지 범위 안에 들어오면 추격 시작 !
            nav.SetDestination(target.position);
            nav.isStopped = false;
        }
        else if (distanceToTarget <= chaseRange && nav.remainingDistance > nav.stoppingDistance)
        {
            // 추격 범위 안에서는 계속 추격 !
            nav.SetDestination(target.position);
            nav.isStopped = false;
        }
        else if (distanceToInitial > 1f)
        {
            // 플레이어가 추격 범위를 벗어나면 원래 위치로 돌아가기
            nav.SetDestination(initialPosition);
            nav.isStopped = false;
        }
        else
        {
            // 원래 위치에 도착하면 다시 랜덤 위치 순찰 !
            Patrol();
        }
    }

    private void LateUpdate()
    {
        // FreezeVelocity();
    }

    // 플레이어와 몬스터가 충돌하면 몬스터가 밀려난 뒤 플레이어를 향해 다시 추격하지 못하는 오류 방지
    private void FreezeVelocity()
    {
        // 물리력이 NavMeshAgent 이동을 방해하지 않도록 로직 작성.
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    private void Patrol()
    {
        // 순찰 시간이 되었고, 이미 주어진 목적지에 거의 도착했을 때에 새로운 순찰 위치로 이동
        if (Time.time - lastPatrolTime >= patrolTime && nav.remainingDistance <= nav.stoppingDistance)
        {
            lastPatrolTime = Time.time;
            // Random.inside 구문 ==> 초기 위치를 기준으로 paterolRange 범위 안의 랜덤한 값의 위치로 순찰 이동 설정.
            Vector3 randomDirection = Random.insideUnitSphere * patrolRange;
            randomDirection += initialPosition;

            NavMeshHit hit;
            // NavMesh 상에 유효한 공간을 찾는 함수 => 랜덤한 위치가 구워지지 않은 위치면 안되니깐 그 근처의 구워진 공간으로 이동
            NavMesh.SamplePosition(randomDirection, out hit, patrolRange, NavMesh.AllAreas);
            nav.SetDestination(hit.position);
            nav.isStopped = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            currentHealth -= 10;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.CompareTag("Shield"))
        {
            // 쉴드 데미지 구현
            currentHealth -= 10;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
        }
    }

    private IEnumerator OnDamage(Vector3 reactVec)
    {
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            material.color = Color.white;
        }
        else
        {
            material.color = Color.gray;
            rigid.velocity = Vector3.zero;
            collider.enabled = false;
            
            animation.SetTrigger("isDie");
            nav.enabled = false;
            isSurvive = false;
            Destroy(gameObject, 4);
        }
    }
}
