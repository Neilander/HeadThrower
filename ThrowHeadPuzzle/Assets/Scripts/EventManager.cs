using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局事件管理器
/// 负责事件的订阅、取消订阅、触发
/// 挂载在任意全局物体上(推荐GameManager)
/// </summary>
public class EventManager : MonoBehaviour
{
    // ===================== 单例模式 =====================
    public static EventManager Instance;

    // ===================== 核心字典：存储所有事件监听 =====================
    private Dictionary<string, Action> 事件监听字典 = new Dictionary<string, Action>();

    // ===================== 初始化 =====================
    private void Awake()
    {
        // 单例去重
        bool 实例已存在 = (Instance != null);
        if (实例已存在)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // 切换场景不销毁
        DontDestroyOnLoad(gameObject);
    }

    // ===================== 核心功能：添加监听 =====================
    public void 添加监听(string 事件名称, Action 回调方法)
    {
        bool 事件已存在 = 事件监听字典.ContainsKey(事件名称);
        if (事件已存在)
        {
            事件监听字典[事件名称] += 回调方法;
        }
        else
        {
            事件监听字典.Add(事件名称, 回调方法);
        }
    }

    // ===================== 核心功能：移除监听 =====================
    public void 移除监听(string 事件名称, Action 回调方法)
    {
        bool 事件已存在 = 事件监听字典.ContainsKey(事件名称);
        if (事件已存在)
        {
            事件监听字典[事件名称] -= 回调方法;
        }
    }

    // ===================== 核心功能：触发事件 =====================
    public void 触发事件(string 事件名称)
    {
        bool 事件存在且有监听 =
            事件监听字典.ContainsKey(事件名称) && 事件监听字典[事件名称] != null;
        if (事件存在且有监听)
        {
            事件监听字典[事件名称]?.Invoke();
        }
    }
}
