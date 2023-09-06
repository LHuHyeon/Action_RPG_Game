using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 판매할 아이템과 금액 클래스
[System.Serializable]
public class Sale
{
    public Item item;
    public int coin;
}

[CreateAssetMenu(fileName = "New Shop", menuName = "New Shop/shop")]
public class Shop : ScriptableObject
{
    public Sale[] sales;
}
