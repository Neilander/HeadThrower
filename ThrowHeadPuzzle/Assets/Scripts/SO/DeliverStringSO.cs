using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "事件/传递string类型变量的事件SO")]


public class DeliverStringSO : ScriptableObject
{
    public UnityEvent<string> ItemData;
    public void RaiseEvent(string Item)
    {
        ItemData?.Invoke(Item);
    }
}
