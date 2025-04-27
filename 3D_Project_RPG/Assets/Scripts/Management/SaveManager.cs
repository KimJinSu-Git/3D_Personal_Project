using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Suntail;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private string saveFilePath;

    private void Awake()
    {
        Instance = this;
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
        // C:/사용자/사용자이름/AppData/LocalLow/회사이름/게임이름/save.json  <= 평균적인 경로 위치
    }

    private void Start()
    {
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame(); // 게임 끌 때 바로 저장
    }
    
    public void SaveGame()
    {
        SaveData data = new SaveData();

        // 플레이어 정보 저장
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            data.playerLevel = player.playerLevel;
            data.playerCurrentExp = player.currentExp;
            data.playerRequiredExp = player.requiredExp;
            data.playerCurrentHp = player.playerCurrentHp;
            data.playerMaxHp = player.playerMaxHp;
            data.playerGold = PlayerGoldManager.Instance.gold;
        }

        // 인벤토리 저장
        foreach (var slot in InventoryManager.Instance.inventorySlots)
        {
            if (slot != null && slot.itemData != null)
            {
                InventorySlotData slotData = new InventorySlotData
                {
                    itemId = slot.itemData.id,
                    quantity = slot.quantity
                };
                data.inventorySlots.Add(slotData);
            }
            else
            {
                data.inventorySlots.Add(null); // 빈 슬롯은 null 저장
            }
        }

        // 퀵슬롯 저장
        foreach (var quickSlot in QuickSlotManager.Instance.quickSlots)
        {
            if (quickSlot.GetSlotData() != null)
            {
                int index = InventoryManager.Instance.inventorySlots.IndexOf(quickSlot.GetSlotData());
                data.quickSlotIndexes.Add(index);
            }
            else
            {
                data.quickSlotIndexes.Add(-1); // 등록 안된 슬롯
            }
        }

        // Json 변환
        string json = JsonUtility.ToJson(data, true); // true는 보기 좋게 포맷팅을 시켜주고

        // 파일 저장
        File.WriteAllText(saveFilePath, json);

        Debug.Log($"게임 저장 완룡 ! 저장된 경로는 => {saveFilePath}");
    }
    
    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("저장 파일이 없기 때문에 새로운 게임으로 시작할게용");
            return;
        }
    
        // 저장된 Json 읽어오기
        string json = File.ReadAllText(saveFilePath);
    
        // Json을 SaveData 객체로 변환
        SaveData data = JsonUtility.FromJson<SaveData>(json);
    
        // 플레이어 상태 복구
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.playerLevel = data.playerLevel;
            player.currentExp = data.playerCurrentExp;
            player.requiredExp = data.playerRequiredExp;
            player.playerCurrentHp = data.playerCurrentHp;
            player.playerMaxHp = data.playerMaxHp;
            PlayerGoldManager.Instance.gold = data.playerGold;
        }
    
        // 인벤토리 복구
        InventoryManager inventory = InventoryManager.Instance;
        inventory.inventorySlots.Clear();
    
        foreach (var slotData in data.inventorySlots)
        {
            if (slotData != null)
            {
                ItemData item = ItemDatabase.Instance.GetItemByID(slotData.itemId);
                if (item != null)
                    inventory.inventorySlots.Add(new InventorySlot(item, slotData.quantity));
                else
                    inventory.inventorySlots.Add(null); // 아이템 ID 못 찾으면 빈 슬롯 처리로 우선 에러방지
            }
            else
            {
                inventory.inventorySlots.Add(null);
            }
        }
    
        inventory.UpdateUI();
    
        // 퀵슬롯 복구
        QuickSlotManager quickSlotManager = QuickSlotManager.Instance;
    
        for (int i = 0; i < quickSlotManager.quickSlots.Count; i++)
        {
            if (i < data.quickSlotIndexes.Count)
            {
                int index = data.quickSlotIndexes[i];
                if (index >= 0 && index < inventory.inventorySlots.Count)
                {
                    quickSlotManager.quickSlots[i].SetSlot(inventory.inventorySlots[index]);
                }
                else
                {
                    quickSlotManager.quickSlots[i].SetSlot(null);
                }
            }
            else
            {
                quickSlotManager.quickSlots[i].SetSlot(null);
            }
        }
    
        Debug.Log("게임 불러오기 완료!");
    }
}
