using System.Collections;
using System.Collections.Generic;
using Suntail;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public const int MaxSlotCount = 20;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>(MaxSlotCount);
    
    public Transform slotParent;
    public GameObject slotPrefab;
    public TMP_Text goldText;
    
    [SerializeField] private GameObject inventoryPanel;
    // [SerializeField] private GameObject toolTipPanel;
    [SerializeField] private PlayerController playerController;

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < MaxSlotCount; i++)
        {
            inventorySlots.Add(null);
        }

        UpdateUI();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && DialogueUI.Instance.dialoguePanel.activeSelf == false && ShopManager.Instance.IsOpen == false)
        {
            if (inventoryPanel.activeSelf)
            {
                inventoryPanel.SetActive(false);
                TooltipUI.Instance.Hide();
                // toolTipPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                playerController.isInventoryOpen = false;
            }
            else
            {
                inventoryPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                playerController.isInventoryOpen = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddItem("potion_small", 1);
        }
        
        goldText.text = $"Gold: {PlayerGoldManager.Instance.gold} G";
    }

    public void AddItem(string itemId, int amount)
    {
        ItemData data = ItemDatabase.Instance.GetItemByID(itemId);
        if (data == null) return;

        // 아이템이 동일하고, 중복이 가능한 아이템이면 수량만 증가시키고
        if (data.isDuplicate)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot != null && slot.itemData != null && slot.itemData.id == itemId && slot.quantity < data.maxStack)
                {
                    slot.AddQuantity(amount);
                    UpdateUI();
                    return;
                }
            }
        }

        // 중복이 안되는 친구면 빈 슬롯에다가 추가 시키기.
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i] == null || inventorySlots[i].itemData == null)
            {
                inventorySlots[i] = new InventorySlot(data, amount);
                UpdateUI();
                return;
            }
        }

        Debug.Log("인벤토리에 빈 슬롯이 없워");
    }
    
    public void SwapSlots(InventorySlotUI a, InventorySlotUI b)
    {
        int indexA = a.slotIndex;
        int indexB = b.slotIndex;
        
        (inventorySlots[indexA], inventorySlots[indexB]) = (inventorySlots[indexB], inventorySlots[indexA]);

        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            InventorySlot slot = inventorySlots[i];
            InventorySlotUI slotUI = obj.GetComponent<InventorySlotUI>();
            slotUI.SetSlot(slot, i);
            slotUI.slotIndex = i;
        }
    }
}
