using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragItemUI : MonoBehaviour
{
    public static DragItemUI Instance;
    public Image dragIcon;

    private void Awake()
    {
        Instance = this;
        dragIcon.gameObject.SetActive(false);
    }

    public void StartDrag(InventorySlotUI sourceSlot, Sprite iconSprite)
    {
        dragIcon.sprite = iconSprite;
        dragIcon.gameObject.SetActive(true);
        UpdatePosition(Input.mousePosition);
    }

    public void UpdatePosition(Vector3 pos)
    {
        dragIcon.transform.position = pos;
    }

    public void EndDrag()
    {
        dragIcon.gameObject.SetActive(false);
    }
}
