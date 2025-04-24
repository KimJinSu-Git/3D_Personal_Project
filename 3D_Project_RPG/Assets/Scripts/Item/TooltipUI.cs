using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public TMP_Text itemNameText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;

    private float halfwidth;
    private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
        halfwidth = GetComponentInParent<CanvasScaler>().referenceResolution.x * 0.5f;
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
    
    public void Show(ItemData data, Vector3 pos)
    {
        itemNameText.text = data.itemName;
        descriptionText.text = data.description;
        priceText.text = $"{data.price}";

        transform.position = pos;

        if (rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x > halfwidth)
        {
            rectTransform.pivot = new Vector2(1, 1);
        }
        else
        {
            rectTransform.pivot = new Vector2(0, 1);
        }
        
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
