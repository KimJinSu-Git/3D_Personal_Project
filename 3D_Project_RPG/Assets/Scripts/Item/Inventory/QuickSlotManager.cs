using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager Instance;

    public List<QuickSlotUI> quickSlots = new List<QuickSlotUI>();

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        // 인벤토리 매니저에서 potion_small 아이템 하나 가져와서 슬롯에 넣기
        var potion = ItemDatabase.Instance.GetItemByID("potion_small");
        if (potion != null)
        {
            InventorySlot fakeSlot = new InventorySlot(potion, 1);
            quickSlots[0].SetSlot(fakeSlot); // 1번 슬롯에 수동 할당
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseQuickSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseQuickSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseQuickSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseQuickSlot(3);
    }

    public void UseQuickSlot(int index)
    {
        if (index < 0 || index >= quickSlots.Count) return;
        quickSlots[index].UseItem();
    }
    
    public void RefreshAllSlots()
    {
        foreach (var slot in quickSlots)
        {
            slot.RefreshIcon();
        }
    }
}
