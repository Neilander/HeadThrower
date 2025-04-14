using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "事件/传递bool类型变量的事件SO")]

public class DeliverBoolSO : ScriptableObject
{
    public UnityAction<bool> _boolvalue;

    public void RaiseEvent(bool _bool)
    {
        _boolvalue?.Invoke(_bool);
    }

}

