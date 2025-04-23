using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIconManager : MonoBehaviour
{
    [SerializeField] private RectTransform minimapUI;
    [SerializeField] private Transform player;
    [SerializeField] private float mapScale = 3f; 

    private List<MinimapIcon> icons = new List<MinimapIcon>();

    public void Register(MinimapIcon icon)
    {
        if (!icons.Contains(icon))
            icons.Add(icon);
    }

    private void LateUpdate()
    {
        foreach (MinimapIcon icon in icons)
        {
            if (icon.iconUI == null) continue;

            Vector3 worldOffset = icon.transform.position - player.position;

            Vector2 offset = new Vector2(worldOffset.x, worldOffset.z);

            // 회전 보정
            float angle = player.eulerAngles.y * Mathf.Deg2Rad;
            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            float rotatedX = offset.x * cos + offset.y * sin;
            float rotatedY = -offset.x * sin + offset.y * cos;

            icon.iconUI.anchoredPosition = new Vector2(rotatedX, rotatedY) * mapScale;
        }
    }
}
