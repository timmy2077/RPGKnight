using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    [SerializeField] private QuestSO questToOffer;
    [SerializeField] private QuestSO questToTurnIn;
    private bool playerInRange;
    private bool isQuestUIOpen = false;

    private void Update()
{
    // if (playerInRange && Input.GetButtonDown("ToggleQuest"))
    if (Input.GetButtonDown("ToggleQuest"))
    {
        if (isQuestUIOpen)
        {
            QuestEvents.OnQuestCloseRequested?.Invoke();
            isQuestUIOpen = false;
        }
        else
        {
            bool canTurnIn = questToTurnIn != null && QuestEvents.IsQuestComplete?.Invoke(questToTurnIn) == true;
            if(canTurnIn)
            {
                QuestEvents.OnQuestTurnInRequested?.Invoke(questToTurnIn);
                isQuestUIOpen = true;
            }
            else
            {
                QuestEvents.OnQuestOfferRequested?.Invoke(questToOffer);
                isQuestUIOpen = true;
            }
        }
    }
}

// private void OnTriggerEnter2D(Collider2D collision)
// {
//     if (collision.gameObject.CompareTag("Player"))
//     {
//         playerInRange = true;
//     }
// }

// private void OnTriggerExit2D(Collider2D collision)
// {
//     if (collision.gameObject.CompareTag("Player"))
//     {
//         playerInRange = false;
//     } 
// }
}
