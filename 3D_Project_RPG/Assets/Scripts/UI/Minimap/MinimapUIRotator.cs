using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapUIRotator : MonoBehaviour
{
    [SerializeField] private Transform target;

    void Update()
    {
        if (target == null) return;

        // 플레이어가 보는 방향의 반대로 UI를 회전
        transform.rotation = Quaternion.Euler(0, 0, -target.eulerAngles.y);
    }
}
