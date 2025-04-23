using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public Dictionary<string, DialogueLine> DialogueDiCtionary = new Dictionary<string, DialogueLine>();

    private void Awake()
    {
        LoadCSV();
    }

    void LoadCSV()
    {
        TextAsset csv = Resources.Load<TextAsset>("dialogue");
        if (csv == null)
        {
            Debug.LogError("CSV 파일이 없어");
            return;
        }

        string[] lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            DialogueLine line = new DialogueLine
            {
                id = fields.Length > 0 ? fields[0].Trim() : "",
                npcName = fields.Length > 1 ? fields[1].Trim() : "",
                text = fields.Length > 2 ? fields[2].Trim().Trim('"') : "",
                choice1 = fields.Length > 3 ? fields[3].Trim() : "",
                next1 = fields.Length > 4 ? fields[4].Trim() : "",
                choice1Action = fields.Length > 5 ? fields[5].Trim() : "",
                choice2 = fields.Length > 6 ? fields[6].Trim() : "",
                next2 = fields.Length > 7 ? fields[7].Trim() : "",
                choice2Action = fields.Length > 8 ? fields[8].Trim() : ""
            };

            if (!string.IsNullOrEmpty(line.id) && !DialogueDiCtionary.ContainsKey(line.id))
            {
                DialogueDiCtionary[line.id] = line;
            }
        }
    }

    public DialogueLine GetDialogue(string id)
    {
        return DialogueDiCtionary.TryGetValue(id, out var line) ? line : null;
    }
}
