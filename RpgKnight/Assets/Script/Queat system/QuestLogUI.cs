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
    [SerializeField] private CanvasGroup declineCanvasGroup;
    [SerializeField] private CanvasGroup completeCanvasGroup;

    [Header("Submit Hint")]
    [SerializeField] private CanvasGroup submitHintCanvasGroup;
    [SerializeField] private TMP_Text submitHintText;
    [SerializeField] private float submitHintDuration = 2.5f;
    private QuestAcceptSource currentOpenSource;
    private Coroutine hideSubmitHintCoroutine;

    private void Awake()
    {
        HideSubmitHint();
    }

    private void OnEnable()
    {
        QuestEvents.OnQuestOfferRequested += ShowQuestoffer;
        QuestEvents.OnQuestTurnInRequested += ShowQuestTurnIn;
        QuestEvents.OnQuestCloseRequested += OnDeclineQuestClicked;
        QuestEvents.OnQuestProgressChanged += RefreshOpenQuestDisplay;
    }

    private void OnDisable()
    {
        QuestEvents.OnQuestOfferRequested -= ShowQuestoffer;
        QuestEvents.OnQuestTurnInRequested -= ShowQuestTurnIn;
        QuestEvents.OnQuestCloseRequested -= OnDeclineQuestClicked;
        QuestEvents.OnQuestProgressChanged -= RefreshOpenQuestDisplay;
    }

    public void ShowQuestoffer(QuestSO incomingQuestSO, QuestAcceptSource source)
    {
        currentOpenSource = source;
        HideSubmitHint();

        QuestSO displayQuest = incomingQuestSO;
        if (questManager.GetCompleteQuest(incomingQuestSO))
            displayQuest = noAvailableQuestSO;

        HandleQuestClicked(displayQuest);
        RefreshQuestList();
        SetCanvasState(questCanvas, true);
    }

    public void ShowQuestTurnIn(QuestSO incomingQuestSO, QuestAcceptSource source)
    {
        currentOpenSource = source;
        HideSubmitHint();
        questSO = incomingQuestSO;
        HandleQuestClicked(questSO);
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, false);
        bool canComplete = questManager.WasAcceptedFrom(incomingQuestSO, source)
            && questManager.IsQuestComplete(incomingQuestSO);
        SetCanvasState(completeCanvasGroup, canComplete);
        
        SetCanvasState(questCanvas, true);
    }



    public void OnAcceptQuestClicked()
    {
        QuestEvents.OnQuestAccepted?.Invoke(questSO, currentOpenSource);
        questManager.AcceptQuest(questSO, currentOpenSource);
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, true);
        SetCanvasState(completeCanvasGroup, false);
        RefreshQuestList();
    }

    public void OnDeclineQuestClicked()
    {
        HideSubmitHint();
        SetCanvasState(questCanvas, false);
    }

    public void OnCompleteQuestClicked()
    {
        if (!questManager.IsQuestComplete(questSO))
            return;

        if (!questManager.WasAcceptedFrom(questSO, currentOpenSource))
        {
            ShowSubmitHint(GetWrongSourceHintMessage());
            return;
        }

        QuestSO completedQuest = questSO;
        questManager.CompleteQuest(completedQuest);
        QuestEvents.OnQuestCompleted?.Invoke(completedQuest);
        RefreshQuestList();
        HandleQuestClicked(noAvailableQuestSO);
        SetCanvasState(completeCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, true);
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

    private void RefreshOpenQuestDisplay()
    {
        if (questCanvas.alpha <= 0f || questSO == null || questSO == noAvailableQuestSO)
            return;

        HandleQuestClicked(questSO);
    }

    public void HandleQuestClicked(QuestSO questSO)
    {
        this.questSO = questSO;
        questNameText.text = questSO.questName;
        questDescriptionText.text = questSO.questDescription;
    
        DisplayObjectives();
        DisplayRewards();
        UpdateActionButtons(questSO);
    }

    private void UpdateActionButtons(QuestSO selectedQuest)
    {
        if (selectedQuest == null || selectedQuest == noAvailableQuestSO)
        {
            SetCanvasState(acceptCanvasGroup, false);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, false);
            return;
        }

        if (questManager.GetCompleteQuest(selectedQuest))
        {
            SetCanvasState(acceptCanvasGroup, false);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, false);
            return;
        }

        if (questManager.IsQuestAccepted(selectedQuest))
        {
            SetCanvasState(acceptCanvasGroup, false);
            SetCanvasState(declineCanvasGroup, true);
            SetCanvasState(completeCanvasGroup, questManager.IsQuestComplete(selectedQuest));
            return;
        }

        SetCanvasState(acceptCanvasGroup, true);
        SetCanvasState(declineCanvasGroup, true);
        SetCanvasState(completeCanvasGroup, false);
    }

    private string GetWrongSourceHintMessage()
    {
        if (questManager.WasAcceptedFrom(questSO, QuestAcceptSource.NPC))
            return "NPC";

        if (questManager.WasAcceptedFrom(questSO, QuestAcceptSource.QuestBoard))
            return "QuestBoard";

        return "NO";
    }

    private void ShowSubmitHint(string message)
    {
        if (submitHintCanvasGroup == null || submitHintText == null)
            return;

        submitHintText.text = message;
        SetCanvasState(submitHintCanvasGroup, true);

        if (hideSubmitHintCoroutine != null)
            StopCoroutine(hideSubmitHintCoroutine);

        hideSubmitHintCoroutine = StartCoroutine(HideSubmitHintAfterDelay());
    }

    private IEnumerator HideSubmitHintAfterDelay()
    {
        yield return new WaitForSecondsRealtime(submitHintDuration);
        HideSubmitHint();
    }

    private void HideSubmitHint()
    {
        if (hideSubmitHintCoroutine != null)
        {
            StopCoroutine(hideSubmitHintCoroutine);
            hideSubmitHintCoroutine = null;
        }

        if (submitHintCanvasGroup != null)
            SetCanvasState(submitHintCanvasGroup, false);
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
