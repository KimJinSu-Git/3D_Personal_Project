using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnZone : MonoBehaviour
{
    [System.Serializable]
    public class SpawnSetting
    {
        public string monsterId; 
        public int spawnCount = 3;
    }

    [Header("스폰 설정")]
    public List<SpawnSetting> spawnSettings;

    [Header("스폰 범위")]
    public float spawnRadius = 10f;

    [Header("리젠 타이밍")]
    public float respawnDelay = 5f;

    private Dictionary<string, List<GameObject>> activeMonsters = new Dictionary<string, List<GameObject>>();

    private void Start()
    {
        foreach (var setting in spawnSettings)
        {
            activeMonsters.Add(setting.monsterId, new List<GameObject>());

            for (int i = 0; i < setting.spawnCount; i++)
            {
                SpawnMonster(setting.monsterId);
            }
        }
    }

    public void MonsterDie(string monsterId, GameObject monster)
    {
        if (activeMonsters.ContainsKey(monsterId))
        {
            activeMonsters[monsterId].Remove(monster);
            StartCoroutine(RespawnMonster(monsterId));
        }
    }

    private IEnumerator RespawnMonster(string monsterId)
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnMonster(monsterId);
    }

    private void SpawnMonster(string monsterId)
    {
        GameObject monster = MonsterPoolManager.Instance.GetPool(monsterId);
        monster.transform.position = GetRandomPosition();
        monster.transform.rotation = Quaternion.identity;

        Enemy enemy = monster.GetComponent<Enemy>();
        enemy?.SetSpawnZone(this, monsterId);
        enemy?.Init();
        activeMonsters[monsterId].Add(monster);
    }

    private Vector3 GetRandomPosition()
    {
        Vector2 randCircle = Random.insideUnitCircle * spawnRadius;
        return transform.position + new Vector3(randCircle.x, 0, randCircle.y);
    }
}
