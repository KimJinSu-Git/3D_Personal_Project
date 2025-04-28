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
