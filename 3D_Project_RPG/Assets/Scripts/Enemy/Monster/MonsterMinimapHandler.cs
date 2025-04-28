using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMinimapHandler : MonoBehaviour
{
    [Header("미니맵 아이콘 설정")]
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Transform iconParent;

    private MinimapIcon icon;

    void Start()
    {
        if (iconParent == null)
        {
            GameObject found = GameObject.Find("MinimapIcons");
            if (found != null)
            {
                iconParent = found.transform;
            }
            else
            {
                Debug.LogWarning("MinimapIcons가 없슴당");
                return;
            }
        }

        GameObject iconCreate = Instantiate(iconPrefab, iconParent);
        
        icon = GetComponent<MinimapIcon>();
        icon.iconUI = iconCreate.GetComponent<RectTransform>();

        MinimapIconManager manager = FindObjectOfType<MinimapIconManager>();
        if (manager != null)
        {
            manager.Register(icon);
        }
        else
        {
            Debug.LogWarning("MinimapIconManager를 만들어 주셨나용?");
        }
    }
}
