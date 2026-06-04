using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class QuestEvents
{
   public static Action<QuestSO> OnQuestOfferRequested;
   public static Action<QuestSO> OnQuestTurnInRequested;
   public static Action<QuestSO> OnQuestAccepted;
   public static Action OnQuestCloseRequested;
   public static Func<QuestSO, bool> IsQuestComplete;
}
