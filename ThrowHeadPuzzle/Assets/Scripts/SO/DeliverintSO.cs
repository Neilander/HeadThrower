using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "事件/传递int类型变量的事件SO")]
public class DeliverintSO : ScriptableObject
{
    public UnityAction<int> _intvalue;

    public void RaiseEvent(int _int)
    {
        _intvalue?.Invoke(_int);
    }
}
