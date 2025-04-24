using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string id; // 아이템 ID
    public string itemName; // 아이템 이름
    public string description; // 아이템 설명
    public Sprite icon; // 아이템 이미지
    public int price; // 아이템 가격 (상점 연동)
    public bool isDuplicate; // 중복되는 아이템인지 체크
    public int maxStack; // 중복이 되는 아이템이면 몇개까지 겹쳐질수 있는지 ex) 회복물약 x99 이런 느낌으로 만들자.
}