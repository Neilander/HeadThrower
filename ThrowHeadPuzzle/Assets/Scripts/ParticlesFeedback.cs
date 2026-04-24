using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MMFeedbacks 风格粒子命中反馈（自动定位命中位置，无需手动指定目标）
/// 文件名：ParticlesFeedback.cs
/// </summary>
public class ParticlesFeedback : MonoBehaviour
{
    // ========== 常量提取（无魔法数字） ==========
    private const float 默认声明时长 = 0f;
    private const float 默认模拟速度 = 1f;
    private const float 默认粒子清理延时 = 5f;
    private const float 最小有效模拟速度 = 0.1f;
    private const string 地图图层名称 = "Map";

    // ========== MMF 风格播放模式 ==========
    public enum 粒子播放模式
    {
        正常播放,
        单次播放,
    }

    // ========== 面板配置 ==========
    [Header("粒子绑定")]
    public 粒子播放模式 播放模式 = 粒子播放模式.正常播放;
    public ParticleSystem 绑定粒子;
    public List<ParticleSystem> 随机粒子列表 = new List<ParticleSystem>();

    [Header("行为设置")]
    public bool 初始化时停止 = true;
    public bool 禁用时停止 = true;
    public bool 播放时激活对象 = true;

    [Header("时长与速度")]
    public float 声明时长 = 默认声明时长;
    public bool 强制模拟速度 = false;
    public float 模拟速度 = 默认模拟速度;

    // ========== 内部变量 ==========
    private ParticleSystem 当前使用粒子;
    private bool 已完成初始化;
    private GameObject 子弹本体; // 直接绑定子弹父物体
    private Rigidbody2D 子弹刚体;

    // ========== 初始化 ==========
    private void Awake()
    {
        // 🔥 核心修复：直接获取子弹父物体（碰撞主体）
        子弹本体 = transform.parent.gameObject;
        子弹刚体 = 子弹本体.GetComponent<Rigidbody2D>();

        bool 有绑定粒子 = 绑定粒子 != null;
        bool 需要初始化停止 = 初始化时停止 && 有绑定粒子;
    }

    private void OnEnable() { }

    private void OnDisable()
    {
        bool 粒子有效 = 当前使用粒子 != null;
        bool 需要禁用停止 = 禁用时停止 && 粒子有效;

        CancelInvoke(nameof(停止粒子发射));
    }

    // ========== 核心对外接口 ==========
    public void 播放反馈(Vector3 命中世界坐标)
    {
        bool 可正常播放 = 已完成初始化;
        if (!可正常播放)
        {
            Debug.LogWarning("【粒子反馈】未完成初始化，无法播放", this);
            return;
        }

        选择当前粒子();
        bool 粒子有效 = 当前使用粒子 != null;
        if (!粒子有效)
        {
            Debug.LogWarning("【粒子反馈】无有效粒子", this);
            return;
        }

        当前使用粒子.transform.position = 命中世界坐标;

        if (播放时激活对象)
        {
            当前使用粒子.gameObject.SetActive(true);
        }

        if (强制模拟速度)
        {
            var 主模块 = 当前使用粒子.main;
            主模块.simulationSpeed = 模拟速度 > 最小有效模拟速度 ? 模拟速度 : 默认模拟速度;
        }

        当前使用粒子.Play(true);
        Debug.Log("【粒子反馈】粒子特效已播放", this);

        float 停止延时 = 声明时长 > 0f ? 声明时长 : 默认粒子清理延时;
        Invoke(nameof(停止粒子发射), 停止延时);

        bool 是单次模式 = 播放模式 == 粒子播放模式.单次播放;
        if (是单次模式)
        {
            Invoke(nameof(仅停止发射), 0.1f);
        }
    }

    // ========== 内部逻辑 ==========
    private void 选择当前粒子()
    {
        bool 随机列表可用 = 随机粒子列表 != null && 随机粒子列表.Count > 0;
        if (随机列表可用)
        {
            int 随机序号 = Random.Range(0, 随机粒子列表.Count);
            当前使用粒子 = 随机粒子列表[随机序号];
        }
        else
        {
            当前使用粒子 = 绑定粒子;
        }
    }

    private void 停止粒子发射()
    {
        if (当前使用粒子 != null)
            当前使用粒子.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void 仅停止发射()
    {
        if (当前使用粒子 != null)
            当前使用粒子.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
