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

    [Section("事件委托")]
    [SerializeField]
    private UnityAction<int> 事件;

    /// <summary>
    /// 订阅事件 - 添加回调方法
    /// </summary>
    /// <param name="回调">接收int值的回调方法</param>
    public void 订阅事件(UnityAction<int> 回调)
    {
        bool 回调有效 = 回调 != null;
        if (回调有效)
        {
            事件 += 回调;
        }
    }

    /// <summary>
    /// 取消订阅事件 - 移除回调方法
    /// </summary>
    /// <param name="回调">要移除的回调方法</param>
    public void 取消订阅事件(UnityAction<int> 回调)
    {
        bool 回调有效 = 回调 != null;
        if (回调有效)
        {
            事件 -= 回调;
        }
    }

    /// <summary>
    /// 触发事件 - 通知所有订阅者
    /// </summary>
    /// <param name="值">要传递的int值</param>
    public void 触发事件(int 值)
    {
        bool 有订阅者 = 事件 != null;

        if (有订阅者)
        {
            事件?.Invoke(值);
        }
    }

    /// <summary>
    /// 清空所有订阅者
    /// </summary>
    public void 清空所有订阅()
    {
        事件 = null;
    }
}
