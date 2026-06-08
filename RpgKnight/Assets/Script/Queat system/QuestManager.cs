using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : MonoBehaviour
{
    private Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();
    private Dictionary<QuestSO, QuestAcceptSource> questSources = new();
    private List<QuestSO> completedQuests = new();

    private void OnEnable()
    {
        QuestEvents.IsQuestComplete += IsQuestComplete;
        QuestEvents.OnEnemyKilled += RegisterEnemyKill;
        InventoryManager.OnInventoryChanged += HandleInventoryChanged;
    }

    private void OnDisable()
    {
        QuestEvents.IsQuestComplete -= IsQuestComplete;
        QuestEvents.OnEnemyKilled -= RegisterEnemyKill;
        InventoryManager.OnInventoryChanged -= HandleInventoryChanged;
    }

    public bool IsQuestComplete(QuestSO questSO)
    {
        if (!questProgress.ContainsKey(questSO))
            return false;

            foreach (var objective in questSO.objectives)
            {
               UpdateObjectiveProgress(questSO, objective);
            }
            foreach (var objective in questSO.objectives)
            {
               if(GetCurrentAmount(questSO, objective) < objective.requiredAmount)
                    return false;
            }
            return true;
        
    }

    public void CompleteQuest(QuestSO questSO)
    {
        
        questProgress.Remove(questSO);
        questSources.Remove(questSO);
        completedQuests.Add(questSO);

        foreach (var objective in questSO.objectives)
        {
            if(objective.targetItem != null && objective.requiredAmount > 0)
            {
                InventoryManager.Instance.RemoveItem(objective.targetItem, objective.requiredAmount);
            }
        }
        foreach (var reward in questSO.rewards)
        {
            InventoryManager.Instance.AddItem(reward.itemSO, reward.quantity);
        }
        
    }

    public bool GetCompleteQuest(QuestSO questSO)
    {
        return completedQuests.Contains(questSO);
    }


    public bool IsQuestAccepted(QuestSO questSO)
    {
        return questProgress.ContainsKey(questSO);
    }


        public void AcceptQuest(QuestSO questSO, QuestAcceptSource source)
    {
    questProgress[questSO] = new Dictionary<QuestObjective, int>();
    questSources[questSO] = source;

    foreach (var objective in questSO.objectives)
    {
        UpdateObjectiveProgress(questSO, objective);
    }
    }

    public bool WasAcceptedFrom(QuestSO questSO, QuestAcceptSource source)
    {
        return questSources.TryGetValue(questSO, out var acceptedSource) && acceptedSource == source;
    }
    

    public List<QuestSO> GetActiveQuests()
    {
        return new List<QuestSO>(questProgress.Keys);
    }

    public void RegisterEnemyKill(EnemySO enemySO)
    {
        if (enemySO == null)
            return;

        foreach (var questPair in questProgress)
        {
            var progressDict = questPair.Value;

            foreach (var objective in questPair.Key.objectives)
            {
                if (objective.targetEnemy != enemySO)
                    continue;

                if (!progressDict.TryGetValue(objective, out int currentAmount))
                    currentAmount = 0;

                if (currentAmount < objective.requiredAmount)
                    progressDict[objective] = currentAmount + 1;
            }
        }

        QuestEvents.OnQuestProgressChanged?.Invoke();
    }

    private void HandleInventoryChanged()
    {
        if (questProgress.Count == 0)
            return;

        bool hasItemObjective = false;

        foreach (var questPair in questProgress)
        {
            foreach (var objective in questPair.Key.objectives)
            {
                if (objective.targetItem == null)
                    continue;

                hasItemObjective = true;
                UpdateObjectiveProgress(questPair.Key, objective);
            }
        }

        if (hasItemObjective)
            QuestEvents.OnQuestProgressChanged?.Invoke();
    }

    public void UpdateObjectiveProgress(QuestSO questSO, QuestObjective objective)
{
    if (!questProgress.ContainsKey(questSO))
        // questProgress[questSO] = new Dictionary<QuestObjective, int>();
        return;

    var progressDictionary = questProgress[questSO];
    int newAmount = 0;

    if (objective.targetItem != null)
        newAmount = InventoryManager.Instance.GetItemQuantity(objective.targetItem);
    else if (objective.targetLocation != null && GameManager.instance.locationHistoryTracker.HasVisited(objective.targetLocation))
        newAmount = objective.requiredAmount;
    else if (objective.targetNPC != null && GameManager.instance.dialogueHistoryTracker.HasSpokenWith(objective.targetNPC))
        newAmount = objective.requiredAmount;
    else if (objective.targetEnemy != null)
    {
        if (!progressDictionary.ContainsKey(objective))
            progressDictionary[objective] = 0;
        return;
    }

        progressDictionary[objective] = newAmount;
}

public string GetProgressText(QuestSO questSO, QuestObjective objective)
{
    if (!questProgress.ContainsKey(questSO))
        return "In Progress";

    UpdateObjectiveProgress(questSO, objective);
    int currentAmount = GetCurrentAmount(questSO, objective);
    
    if (currentAmount >= objective.requiredAmount)
    return "Complete";

else if (objective.targetItem != null || objective.targetEnemy != null)
    return $"{currentAmount}/{objective.requiredAmount}";

else
    return "In Progress";
}

    public int GetCurrentAmount(QuestSO questSO, QuestObjective objective)
{
    if (!questProgress.ContainsKey(questSO))
        return 0;

    if (objective.targetItem != null)
        return InventoryManager.Instance.GetItemQuantity(objective.targetItem);

    if (questProgress.TryGetValue(questSO, out var objectiveDictionary)
        && objectiveDictionary.TryGetValue(objective, out int amount))
        return amount;

    return 0;
}
}
