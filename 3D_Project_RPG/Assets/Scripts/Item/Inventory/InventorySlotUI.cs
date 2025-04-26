using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    public TMP_Text quantityText;
    public int slotIndex;

    private InventorySlot slot;

    public InventorySlot Slot => slot;
    
    public InventorySlot GetSlotData()
    {
        return slot;
    }

    public void SetSlot(InventorySlot slot, int index)
    {
        this.slot = slot;
        this.slotIndex = index;

        if (slot != null && slot.itemData != null)
        {
            icon.sprite = slot.itemData.icon;
            icon.enabled = true;
            quantityText.text = slot.quantity.ToString();
        }
        else
        {
            icon.sprite = null;
            icon.enabled = false;
            quantityText.text = "";
        }
    }
    
    void SellItem()
    {
        int sellPrice = Mathf.FloorToInt(slot.itemData.price * 0.5f); // 반값 세일 !!
        PlayerGoldManager.Instance.AddGold(sellPrice);
        slot.ReduceQuantity(1);

        if (slot.quantity <= 0)
        {
            slot.itemData = null;
        }

        InventoryManager.Instance.UpdateUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"이 아이템이 우클릭 됐어요: {slot.itemData.itemName}");
            if (slot != null && slot.itemData != null)
            {
                if (ShopManager.Instance.IsOpen)
                {
                    SellItem();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slot is { itemData: not null } && TooltipUI.Instance != null)
        {
            TooltipUI.Instance.Show(slot.itemData, Input.mousePosition);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Hide();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot == null || slot.itemData == null) return;
        DragItemUI.Instance.StartDrag(this, icon.sprite);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragItemUI.Instance.UpdatePosition(Input.mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragItemUI.Instance.EndDrag();

        GameObject targetObject = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log("Raycast Target: " + targetObject?.name);
        if (targetObject == null) return;

        InventorySlotUI targetSlotUI = targetObject.GetComponentInParent<InventorySlotUI>();
        
        if (targetSlotUI != null && targetSlotUI != this)
        {
            InventoryManager.Instance.SwapSlots(this, targetSlotUI);
        }
        else
        {
            Debug.Log("타겟 슬롯 UI를 감지하지 못했어용용죽겠지용");
        }
    }
}