using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public DialogueManager dialogueManager;
    public DialogueHistoryTracker dialogueHistoryTracker;
    public LocationHistoryTracker locationHistoryTracker;
    public QuestManager questManager;

    [Header("Persiitent Objent")]
    public GameObject[] persistentObjects;

    private void Awake()
    {
        if (instance != null)
        {
            ClearUpAndDestroy();
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistentObjects();
        }
    }

    private void MarkPersistentObjects()
    {
        foreach (var obj in persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    private void ClearUpAndDestroy()
    {
        foreach (var obj in persistentObjects)
        {
           if (obj != null)
        {
            Destroy(obj);
        }
        }
        Destroy(gameObject);
    }
}
