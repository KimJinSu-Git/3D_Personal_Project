using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState { NotStarted, InProgress, ReadyComplete ,Completed }

public class Quest
{
    public string ID;
    public string Title;
    public string Description;
    public string TargetType; 
    public string TargetID;  
    public int TargetCount;
    public int CurrentCount = 0;
    public int RewardXP;
    public List<string> RewardItems;
    public string QuestGiverNPC;
    public QuestState State = QuestState.NotStarted;
}