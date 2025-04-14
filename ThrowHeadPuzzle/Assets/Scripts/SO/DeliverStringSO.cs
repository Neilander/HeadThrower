using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "事件/传递string类型变量的事件SO")]

public class DeliverStringSO : ScriptableObject
{
    public ObjectType objectType;
    [SerializeField]
    public UnityAction<string> ItemData;
    [SerializeField]
    public UnityAction<string> NPCData;
    public void DeliverItem(string ItemName)
    {
        ItemData?.Invoke(ItemName);
    }
    public void DeliverNPC(string NPCName)
    {
        ItemData?.Invoke(NPCName);
    }
}

public enum ObjectType
{
    item, NPC
}
