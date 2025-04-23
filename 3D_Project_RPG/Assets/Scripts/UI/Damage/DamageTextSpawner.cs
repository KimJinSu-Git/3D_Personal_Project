using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance;

    [SerializeField] private Transform worldCanvas;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnDamageText(Vector3 worldPos, int damage)
    {
        GameObject textObj = DamageTextPoolManager.Instance.GetPool();
        textObj.transform.SetParent(worldCanvas);
        textObj.transform.position = worldPos;

        DamageText dmgText = textObj.GetComponent<DamageText>();
        dmgText.SetText(damage);
    }
}
