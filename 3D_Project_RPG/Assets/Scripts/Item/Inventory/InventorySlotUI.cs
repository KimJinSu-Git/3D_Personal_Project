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
    
    public void SetSlot(InventorySlot slot, int index)
    {
        this.slot = slot;
        this.slotIndex = index;

        if (slot != null && slot.itemData != null)
        {
            icon.sprite = slot.itemData.icon;
            icon.color = Color.red; // 임시 색깔
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
        QuickSlotManager.Instance.RefreshAllSlots();
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
                    // return;
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
        DragItemUI.Instance.EndDrag(); // 드래그 중인 아이템 이미지 꺼주고.

        GameObject targetObject = eventData.pointerCurrentRaycast.gameObject; // 마우스가 종료된 위치에 타겟오브젝트를 찾고.
        if (targetObject == null) return; // 타겟 오브젝트가 없다면 슬롯이 아니므로 그대로 종료.
        InventorySlotUI targetSlotUI = targetObject.GetComponentInParent<InventorySlotUI>(); // 있다면 타겟의 부모한테서 인벤토리슬롯UI를 가져와서 담아준다.
        if (targetSlotUI != null && targetSlotUI != this)
        {
            InventoryManager.Instance.SwapSlots(this, targetSlotUI); // 기존 아이템과 마우스 포인터가 끝난 슬롯 아이템의 위치를 바꾼다.
        }
    }
}