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
                Debug.LogWarning("MinimapIcons 오브젝트를 찾을 수 없습니다.");
                return;
            }
        }

        // 아이콘 생성
        GameObject iconObj = Instantiate(iconPrefab, iconParent);

        // RectTransform 가져와서 iconUI에 연결
        icon = GetComponent<MinimapIcon>();
        icon.iconUI = iconObj.GetComponent<RectTransform>();

        // 미니맵 매니저에 등록
        MinimapIconManager manager = FindObjectOfType<MinimapIconManager>();
        if (manager != null)
        {
            manager.Register(icon);
        }
        else
        {
            Debug.LogWarning("MinimapIconManager를 찾을 수 없습니다.");
        }
    }
}
