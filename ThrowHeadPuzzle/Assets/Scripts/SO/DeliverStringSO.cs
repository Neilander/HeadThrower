using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "事件/传递string类型变量的事件SO")]

public class DeliverStringSO : ScriptableObject
{
    public ObjectType objectType;
    public UnityEvent<string> ItemData;
    public void RaiseEvent(string ItemName)
    {
        ItemData?.Invoke(ItemName);
    }
}

public enum ObjectType
{
    item, NPC
}
