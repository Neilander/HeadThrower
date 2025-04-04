using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "事件/传递bool类型变量的事件SO")]

public class DeliverBoolSO : ScriptableObject
{
    public UnityEvent<bool> eventQ;
    public UnityEvent<bool> eventE;

    public void Change_Q_State(bool _bool)
    {
        eventQ?.Invoke(_bool);
    }

    public void Change_E_State(bool _bool)
    {
        eventE?.Invoke(_bool);
    }
}

