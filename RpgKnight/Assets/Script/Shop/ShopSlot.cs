using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public ItemSO itemSO;
    // public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;
    public Image itemImage;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopInfo shopInfo; 
    public int price;

    public void Initialize(ItemSO newItemSO, int price)
    {
    //fill the slot with information
    itemSO = newItemSO;
    itemImage.sprite = itemSO.itemIcon;
    // itemNameText.text = itemSO.itemName;
    this.price = price;
    priceText.text = price.ToString();
    }

    public void OnBuyButtonClick()
    {
        shopManager.TryBuyItem(itemSO, price);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemSO != null)
        shopInfo.ShowItemInfo(itemSO);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopInfo.HideItemInfo();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(itemSO != null)
        shopInfo.FollowMouse();
    }
}
