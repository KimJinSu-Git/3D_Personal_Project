using System.Collections;
using System.Collections.Generic;
using Suntail;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Slider greenHealthSlider;    // 초록색 체력 (실시간 감소)
    [SerializeField] private Slider whiteHealthSlider;    // 흰색 체력 (지연 감소)
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Animation Settings")]
    [SerializeField] private float delayReduceHp = 0.3f;
    [SerializeField] private float reduceLerpSpeed = 5f;

    private int lastHp = -1;
    private Coroutine drainCoroutine;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerController>();

        UpdateHealthUI(force: true);
    }

    private void Update()
    {
        UpdateHealthUI();
        UpdateLevelUI();
    }
    
    private void UpdateLevelUI()
    {
        if (player != null)
        {
            levelText.text = $"Lv.{player.playerLevel}";
        }
    }

    private void UpdateHealthUI(bool force = false)
    {
        if (player == null) return;

        int currentHp = player.playerCurrentHp;
        int maxHp = player.playerMaxHp;

        greenHealthSlider.maxValue = maxHp;
        whiteHealthSlider.maxValue = maxHp;

        greenHealthSlider.value = currentHp;
        healthText.text = $"{currentHp:N0} / {maxHp:N0}";

        if (force || lastHp == -1)
        {
            whiteHealthSlider.value = currentHp;
            lastHp = currentHp;
            return;
        }

        if (currentHp < lastHp)
        {
            if (drainCoroutine != null) StopCoroutine(drainCoroutine);
            drainCoroutine = StartCoroutine(DrainWhiteBar(lastHp, currentHp));
        }
        else if (currentHp > whiteHealthSlider.value)
        {
            whiteHealthSlider.value = currentHp;
        }

        lastHp = currentHp;
    }

    private IEnumerator DrainWhiteBar(float from, float to)
    {
        yield return new WaitForSeconds(delayReduceHp);

        float current = from;

        while (Mathf.Abs(current - to) > 0.1f)
        {
            current = Mathf.Lerp(current, to, Time.deltaTime * reduceLerpSpeed);
            whiteHealthSlider.value = current;
            yield return null;
        }

        whiteHealthSlider.value = to;
    }
}