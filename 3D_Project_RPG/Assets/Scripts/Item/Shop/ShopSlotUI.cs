using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    private ItemData itemData;

    public void SetSlot(ItemData data)
    {
        itemData = data;

        icon.sprite = data.icon;
        nameText.text = data.itemName;
        priceText.text = $"{data.price} G";

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            ShopManager.Instance.BuyItem(itemData);
        });
    }
}
