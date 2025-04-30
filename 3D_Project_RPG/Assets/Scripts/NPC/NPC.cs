using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType
{
    Normal,
    Quest,
    Shop
}

public class NPC : MonoBehaviour
{
    public NPCType npcType;
    
    public string npcName;
    public string startDialogueId;
    public List<string> questIDs = new List<string>(); 

    private Animator animator;
    private Quaternion originalRotation;
    private bool isInteracting;
    
    private void Start()
    {
        isInteracting = false;
        animator = GetComponentInChildren<Animator>();
        originalRotation = transform.rotation;
    }

    private void OnTriggerStay(Collider other)  
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.G) && DialogueUI.Instance.dialoguePanel.activeSelf == false)
        {
            Vector3 lookPlayer = other.transform.position - transform.position;
            lookPlayer.y = 0f;
            if (lookPlayer != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookPlayer);

            switch (npcType)
            {
                case NPCType.Normal:
                    FindObjectOfType<DialogueUI>().StartDialogue(startDialogueId);
                    break;

                case NPCType.Quest:
                    FindObjectOfType<DialogueUI>().StartDialogue(startDialogueId);
                    break;

                case NPCType.Shop:
                    FindObjectOfType<DialogueUI>().StartDialogue(startDialogueId);
                    ShopManager.Instance.gameObject.SetActive(true); 
                    break;
            }
        }
    }
    
    public void FacePlayer(bool lookAtPlayer)
    {
        if (lookAtPlayer == false)
        {
            Debug.Log("Facing player");
            StartCoroutine(RotateBack());
        }
    }

    public string GetDialogueId()
    {
        if (questIDs != null && questIDs.Count > 0)
        {
            foreach (var questID in questIDs)
            {
                Quest quest = QuestManager.Instance?.GetQuestID(questID);
                if (quest == null) continue;

                if (quest.State == QuestState.ReadyComplete)
                    return $"Q_ReadyComplete_{questID}";
            }

            foreach (var questID in questIDs)
            {
                Quest quest = QuestManager.Instance?.GetQuestID(questID);
                if (quest == null) continue;

                if (quest.State == QuestState.NotStarted)
                    return $"Q_Start_{questID}";
            }

            foreach (var questID in questIDs)
            {
                Quest quest = QuestManager.Instance?.GetQuestID(questID);
                if (quest == null) continue;

                if (quest.State == QuestState.InProgress)
                    return $"Q_Progress_{questID}";
            }
        }
        
        return startDialogueId;
    }

    private IEnumerator RotateBack()
    {
        float t = 0f;
        Quaternion startRot = transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f; 
            transform.rotation = Quaternion.Slerp(startRot, originalRotation, t);
            yield return null;
        }

        transform.rotation = originalRotation;
    }
}
