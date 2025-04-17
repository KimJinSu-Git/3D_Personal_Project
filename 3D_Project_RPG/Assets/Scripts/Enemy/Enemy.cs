using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    [Header("Damage")] 
    public int damage;
    
    [Header("Target")]
    public Transform target;

    // 기본 감지 범위 설정 (하이어라키 창에서 몬스터 개체애 따라 수정 가능)
    [Header("Ranges")]
    public float detectionRange = 10f; // 감지 범위
    public float chaseRange = 15f; // 추격 범위
    public float patrolRange = 5f; // 순찰 범위 
    public float maxDistance = 20f; // 시작 위치로 부터 최대 범위

    [Header("Animations")]
    public Animator animation; // MonsterController에서 제어할 애니메이터
    
    [Header("Patrol")]
    public float patrolTime = 3f;
    [HideInInspector] public float lastPatrolTime;
    [HideInInspector] public Vector3 initialPosition;
    
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Collider collider;
    [HideInInspector] public Material material;
    [HideInInspector] public NavMeshAgent nav;
    

    [HideInInspector] public bool isSurvive;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }
        nav = GetComponent<NavMeshAgent>();

        initialPosition = transform.position;
        lastPatrolTime = Time.time;
        currentHealth = maxHealth;
        isSurvive = true;
    }

    private void LateUpdate()
    {
        // 플레이어와 몬스터가 충돌하면 몬스터가 밀려난 뒤 플레이어를 향해 다시 추격하지 못하는 오류 방지
        if (rigid != null)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentHealth <= 0 || !isSurvive) return;

        if (other.CompareTag("Sword") || other.CompareTag("Shield"))
        {
            int damage = 10; 
            currentHealth -= damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
        }
    }

    private IEnumerator OnDamage(Vector3 reactVec)
    {
        if (material != null)
        {
            material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            if (material != null)
            {
                material.color = Color.white;
            }
        }
        else
        {
            if (material != null)
            {
                material.color = Color.gray;
            }
            if (rigid != null) rigid.velocity = Vector3.zero;
            if (collider != null) collider.enabled = false;
            isSurvive = false;
            if (nav != null && nav.enabled) nav.enabled = false;
        }
    }
}
