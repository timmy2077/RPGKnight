using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestAcceptSource
{
    QuestBoard,
    NPC
}

public static class QuestEvents
{
   public static Action<QuestSO, QuestAcceptSource> OnQuestOfferRequested;
   public static Action<QuestSO, QuestAcceptSource> OnQuestTurnInRequested;
   public static Action<QuestSO, QuestAcceptSource> OnQuestAccepted;
   public static Action OnQuestCloseRequested;
   public static Func<QuestSO, bool> IsQuestComplete;
   public static Action<QuestSO> OnQuestCompleted;
   public static Action<EnemySO> OnEnemyKilled;
   public static Action OnQuestProgressChanged;
}
