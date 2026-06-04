using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private QuestObjectiveSlot[] objectiveSlots;
    [SerializeField] private QuestRewardSlot[] rewardSlots;
    [SerializeField] private CanvasGroup questCanvas;
    private QuestSO questSO;
    [SerializeField] private QuestSO noAvailableQuestSO;
    [SerializeField] private QuestLogSlot[] questSlots;
    [SerializeField] private CanvasGroup acceptCanvasGroup;
    [SerializeField] private CanvasGroup declineCanvasGrtoup;
    [SerializeField] private CanvasGroup completeCanvasGroup;


    private void OnEnable()
    {
        QuestEvents.OnQuestOfferRequested += ShowQuestoffer;
        QuestEvents.OnQuestTurnInRequested += ShowQuestTurnIn;
        QuestEvents.OnQuestCloseRequested += OnDeclineQuestClicked;
    }

    private void OnDisable()
    {
        QuestEvents.OnQuestOfferRequested -= ShowQuestoffer;
        QuestEvents.OnQuestTurnInRequested -= ShowQuestTurnIn;
        QuestEvents.OnQuestCloseRequested -= OnDeclineQuestClicked;
    }

    public void ShowQuestoffer(QuestSO incomingQuestSO)
    {
        if(questManager.IsQuestAccepted(incomingQuestSO) || questManager.GetCompleteQuest(incomingQuestSO))
        {
        questSO = noAvailableQuestSO;
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGrtoup, true);
        SetCanvasState(completeCanvasGroup, false);
        }
        else{
        questSO = incomingQuestSO;
        SetCanvasState(acceptCanvasGroup, true);
        SetCanvasState(declineCanvasGrtoup, true);
        SetCanvasState(completeCanvasGroup, false);
        }
        HandleQuestClicked(questSO);
        SetCanvasState(questCanvas, true);
    
    }

    public void ShowQuestTurnIn(QuestSO incomingQuestSO)
    {
        Debug.Log("ShowQuestTurnIn");
        questSO = incomingQuestSO;
        HandleQuestClicked(questSO);
        SetCanvasState(acceptCanvasGroup, false);
        // SetCanvasState(declineCanvasGrtoup, true);
        SetCanvasState(declineCanvasGrtoup, false);
        SetCanvasState(completeCanvasGroup, true);
        
        SetCanvasState(questCanvas, true);
    }



    public void OnAcceptQuestClicked()
    {
        QuestEvents.OnQuestAccepted?.Invoke(questSO);
        questManager.AcceptQuest(questSO);
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGrtoup, true);
        SetCanvasState(completeCanvasGroup, false);
        RefreshQuestList();
    }

    public void OnDeclineQuestClicked()
    {
        SetCanvasState(questCanvas, false);
    }

    public void OnCompleteQuestClicked()
    {
        questManager.CompleteQuest(questSO);
        RefreshQuestList();
        HandleQuestClicked(noAvailableQuestSO);
        SetCanvasState(completeCanvasGroup, false);
    }

    private void SetCanvasState(CanvasGroup canvasGroup, bool isActive)
    {
        canvasGroup.alpha = isActive ? 1 : 0;
        canvasGroup.interactable = isActive;
        canvasGroup.blocksRaycasts = isActive;
    }

    private void RefreshQuestList()
    {
        List<QuestSO> activeQuests = questManager.GetActiveQuests();
        for (int i = 0; i < questSlots.Length; i++)
{
    // 逻辑1：如果当前索引小于活跃任务数量 → 显示对应任务
    if(i < activeQuests.Count)
    {
        // 给第i个任务槽位设置任务内容（示例：设置任务名称、图标等）
        questSlots[i].SetQuest(activeQuests[i]);
    }
    // 逻辑2：如果索引超出活跃任务数量 → 隐藏多余的任务槽位
    else
    {
        questSlots[i].ClearSlot();
    }
}
    }

    public void HandleQuestClicked(QuestSO questSO)
    {
        this.questSO = questSO;
        questNameText.text = questSO.questName;
        questDescriptionText.text = questSO.questDescription;
    
        DisplayObjectives();
        DisplayRewards();

        
    }

    private void DisplayObjectives()
{
    for (int i = 0; i < objectiveSlots.Length; i++)
    {
        if (i < questSO.objectives.Count)
        {
            var objective = questSO.objectives[i];
            questManager.UpdateObjectiveProgress(questSO, objective);

    int currentAmount = questManager.GetCurrentAmount(questSO, objective);
    string progress = questManager.GetProgressText(questSO, objective);
    bool isComplete = currentAmount >= objective.requiredAmount;

    objectiveSlots[i].gameObject.SetActive(true);
    objectiveSlots[i].RefreshObjectives(objective.description, progress, isComplete);
        }
        else
        {
            objectiveSlots[i].gameObject.SetActive(false);
        }
    }
}

    private void DisplayRewards()
    {
        for (int i = 0; i < rewardSlots.Length; i++)
        {
            if (i < questSO.rewards.Count)
            {
                var reward = questSO.rewards[i];
                rewardSlots[i].DisplayReward(reward.itemSO.itemIcon, reward.quantity);
                rewardSlots[i].gameObject.SetActive(true);
            }
            else
            {
                rewardSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
