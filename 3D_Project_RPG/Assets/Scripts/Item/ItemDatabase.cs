using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    
    public Dictionary<string, ItemData> items =  new Dictionary<string, ItemData>();

    private void Awake()
    {
        Instance = this;
        
        LoadItemsCsv();
    }

    private void LoadItemsCsv()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Data/items");
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일이 없어요 확인 부탁드려요. 경로:Resources/Data/items.csv");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] parts = lines[i].Split(',');

            ItemData item = new ItemData
            {
                id = parts[0],
                itemName = parts[1],
                description = parts[2],
                icon = Resources.Load<Sprite>(parts[3]),

                price = int.Parse(parts[4]),
                isDuplicate = bool.Parse(parts[5]),
                maxStack = int.Parse(parts[6]),
    
                itemType = Enum.Parse<ItemType>(parts[7].Trim()),

                healAmount = int.Parse(parts[8]),
                percentHealthPotion = bool.Parse(parts[9]),
                healPercent = float.Parse(parts[10])
            };
            
            items.TryAdd(item.id, item);
        }

        Debug.Log($"로드된 아이템 갯수: {items.Count}");
    }

    public ItemData GetItemByID(string id)
    {
        // if (items.TryGetValue(id, out ItemData item))
        //     return item;
        // return null;        => 아래와 같은 코드
        return items.GetValueOrDefault(id);
    }
}
