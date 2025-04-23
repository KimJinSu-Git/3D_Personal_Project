using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text questText;
    public GameObject questPanel;

    private void Start()
    {
        StartCoroutine(WaitForQuestManager());
    }
    
    private IEnumerator WaitForQuestManager()
    {
        yield return new WaitUntil(() => QuestManager.Instance != null);
        
        QuestManager.QuestUpdated += UpdateQuest;
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.QuestUpdated -= UpdateQuest;
        }
    }

    void UpdateQuest()
    {
        List<Quest> activeQuests = QuestManager.Instance.GetActiveQuests();
        List<Quest> displayQuests = activeQuests.FindAll(q => q.State != QuestState.Completed);
        
        if (activeQuests.Count == 0)
        {
            questPanel.SetActive(false);
            return;
        }

        questPanel.SetActive(true);
        
        string display = "";
        foreach (var quest in displayQuests)
        {
            string line = $"▶ {quest.Title} {quest.CurrentCount} / {quest.TargetCount}";
            if (quest.State == QuestState.ReadyComplete)
            {
                line += " (보고 필요)";
            }
            display += line + "\n";
        }

        questText.text = display;
    }
}
