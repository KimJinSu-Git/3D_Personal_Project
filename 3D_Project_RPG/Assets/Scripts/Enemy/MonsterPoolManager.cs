using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPoolManager : MonoBehaviour
{
    public static MonsterPoolManager Instance;

    [System.Serializable]
    public class MonsterPool
    {
        public string monsterId;
        public GameObject monsterPrefab;
        public int poolSize = 5;

        [HideInInspector] public Queue<GameObject> poolQueue = new Queue<GameObject>();
    }

    public List<MonsterPool> monsterPools;

    private Dictionary<string, MonsterPool> poolDictionary = new Dictionary<string, MonsterPool>();
    
    private Transform poolParent;

    private void Awake()
    {
        Instance = this;

        GameObject poolContainer = GameObject.Find("MonsterPoolContainer");

        if (poolContainer == null)
        {
            Debug.LogError("MonsterPoolContainer가 없워");
            return;
        }

        poolParent = poolContainer.transform;

        foreach (var pool in monsterPools)
        {
            pool.poolQueue = new Queue<GameObject>();
            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject monsterObj = Instantiate(pool.monsterPrefab, poolParent);
                monsterObj.SetActive(false);
                pool.poolQueue.Enqueue(monsterObj);
            }
            poolDictionary.Add(pool.monsterId, pool);
        }
    }

    public GameObject GetPool(string monsterId)
    {
        if (!poolDictionary.ContainsKey(monsterId)) return null;

        var pool = poolDictionary[monsterId];

        if (pool.poolQueue.Count > 0)
        {
            GameObject monsterObj = pool.poolQueue.Dequeue();
            monsterObj.SetActive(true);
            return monsterObj;
        }

        GameObject newObj = Instantiate(pool.monsterPrefab, poolParent);
        pool.poolQueue.Enqueue(newObj); 
        newObj.SetActive(true);
        return newObj;
    }

    public void ReturnPool(string monsterId, GameObject monsterObj)
    {
        Debug.Log("오브젝트 꺼주고 있나 ?");
        monsterObj.SetActive(false);

        if (poolDictionary.ContainsKey(monsterId))
        {
            poolDictionary[monsterId].poolQueue.Enqueue(monsterObj);
        }
        else
        {
            if (monsterPools.Exists(p => p.monsterId == monsterId))
            {
                poolDictionary[monsterId].poolQueue = new Queue<GameObject>();
                poolDictionary[monsterId].poolQueue.Enqueue(monsterObj);
            }
        }
    }
}
