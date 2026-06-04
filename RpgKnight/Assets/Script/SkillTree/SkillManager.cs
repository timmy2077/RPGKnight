using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Player_Combat playerCombat;
    private int speedBoostCounter = 0;

    private void OnEnable()
    {
        SkillSlot.OnAbilityPointSpent += HandleAbilityPointSpent;
    }

    private void OnDisable()
    {
        SkillSlot.OnAbilityPointSpent -= HandleAbilityPointSpent;
    }

    private void HandleAbilityPointSpent(SkillSlot slot)
    {
        string skillName = slot.skillSO.skillName;
        switch (skillName)
        {
            case "Max Health Boost":
                StatsManager.Instance.UpdateMaxHealth(1);
                break;
            case "Max Attack Boost":
                StatsManager.Instance.UpdateDamage(1);
                break;
            case "Max Speed Boost":
                speedBoostCounter++;
                if (speedBoostCounter >= 5)
                {
                    StatsManager.Instance.UpdateSpeed(1);
                    speedBoostCounter = 0;
                }
                break;        
            default:
                Debug.LogError("Skill not found: " + skillName);
                break;
        }
    }

    
}
