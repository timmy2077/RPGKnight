using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Item", menuName = "Item/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea(3,10)]
    public string itemDescription;
    public Sprite itemIcon;

    public bool isGold;
    public bool isEXP;
    public int stackSize=6;

    [Header("Stats")]
    public int currentHealth;
    public int maxHealth;
    public int speed;
    public int damage;
    [Header("For Temporary Items")]
    public float duration;
}
