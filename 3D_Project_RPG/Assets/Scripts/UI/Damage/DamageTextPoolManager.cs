using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextPoolManager : MonoBehaviour
{
    public static DamageTextPoolManager Instance;

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Transform poolParent;
    [SerializeField] private int poolSize = 15;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject textObj = Instantiate(damageTextPrefab, poolParent);
            textObj.SetActive(false);
            pool.Enqueue(textObj);
        }
    }

    public GameObject GetPool()
    {
        if (pool.Count > 0)
        {
            GameObject textObj = pool.Dequeue();
            textObj.SetActive(true);
            return textObj;
        }

        GameObject newObj = Instantiate(damageTextPrefab, poolParent);
        return newObj;
    }

    public void ReturnPool(GameObject textObj)
    {
        textObj.SetActive(false);
        pool.Enqueue(textObj);
    }
}
