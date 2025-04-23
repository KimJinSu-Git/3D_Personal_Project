using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 30f, 0);

    void LateUpdate()
    {
        if (target == null) return;

        // 위치는 항상 플레이어의 위쪽에 위치.
        transform.position = target.position + offset;

        // 플레이어의 Y축 회전만 따라가도록.
        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
    }
}
