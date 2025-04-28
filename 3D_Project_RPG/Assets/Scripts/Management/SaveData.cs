using System;
using System.Collections.Generic;
using System.Numerics;

// 저장할 데이터만 담을 클래스
[Serializable]
public class SaveData
{
    // 플레이어 정보
    public int playerLevel;
    public int playerCurrentExp;
    public int playerRequiredExp;
    public int playerCurrentHp;
    public int playerMaxHp;
    public int playerGold;
    
    // 플레이어 위치 정보
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    
    // 퀘스트 정보
    public List<string> activeQuestIDs = new List<string>();
    public List<int> activeQuestCounts = new List<int>();

    // 인벤토리 슬롯 정보
    public List<InventorySlotData> inventorySlots = new List<InventorySlotData>();

    // 퀵슬롯 정보
    public List<int> quickSlotIndexes = new List<int>(); 
}

[Serializable]
public class InventorySlotData
{
    public string itemId; 
    public int quantity; 
}