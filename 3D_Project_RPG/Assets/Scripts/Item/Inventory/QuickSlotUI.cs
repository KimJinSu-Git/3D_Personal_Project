using System.Collections;
using System.Collections.Generic;
using Suntail;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlotUI : MonoBehaviour, IDropHandler
{
    public Image itemIcon;
    public TMP_Text quantityText;

    private InventorySlot linkedSlot; 

    public void SetSlot(InventorySlot slot)
    {
        linkedSlot = slot;
        RefreshIcon();
    }

    public void UseItem()
    {
        if (linkedSlot == null || linkedSlot.itemData == null) return;

        ItemData data = linkedSlot.itemData;

        // 포션 사용
        if (data.itemType == ItemType.Potion)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                if (data.percentHealthPotion)
                {
                    int healAmount = Mathf.RoundToInt(player.playerMaxHp * data.healPercent);
                    player.playerCurrentHp = Mathf.Min(player.playerCurrentHp + healAmount, player.playerMaxHp);
                    Debug.Log($"[퀵슬롯] 퍼센트 회복 ▶ {healAmount} 회복");
                }
                else
                {
                    player.playerCurrentHp = Mathf.Min(player.playerCurrentHp + data.healAmount, player.playerMaxHp);
                    Debug.Log($"[퀵슬롯] 고정 회복 ▶ {data.healAmount} 회복");
                }
            }
        }

        linkedSlot.ReduceQuantity(1);

        if (linkedSlot.quantity <= 0)
        {
            linkedSlot.itemData = null;
        }

        InventoryManager.Instance.UpdateUI();
        QuickSlotManager.Instance.RefreshAllSlots();
    }

    public void RefreshIcon()
    {
        if (linkedSlot != null && linkedSlot.itemData != null)
        {
            itemIcon.sprite = linkedSlot.itemData.icon;
            itemIcon.enabled = true;
            quantityText.text = linkedSlot.quantity.ToString();
            quantityText.enabled = true;
        }
        else
        {
            itemIcon.enabled = false;
            quantityText.text = "";
            quantityText.enabled = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlotUI>();
        if (draggedSlot != null)
        {
            int index = draggedSlot.slotIndex;
            InventorySlot inventorySlot = InventoryManager.Instance.inventorySlots[index];

            if (inventorySlot != null && inventorySlot.itemData != null)
            {
                SetSlot(inventorySlot);
            }
        }
    }
}
