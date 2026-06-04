using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class QuestRewardSlot : MonoBehaviour
{
    public Image rewardImage;
    public TMP_Text rewardQuantity;

public void DisplayReward(Sprite sprite, int quantity)
{
    rewardImage.sprite = sprite;
    rewardQuantity.text = quantity.ToString();
}
}
