using UnityEngine;

public enum ItemType { Potion, Equipment}

[System.Serializable]
public class ItemData
{
    public string id; // 아이템 ID
    public string itemName; // 아이템 이름
    public string description; // 아이템 설명
    public Sprite icon; // 아이템 이미지
    public ItemType itemType; // 아이템 타입, 소비물약이냐 장비아이템이냐 등등 타입 구분할 용도.. 또 뭐가 있으면 좋을까
    
    public int price; // 아이템 가격 (상점 연동)
    public bool isDuplicate; // 중복되는 아이템인지 체크
    public int maxStack; // 중복이 되는 아이템이면 몇개까지 겹쳐질수 있는지 ex) 회복물약 x99 이런 느낌으로 만들자.

    public int healAmount; // 소비물약일 때 hp 회복량
    public bool percentHealthPotion; // healAmount 와 다르게 고정수치가 아닌 플레이어의 hp 비율로 회복하는 아이템인지 확인
    public float healPercent; // (0.1 = 10%) 힐 되는 용도
}