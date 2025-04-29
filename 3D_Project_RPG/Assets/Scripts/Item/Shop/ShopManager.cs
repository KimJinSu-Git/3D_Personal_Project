using System;
using System.Collections;
using System.Collections.Generic;
using Suntail;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public Transform shopSlotParent;
    public GameObject shopSlotPrefab;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private PlayerController player;

    private List<ItemData> shopItems = new List<ItemData>();
    
    public bool IsOpen => shopPanel.activeSelf;

    private void Awake()
    {
        Instance = this;

        shopPanel.SetActive(false);
        LoadShopCsv();
    }

    private void LateUpdate()
    {
        if (IsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShopPanel();
        }
    }

    private void LoadShopCsv()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Data/shop_items");
        if (csvFile == null)
        {
            Debug.LogError("shop_items.csv이 존재하지 않앙");
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
                maxStack = int.Parse(parts[6])
            };

            shopItems.Add(item);
        }

        GenerateShopSlots();
    }

    private void GenerateShopSlots()
    {
        foreach (Transform child in shopSlotParent)
            Destroy(child.gameObject);

        foreach (var item in shopItems)
        {
            GameObject obj = Instantiate(shopSlotPrefab, shopSlotParent);
            obj.GetComponent<ShopSlotUI>().SetSlot(item);
        }
    }

    public void BuyItem(ItemData item)
    {
        if (!PlayerGoldManager.Instance.SpendGold(item.price))
        {
            Debug.Log("구매 실패 : 골드가 부족해용");
            // TODO : UI 경고창 띄우기를 추가할까...말까.....귀찮을거같은뎅.....
            return;
        }
        
        InventoryManager.Instance.AddItem(item.id, 1);
        QuickSlotManager.Instance.RefreshAllSlots();
        Debug.Log($"{item.itemName} 구매!");
    }

    public void OpenShopPanel()
    {
        DialogueUI.Instance?.EndDialogue();
        player.ChangeState(PlayerState.Talking);
        shopPanel.SetActive(true);
        inventoryPanel.SetActive(true);
    }

    public void CloseShopPanel()
    {
        TooltipUI.Instance.Hide();
        shopPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        DialogueUI.Instance?.ShopEndDialogue();
    }
}
