using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoldManager : MonoBehaviour
{
    public static PlayerGoldManager Instance;

    public int gold = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"골드 획득: +{amount}G (총 보유: {gold})");
        // 추후 UI 갱신
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount)
        {
            Debug.Log("골드 부족 !");
            return false;
        }
        gold -= amount;
        Debug.Log($"골드 사용: -{amount}G (총 보유: {gold})");
        return true;
    }
}
