using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeManager : MonoBehaviour
{
    public SkillSlot[] skillSlots;
    public TextMeshProUGUI pointText;
    public int availablePoints;

    private void Start()
    {
        foreach (SkillSlot slot in skillSlots)
        {
            slot.skillButton.onClick.AddListener(() => CheckAvailablePoints(slot));
        }
        UpdatePointText(0);
    }

    private void CheckAvailablePoints(SkillSlot slot)
    {
        if(availablePoints > 0)
        {
            slot.TryUpgradeSkill();
        }
    }
    
    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
        SkillSlot.OnSkillMaxed += HandleSkillMaxed;
        ExpManager.OnLevelUp += UpdatePointText;
    }

    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
        SkillSlot.OnSkillMaxed -= HandleSkillMaxed;
        ExpManager.OnLevelUp -= UpdatePointText;
    }

    public void UpdatePointText(int points)
    {
        availablePoints += points;
        pointText.text ="Skill Points: " + availablePoints.ToString();
    }

    private void HandleAbilityPointSpent(SkillSlot slot)
    {
        if(availablePoints > 0)
        {
   
            UpdatePointText(-1);
        }
    }

    private void HandleSkillMaxed(SkillSlot slot)
    {
        foreach (SkillSlot s in skillSlots)
        {
            if(!s.isUnlocked && s.CanUnlockSkill())
            {
                s.Unlock();
            }
        }
    }
}
