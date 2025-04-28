using System;
using System.Collections;
using System.Collections.Generic;
using Suntail;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Header("Monster Info")]
    public string monsterName;
    
    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    [Header("EXP")] 
    public int expReward;
    
    [Header("Rewards")]
    public int goldReward;

    [Header("Damage")] 
    public int damage;
    
    [Header("Target")]
    public Transform target;

    [Header("Ranges")]
    public float detectionRange = 10f;
    public float chaseRange = 15f;
    public float patrolRange = 5f;
    public float maxDistance = 20f;

    [Header("Animations")]
    public Animator animation;

    [Header("Patrol")]
    public float patrolTime = 3f;
    [HideInInspector] public float lastPatrolTime;
    [HideInInspector] public Vector3 initialPosition;

    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Collider collider;
    [HideInInspector] public Material material;
    [HideInInspector] public NavMeshAgent nav;

    [HideInInspector] public bool isSurvive;

    private PlayerController player;
    private MonsterSpawnZone spawnZone;
    private string monsterId;

    // ID 반환 메서드
    public string GetMonsterId()
    {
        return monsterId;
    }

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

        player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
    }

    public void Init()
    {
        currentHealth = maxHealth;
        isSurvive = true;

        if (collider != null) collider.enabled = true;
        if (material != null) material.color = Color.white;

        if (nav != null)
        {
            nav.enabled = true;
            nav.ResetPath();
            nav.velocity = Vector3.zero;
        }

        if (animation != null)
        {
            animation.Play("Idle");
        }
    }

    public void SetSpawnZone(MonsterSpawnZone zone, string id)
    {
        spawnZone = zone;
        monsterId = id;
    }

    private void LateUpdate()
    {
        if (rigid != null)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentHealth <= 0 || !isSurvive) return;

        if (other.CompareTag("Sword"))
        {
            if (other.GetComponentInParent<PlayerController>() != null)
            {
                int playerDamage = other.GetComponentInParent<PlayerController>().playerSwrodDamage;
                currentHealth -= playerDamage;
                Vector3 reactVec = transform.position - other.transform.position;
                StartCoroutine(OnDamage(reactVec, playerDamage));
            }
        }
        else if (other.CompareTag("Shield"))
        {
            if (other.GetComponentInParent<PlayerController>() != null)
            {
                int playerDamage = other.GetComponentInParent<PlayerController>().playerShieldDamage;
                currentHealth -= playerDamage;
                Vector3 reactVec = transform.position - other.transform.position;
                StartCoroutine(OnDamage(reactVec, playerDamage));
            }
        }
    }

    private IEnumerator OnDamage(Vector3 reactVec, int damageAmount)
    {
        if (material != null)
        {
            material.color = Color.red;
        }

        Vector3 popupPos = transform.position + Vector3.up;
        DamageTextSpawner.Instance.SpawnDamageText(popupPos, damageAmount);
        
        MonsterHPUI.Instance?.ShowMonsterHP(monsterName, currentHealth, maxHealth);

        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            if (material != null)
                material.color = Color.white;
        }
        else
        {
            if (material != null) material.color = Color.gray;
            if (rigid != null) rigid.velocity = Vector3.zero;
            if (collider != null) collider.enabled = false;
            if (nav != null && nav.enabled) nav.enabled = false;

            MonsterHPUI.Instance?.PanelHide();
            if (player != null)
            {
                player.GainExp(expReward);
                PlayerGoldManager.Instance.AddGold(goldReward);
                Debug.Log($"골드{goldReward}G 획득");
            }
            QuestManager.Instance.ReportKill(monsterId);
            
            spawnZone?.MonsterDie(monsterId, gameObject);
        }
    }
}
