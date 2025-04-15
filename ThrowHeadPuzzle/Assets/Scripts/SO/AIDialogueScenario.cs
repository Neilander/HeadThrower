using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueScenario", menuName = "AI/Dialogue Scenario")]
public class AIDialogueScenario : ScriptableObject
{
    [Header("Basic Info")]
    public string aiName;
    public string playerName;

    [TextArea(4, 6)]
    [Header("Initial Requirement (每次都会发送的)")]
    public string initialRequest;

    [TextArea(4, 6)]
    [Header("Initial Prompt (一开始发送的起始设定)")]
    public string initialPrompt;

    [TextArea(4, 6)]
    [Header("故事的真相")]
    public string storyTruth;


    [TextArea(4, 6)]
    [Header("故事的表象")]
    public string storyForPlayer;

    public string returnFullRequest()
    {
        return string.Format(initialRequest, storyTruth, storyForPlayer);
    }

    public string returnFullPrompt()
    {
        return string.Format(initialPrompt, storyTruth, storyForPlayer);
    }
}
