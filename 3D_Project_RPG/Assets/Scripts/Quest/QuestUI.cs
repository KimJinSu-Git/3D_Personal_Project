using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;
    
    [Header("UI")]
    public TMP_Text questText;
    public GameObject questPanel;
    [SerializeField] private Image image;
    
    private void Awake()
    {
        Instance = this;
    }

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

    public void UpdateQuest()
    {
        List<Quest> activeQuests = QuestManager.Instance.GetActiveQuests();
        List<Quest> displayQuests = activeQuests.FindAll(q => q.State != QuestState.Completed);
        List<Quest> progressQuests = activeQuests.FindAll(q => q.State is QuestState.InProgress or QuestState.ReadyComplete);
        
        if (progressQuests.Count == 0)
        {
            questPanel.SetActive(false);
            return;
        }

        questPanel.SetActive(true);
        
        string display = "";
        foreach (var quest in displayQuests)
        {
            string line = null;
            if (quest.State == QuestState.InProgress)
            {
                image.sprite = Resources.Load<Sprite>("Icons/checkbox_unchecked");
                line = $"       {quest.Title} {quest.CurrentCount} / {quest.TargetCount}";
            }
            
            if (quest.State == QuestState.ReadyComplete)
            {
                image.sprite = Resources.Load<Sprite>("Icons/checkbox_checked");
                line = $"       {quest.Title} {quest.CurrentCount} / {quest.TargetCount}";
            }
            display += line + "\n";
        }

        questText.text = display;
    }
}
