using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPUI : MonoBehaviour
{
    public static MonsterHPUI Instance;

    public GameObject panel;
    public Slider hpSlider;
    public TMP_Text nameText;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowMonsterHP(string monsterName, int currentHp, int maxHp)
    {
        if (panel.activeSelf == false)
            panel.SetActive(true);

        nameText.text = monsterName;
        hpSlider.value = (float)currentHp / maxHp;

        // 코루틴이 있다면 멈추기
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        // 3초 뒤 패널 꺼지도록
        yield return new WaitForSeconds(3f);
        panel.SetActive(false);
    }

    public void UpdateHP(int currentHp, int maxHp)
    {
        if (panel.activeSelf)
        {
            hpSlider.value = (float)currentHp / maxHp;
        }
    }

    // 몬스터가 죽으면 패널 숨기자
    public void PanelHide()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        panel.SetActive(false);
    }
}
