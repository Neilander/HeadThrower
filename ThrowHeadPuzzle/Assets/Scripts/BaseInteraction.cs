using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInteraction : MonoBehaviour
{
    private bool isInteractable = true;
    public List<BaseInteraction> linkedInteractions = new List<BaseInteraction>();

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
    }

    public bool IsInteractable()
    {
        return isInteractable;
    }

    public virtual bool OnInteract(InteractionSignal signal)
    {
        Debug.Log("OnInteract");
        return true;
    }
}

public enum InteractionType
{
    KeyPress,   // 主动按键交互（例如玩家按 E）
    Destroy,    // 被破坏（例如被攻击、炸掉）
    PressDown   // 被踩压（例如跳上去压下按钮）
}

public class InteractionSignal
{
    public GameObject source;         // 谁发起了交互
    public InteractionType type;      // 交互类型
    public object data;               // 可选附加数据（例如力的大小、方向、键位等）

    public InteractionSignal(GameObject source, InteractionType type, object data = null)
    {
        this.source = source;
        this.type = type;
        this.data = data;
    }
}