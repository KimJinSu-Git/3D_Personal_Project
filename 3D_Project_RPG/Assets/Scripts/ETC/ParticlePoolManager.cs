using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePoolManager : MonoBehaviour
{
    public static ParticlePoolManager Instance;

    [System.Serializable]
    public class ParticlePool
    {
        public string poolName;
        public GameObject particlePrefab;
        public int poolSize = 3;
        [HideInInspector] public Queue<GameObject> poolQueue;
    }

    public List<ParticlePool> particlePools;
    private Dictionary<string, ParticlePool> poolDictionary;

    private void Awake()
    {
        Instance = this;

        poolDictionary = new Dictionary<string, ParticlePool>();

        foreach (var pool in particlePools)
        {
            pool.poolQueue = new Queue<GameObject>();
            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject particleObj = Instantiate(pool.particlePrefab);
                particleObj.SetActive(false);

                if (particleObj.GetComponent<ParticleAutoDisable>() == null)
                {
                    particleObj.AddComponent<ParticleAutoDisable>();
                }

                pool.poolQueue.Enqueue(particleObj);
            }
            poolDictionary.Add(pool.poolName, pool);
        }
    }

    public GameObject StartPool(string poolName, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            return null;
        }

        ParticlePool pool = poolDictionary[poolName];
        GameObject particleObj = pool.poolQueue.Dequeue();
        particleObj.SetActive(true);
        particleObj.transform.position = position;
        particleObj.transform.rotation = rotation;

        ParticleSystem ps = particleObj.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }

        pool.poolQueue.Enqueue(particleObj);

        return particleObj;
    }
}

public class ParticleAutoDisable : MonoBehaviour
{
    private ParticleSystem pSystem;

    private void OnEnable()
    {
        if (pSystem == null)
        {
            pSystem = GetComponent<ParticleSystem>();
        }
        pSystem.Play();
        StartCoroutine(DisableTime(pSystem.main.duration + pSystem.main.startLifetime.constantMax));
    }

    private IEnumerator DisableTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}