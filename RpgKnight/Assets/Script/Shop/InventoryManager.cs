using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int gold;
    public UseItem useItem;
    public TextMeshProUGUI goldText;
    public Inventory[] itemSlots;
    public GameObject lootPrefab;
    public Transform player;

    public static event System.Action<int> OnEXperienceGained;
    public static event Action OnInventoryChanged;

    private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }
}

    private void Start()
    {
        foreach (var slot in itemSlots)
            {
                slot.UpdateUI();
            }
        goldText.text = gold.ToString();
    }

    private void OnEnable()
    {
        Loot.OnItemLooted += AddItem;
    }

    private void OnDisable()
    {
        Loot.OnItemLooted -= AddItem;
    }

    public void AddItem(ItemSO itemSO, int quantity)
    {
        if (itemSO.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            return;
        }

        if(itemSO.isEXP)
        {
            OnEXperienceGained?.Invoke(quantity);
            return;
        }

        foreach (var slot in itemSlots)    //It it is the SAME item AND there is ROOM left
{
    if(slot.itemSO == itemSO && slot.quantity < itemSO.stackSize)
    {
        int availableSpace = itemSO.stackSize - slot.quantity;
        int amountToAdd = Mathf.Min(availableSpace, quantity);
        slot.quantity += amountToAdd;
        quantity -= amountToAdd;
        slot.UpdateUI();
        if(quantity <= 0)
        {
            NotifyInventoryChanged();
            return;
        }
    }
}

            foreach (var slot in itemSlots)
            {
                if (slot.itemSO == null)
                {
                    int amountToAdd = Mathf.Min(itemSO.stackSize, quantity);
                    slot.itemSO = itemSO;
                    slot.quantity = quantity;
                    slot.UpdateUI();
                    NotifyInventoryChanged();
                    return;
                }
            }
        if(quantity > 0)
        {
            DropLoot(itemSO, quantity);
        }

        NotifyInventoryChanged();
    }

    public void RemoveItem(ItemSO itemSO, int quantity)
{
    for (int i = 0; i < itemSlots.Length; i++)
    {
        var slot = itemSlots[i];

        //Skip slots that don't match the item
        if (slot.itemSO != itemSO)
            continue;

        if(slot.quantity > quantity)
        {
            //Remove only what we need
            slot.quantity -= quantity;
            slot.UpdateUI();
            quantity = 0;
        }
        else
        {
            //take ALL from this slot
            quantity -= slot.quantity;
            slot.itemSO = null;
            slot.quantity = 0;
            slot.UpdateUI();
        }
    }

    NotifyInventoryChanged();
}

    public void DropItem(Inventory slot)
    {
        if (slot.itemSO != null && slot.quantity > 0)
        {
            DropLoot(slot.itemSO, slot.quantity);
            slot.itemSO = null;
            slot.quantity = 0;
            slot.UpdateUI();
            NotifyInventoryChanged();
        }
    }

    private void DropLoot(ItemSO itemSO, int quantity)
    {
       
        Loot loot = Instantiate(lootPrefab, player.position,Quaternion.identity).GetComponent<Loot>();
        loot. Initialize(itemSO, quantity); 
    }



    public void UseItem(Inventory slot)
    {
        if (slot.itemSO != null && slot.quantity > 0)
        {
            useItem.ApplyItemEffects(slot.itemSO);
            slot.quantity--;
            if(slot.quantity <= 0)
            {
                slot.itemSO = null;
            }
            slot.UpdateUI();
            NotifyInventoryChanged();
        }
    }

    public bool HasItem(ItemSO itemSO)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemSO == itemSO && slot.quantity > 0)
            {
                return true;
            }
        }
        return false;
    }

    public int GetItemQuantity(ItemSO itemSO)
{
    int total = 0;

    foreach (var slot in itemSlots)
    {
        if (slot.itemSO == itemSO)
            total += slot.quantity;
    }

    return total;
}

    private void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }
}
