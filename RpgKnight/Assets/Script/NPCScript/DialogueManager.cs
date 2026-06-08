using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    private void Awake() 
    {
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }   

    [Header("UI References")]
    public Image portrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;
    public bool isDialogueActive = false;
    private DialogueSO currentDialogue;
    private int dialogueIndex;
    public GameObject dialogueCanvas;
    public Button[] choiceButtons;
    private float lastDialogueEndTime;
    private float dialogueCooldown = 0.1f;

    private void Start()
    {

        // 移除直接调用 ShowDialo gue()，因为此时 currentDialogue 还未赋值
    }

    public bool CanStartDialogue()
    {
        return Time.unscaledTime - lastDialogueEndTime >= dialogueCooldown;
    }

    public void StartDialogue(DialogueSO dialogueSO)
    {
        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        isDialogueActive = true;
        ShowDialogue();
    }

    public void AdvanceDialogue()
    {
        if (dialogueIndex < currentDialogue.lines.Length)
        {
            
            ShowDialogue();
        }
        else
        {
            ShowChoices();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueIndex = 0;
        ClearChoices();
        dialogueCanvas.SetActive(false);
        Time.timeScale = 1;
        lastDialogueEndTime = Time.unscaledTime;
    }


    private void ShowDialogue()
    {
        DialogueLine line = currentDialogue.lines[dialogueIndex];
        GameManager.instance.dialogueHistoryTracker.RecordNPC(line.speaker);
        portrait.sprite = line.speaker.portrait;
        actorName.text = line.speaker.actorName;
        dialogueText.text = line.text;
        dialogueCanvas.SetActive(true);
        Time.timeScale = 0;
        dialogueIndex++;
    }

    private void ShowChoices()
{
    ClearChoices();
    if (currentDialogue.options.Length > 0)
    {
        for (int i = 0; i < currentDialogue.options.Length; i++)
        {
            var option = currentDialogue.options[i];
            
            choiceButtons[i].GetComponentInChildren<TMP_Text>().text = option.optionText;
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].onClick.AddListener(() =>
            {
                ChooseOption(option.nextDialogue);
            });
            
        }
        EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
    }
    else
    {
        if (currentDialogue.turnInQuestOnEnd != null &&
    GameManager.instance.questManager.IsQuestComplete(currentDialogue.turnInQuestOnEnd) &&
    GameManager.instance.questManager.WasAcceptedFrom(currentDialogue.turnInQuestOnEnd, QuestAcceptSource.NPC))
    {
    QuestEvents.OnQuestTurnInRequested?.Invoke(currentDialogue.turnInQuestOnEnd, QuestAcceptSource.NPC);
    EndDialogue();
    }

        else if (currentDialogue.offerQuestOnEnd != null)
        {
            EndDialogue();
            QuestEvents.OnQuestOfferRequested?.Invoke(currentDialogue.offerQuestOnEnd, QuestAcceptSource.NPC);
        }
        else
        {
        choiceButtons [0].GetComponentInChildren<TMP_Text>().text = "End";
        choiceButtons [0].onClick.AddListener(() =>{EndDialogue();});
        choiceButtons [0].gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
        }
    }    
    
}

    private void ChooseOption(DialogueSO dialogueSO)
    {
        if (dialogueSO == null)
        {
            EndDialogue();
        }
        else
        {
            ClearChoices();
            StartDialogue(dialogueSO);
        }
    }

    private void ClearChoices()
    {
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }
    
}


