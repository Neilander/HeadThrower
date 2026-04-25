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
    public static EventManager Instance;

    [Section("事件管理器配置")]
    [Tooltip("拖拽这里分配创建好的LevelDataSO文件,该文件管理所有传送位置数据")]
    public LevelDataSO levelData; // 引用上面创建的关卡数据文件

    [SerializeField]
    private DeliverintSO 当前关卡值SO;

    [Section("数据监听")]
    public int 当前关卡ID = 1; // 示例数据监听，实际数据监听可根据需要自行添加

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
