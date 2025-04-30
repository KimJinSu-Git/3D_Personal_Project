using System;
using System.Collections;
using System.Collections.Generic;
using Suntail;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class DialogueActionHandler
{
    public static void Execute(string action, NPC npc = null)
    {
        if (string.IsNullOrEmpty(action)) return;

        if (action.StartsWith("StartQuest:"))
        {
            string questID = action.Split(':')[1];
            QuestManager.Instance.StartQuest(questID);
        }

        if (action.StartsWith("CompleteQuest:"))
        {
            string npcName = action.Split(':')[1];
            QuestManager.Instance.CompleteQuestNpc(npcName);
        }
        
        if (action.StartsWith("OpenShop:"))
        {
            string npcName = action.Split(':')[1];
            Debug.Log(npcName);
            
            if (npc != null && npc.npcName == npcName)
            {
                Debug.Log("OpenShopPanel 실행전");
                ShopManager.Instance.OpenShopPanel();
                Debug.Log("OpenShopPanel 실행후");
            }
            else
            {
                Debug.LogWarning($"'{npcName}'을 가진 npc가 없어용용가리치킨");
            }
        }
    }
}

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;
    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Button choice1Button;
    public Button choice2Button;
    [SerializeField] private Canvas playerHealthCanvas;

    public NPC currentNPC;
    
    private DialogueLoader loader;
    private DialogueLine currentLine;
    private PlayerController player;

    private bool isTyping = false;
    private bool isLastLine = false;
    
    private Coroutine typingCoroutine;
    

    public bool IsTyping => isTyping;
    public bool IsLastLine => isLastLine;
    public DialogueLine CurrentLine => currentLine;

    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        loader = FindObjectOfType<DialogueLoader>();
        player = FindObjectOfType<PlayerController>();
        playerHealthCanvas.enabled = true;
        dialoguePanel.SetActive(false);
    }
    
    public void StartDialogue(string startId)
    {
        playerHealthCanvas.enabled = false;
        dialoguePanel.SetActive(true);

        currentNPC = GameObject.Find(startId)?.GetComponent<NPC>();

        if (currentNPC == null)
        {
            Debug.LogWarning("Npc가 없어요");
        }
        
        currentNPC.FacePlayer(true);
        
        string dialogueId = currentNPC.GetDialogueId();
        
        DialogueLine line = loader.GetDialogue(dialogueId);
        ShowLine(line);
        player.ChangeState(PlayerState.Talking);
        
        QuestManager.Instance.CompleteQuestNpc(currentNPC.npcName); 
    }

    public void ShopEndDialogue()
    {
        playerHealthCanvas.enabled = false;
        dialoguePanel.SetActive(true);
        currentNPC.FacePlayer(true);
        DialogueLine line = loader.GetDialogue("203");
        ShowLine(line);
        player.ChangeState(PlayerState.Talking);

        StartCoroutine(StoreNPCRotation());
    }
    
    private IEnumerator StoreNPCRotation()
    {
        while (IsTyping)
            yield return null;

        if (IsLastLine)
        {
            yield return new WaitUntil(() => dialoguePanel.activeSelf == false);
        }
        // yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));
        
        if (currentNPC != null)
            currentNPC.FacePlayer(false);
    }

    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        dialogueText.text = currentLine.text;
        isTyping = false;

        ActiveChoices();
    }

    private void ShowLine(DialogueLine line)
    {
        if (line == null) return;

        currentLine = line;

        nameText.text = line.npcName;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        typingCoroutine = StartCoroutine(TypeSentence(line.text));

        bool hasChoice1 = !string.IsNullOrEmpty(line.choice1);
        bool hasChoice2 = !string.IsNullOrEmpty(line.choice2);

        isLastLine = !hasChoice1 && !hasChoice2;

        choice1Button.gameObject.SetActive(false);
        choice2Button.gameObject.SetActive(false);
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);

            if (!isTyping) yield break;
        }

        isTyping = false;
        ActiveChoices();
    }

    private void ActiveChoices()
    {
        if (!string.IsNullOrEmpty(currentLine.choice1))
        {
            choice1Button.gameObject.SetActive(true);
            choice1Button.GetComponentInChildren<TMP_Text>().text = currentLine.choice1;
            choice1Button.onClick.RemoveAllListeners();
            choice1Button.onClick.AddListener(() =>
            {
                DialogueActionHandler.Execute(currentLine.choice1Action, currentNPC);
                ShowLine(loader.GetDialogue(currentLine.next1));
            });
        }

        if (!string.IsNullOrEmpty(currentLine.choice2))
        {
            choice2Button.gameObject.SetActive(true);
            choice2Button.GetComponentInChildren<TMP_Text>().text = currentLine.choice2;
            choice2Button.onClick.RemoveAllListeners();
            choice2Button.onClick.AddListener(() =>
            {
                DialogueActionHandler.Execute(currentLine.choice2Action, currentNPC);
                ShowLine(loader.GetDialogue(currentLine.next2));
            });
        }
    }

    public void EndDialogue()
    {
        playerHealthCanvas.enabled = true;
        dialoguePanel.SetActive(false);
        if (currentNPC != null && currentNPC.npcType != NPCType.Shop)
            currentNPC.FacePlayer(false);
        
        player.ChangeState(PlayerState.Idle);
    }
}
