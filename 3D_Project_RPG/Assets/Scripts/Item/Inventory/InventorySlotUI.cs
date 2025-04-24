using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public TMP_Text quantityText;

    private InventorySlot slot;

    public void SetSlot(InventorySlot slot)
    {
        this.slot = slot;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log($"이 아이템이 우클릭 됐어요: {slot.itemData.itemName}");
            // 나중에 상점 판매 / 아이템 사용 연결 할 곳.
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slot is { itemData: not null })
        {
            TooltipUI.Instance.Show(slot.itemData, Input.mousePosition);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }
}