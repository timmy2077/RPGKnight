using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "QuestSO")]
public class QuestSO : ScriptableObject
{
    public string questName;
    [TextArea] public string questDescription;
    public int questLevel;

    public List<QuestObjective> objectives;
    public List<QuestReward> rewards;
}

[System.Serializable]
public class QuestObjective
{
    public string description;

    public ItemSO targetItem;
    public ActorSO targetNPC;
    public LocationSO targetLocation;
    public EnemySO targetEnemy;

    public int requiredAmount;
}

[System.Serializable]
public class QuestReward
{
    public ItemSO itemSO;
    public int quantity;
}