using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private ShopSlot[] shopSlots;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject shopCanvas;
    public static event Action <ShopManager, bool> OnShopStateChanged;


    private void Start()
    {
        PopulateShopItems();
        OnShopStateChanged?.Invoke(this, false);
    }

        private void Update()
    {
        if(Input.GetButtonDown("ToggleShop"))
        {
            bool newState = !shopCanvas.activeSelf; 
            shopCanvas.SetActive(newState);
            // 打开面板时暂停游戏，关闭面板时恢复游戏
            Time.timeScale = newState ? 0 : 1;
            OnShopStateChanged?.Invoke(this, newState);
        }
    }

    public void PopulateShopItems()
{

    
    for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
    {
        if (shopSlots[i] != null)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialize(shopItem.itemSO, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }
    }

    for (int i = shopItems.Count; i < shopSlots.Length; i++)
    {
        if (shopSlots[i] != null)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }
}

    public void TryBuyItem(ItemSO itemSO, int price)
{
    if(itemSO != null && inventoryManager != null && inventoryManager.gold >= price)
    {
        if(HasSpaceForItem(itemSO))
        {
            inventoryManager.gold -= price;
            if (inventoryManager.goldText != null)
            {
                inventoryManager.goldText.text = inventoryManager.gold.ToString();
            }
            inventoryManager.AddItem(itemSO, 1);
        }
    }
}

private bool HasSpaceForItem(ItemSO itemSO)
{
    if (inventoryManager == null || inventoryManager.itemSlots == null)
    {
        return false;
    }
    
    foreach (var slot in inventoryManager.itemSlots)
    {
        if (slot != null)
        {
            if (slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
                return true;
            else if (slot.itemSO == null)
                return true;
        }
    }
    return false;
}

    public void SellItem(ItemSO itemSO)
    {
        if (itemSO == null || inventoryManager == null)
            return;

        if (shopSlots != null)
        {
            foreach (var slot in shopSlots)
            {
                if (slot != null && slot.itemSO == itemSO)
                {
                    inventoryManager.gold += slot.price/2;
                    if (inventoryManager.goldText != null)
                    {
                        inventoryManager.goldText.text = inventoryManager.gold.ToString();
                    }
                    return;
                }
            }
        }
    }

}

[System.Serializable]
public class ShopItems
{
    public ItemSO itemSO;
    public int price;
}
