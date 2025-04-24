using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float lifetime = 1f;

    private float timer;
    private Camera mainCamera;

    public void SetText(int damage)
    {
        text.text = damage.ToString();
        timer = lifetime;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 바라보기
        if (mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }

        transform.position += Vector3.up * (floatSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            DamageTextPoolManager.Instance.ReturnPool(gameObject);
        }
    }
}
