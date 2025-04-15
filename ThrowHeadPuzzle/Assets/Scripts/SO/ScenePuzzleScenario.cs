using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScenePuzzleScenario", menuName = "AI/TurtleSoupScenario")]
public class ScenePuzzleScenario : ScriptableObject
{
    [Header("Judger 设置")]
    [TextArea(4, 6)] public string judgerPrompt;
    [HideInInspector] public string judgerKnowledgeJSON;
    public List<KnowledgeEntry> judgerKnowledgeEntries = new();

    [Header("NPC 设置")]
    [TextArea(4, 6)] public string npcPrompt; // e.g. "You are {0}, a nervous NPC"
    [HideInInspector] public string npcKnowledgeJSON;
    public List<KnowledgeEntry> npcKnowledgeEntries = new();
    public string npcName;

    [Header("玩家信息")]
    public string playerName;

    [ContextMenu("更新 JSON")]
    public void UpdateKnowledgeJSON()
    {
        judgerKnowledgeJSON = GenerateJudgerPromptJSON();
        npcKnowledgeJSON = GenerateNpcPromptJSON();
    }

    private Dictionary<string, string> ToDictionary(List<KnowledgeEntry> entries)
    {
        var dict = new Dictionary<string, string>();
        foreach (var entry in entries)
        {
            if (!string.IsNullOrWhiteSpace(entry.question))
                dict[entry.question] = entry.answer;
        }
        return dict;
    }

    public string GenerateJudgerPromptJSON()
    {
        var data = new
        {
            prompt = judgerPrompt,
            knowledge = ToDictionary(judgerKnowledgeEntries)
        };
        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }

    public string GenerateNpcPromptJSON()
    {
        string formattedPrompt = string.Format(npcPrompt, npcName);
        var data = new
        {
            prompt = formattedPrompt,
            knowledge = ToDictionary(npcKnowledgeEntries)
        };
        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }

}

[System.Serializable]
public class KnowledgeEntry
{
    public string question;
    public string answer;
}
