using System;
using System.Collections;
using System.Collections.Generic;
using Suntail;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    
    public Dictionary<string, Quest> allQuests = new Dictionary<string, Quest>();
    private List<Quest> activeQuests = new List<Quest>();
    public static event Action QuestUpdated;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        LoadQuestCSV(); 
    }

    public List<Quest> GetActiveQuests()
    {
        return activeQuests;
    }
    
    public Quest GetQuestByID(string questID)
    {
        return allQuests.TryGetValue(questID, out var quest) ? quest : null;
    }
    
    public Quest GetNextAvailableQuest(string npcName)
    {
        foreach (var quest in allQuests.Values)
        {
            if (quest.QuestGiverNPC == npcName && quest.State == QuestState.NotStarted)
                return quest;
        }
        return null;
    }
    
    void LoadQuestCSV()
    {
        TextAsset csv = Resources.Load<TextAsset>("Data/quest");
        if (csv == null)
        {
            Debug.LogError("quest.csv 파일이 없서");
            return;
        }

        string[] lines = csv.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            
            string[] fields = lines[i].Split(',');

            if (fields.Length < 7) continue;

            Quest quest = new Quest
            {
                ID = fields[0].Trim(),
                Title = fields[1].Trim(),
                Description = fields[2].Trim(),
                TargetType = fields[3].Trim(),
                TargetID = fields[4].Trim(),
                TargetCount = int.Parse(fields[5]),
                RewardXP = int.Parse(fields[6]),
                RewardItems = new List<string>(),
                QuestGiverNPC = fields[8].Trim()
            };

            if (!string.IsNullOrWhiteSpace(fields[7]))
            {
                string[] rewardItems = fields[7].Split(';');
                foreach (var item in rewardItems)
                {
                    quest.RewardItems.Add(item.Trim());
                }
            }

            if (!allQuests.ContainsKey(quest.ID))
                allQuests.Add(quest.ID, quest);
        }

        Debug.Log($"{allQuests.Count}개 퀘스트 존재 중!");
    }

    public void StartQuest(string questID)
    {
        if (!allQuests.ContainsKey(questID)) return;

        Quest quest = allQuests[questID];
        if (quest.State != QuestState.NotStarted) return;

        quest.State = QuestState.InProgress;
        activeQuests.Add(quest);
        QuestUpdated?.Invoke();
        Debug.Log($"퀘스트 시작: {quest.Title}");
    }

    public void ReportKill(string monsterId)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.TargetType == "Enemy" && quest.TargetID == monsterId && quest.State == QuestState.InProgress)
            {
                quest.CurrentCount++;
                QuestUpdated?.Invoke();
                Debug.Log($"퀘스트 진행 중: {quest.Title} ▶ {quest.CurrentCount}/{quest.TargetCount}");

                if (quest.CurrentCount >= quest.TargetCount)
                {
                    quest.State = QuestState.ReadyComplete;
                    Debug.Log($"퀘스트 완료 ! => npc한테 가서 대화를 다시 걸자");
                    QuestUpdated?.Invoke();
                }
                
            }
        }
    }

    public void CompleteQuestNotifyNpc(string npcName)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.State == QuestState.ReadyComplete && quest.QuestGiverNPC == npcName)
            {
                quest.State = QuestState.Completed;

                PlayerController player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.GainExp(quest.RewardXP);
                    Debug.Log($"보상 지급 ▶ EXP {quest.RewardXP}");

                    foreach (var item in quest.RewardItems)
                    {
                        Debug.Log($"아이템 지급: {item}");
                        // InventoryManager.Instance.AddItem(item);
                    }
                }

                // if (notificationUI != null)
                // {
                //     notificationUI.ShowNotification($"퀘스트 완료: {quest.Title} (보상 지급)");
                // }

                QuestUpdated?.Invoke();
            }
        }
    }
}
