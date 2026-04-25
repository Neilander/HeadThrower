using System.Collections.Generic;
using UnityEngine;

public sealed class MuzzleFlareController : MonoBehaviour
{
    // 核心配置（只保留最常用的）
    [Section("核心配置")]
    [SerializeField]
    private GameObject 弹壳模板;

    [SerializeField]
    private Transform 生成点;

    [SerializeField]
    private float 最短保留时间 = 2.15f;

    [Section("对象池配置")]
    [SerializeField]
    private int 预热数量 = 5;

    [SerializeField]
    private int 最大闲置数量 = 20;

    // 对象池容器
    private readonly Queue<GameObject> 闲置池 = new Queue<GameObject>();

    // 激活对象列表 - 管理当前SetActive(true)的对象
    private readonly List<GameObject> 激活对象列表 = new List<GameObject>();

    // 激活时间记录 - 记录每个激活对象的激活时间
    private readonly Dictionary<GameObject, float> 激活时间记录 =
        new Dictionary<GameObject, float>();

    private void Awake() => 初始化池();

    private void Update() => 自动回收();

    private void OnDestroy() => 清空所有实例();

    // 对外核心接口：播放一次弹壳特效
    public void Play()
    {
        // 从池中获取可用实例
        GameObject 弹壳实例 = 获取可用实例();

        // 设置位置和旋转到生成点
        bool 生成点有效 = 生成点 != null;
        if (生成点有效)
        {
            弹壳实例.transform.position = 生成点.position;
            弹壳实例.transform.rotation = 生成点.rotation;
        }

        // 激活对象
        弹壳实例.SetActive(true);

        // 添加到激活列表
        激活对象列表.Add(弹壳实例);

        // 记录激活时间
        激活时间记录[弹壳实例] = Time.time;

        // 播放粒子特效
        播放粒子特效(弹壳实例);

        // 调试日志
        Debug.Log($"<color=green>播放弹壳特效 - 激活对象数: {激活对象列表.Count}</color>");
    }

    // 对外接口：停止所有特效
    public void StopAll() { }

    // 初始化对象池
    private void 初始化池()
    {
        // 预热固定20个对象
        const int 预热数量常量 = 20;

        for (int i = 0; i < 预热数量常量; i++)
        {
            GameObject 预热实例 = 创建新实例();
            预热实例.SetActive(false);
            闲置池.Enqueue(预热实例);
        }

        Debug.Log($"<color=cyan>弹壳对象池初始化完成，预热数量: {预热数量常量}</color>");
    }

    // 从池里拿实例，没有就新建
    private GameObject 获取可用实例()
    {
        if (闲置池.Count > 0)
            return 闲置池.Dequeue();
        return 创建新实例();
    }

    // 创建新的弹壳实例
    private GameObject 创建新实例()
    {
        GameObject 新实例 = Instantiate(弹壳模板, transform);
        return 新实例;
    }

    // 播放粒子特效
    private void 播放粒子特效(GameObject 弹壳实例)
    {
        // 查找弹壳实例下的所有ParticleSystem组件
        ParticleSystem[] 粒子系统数组 = 弹壳实例.GetComponentsInChildren<ParticleSystem>();

        // 遍历并播放所有粒子系统
        foreach (ParticleSystem 粒子系统 in 粒子系统数组)
        {
            粒子系统.Clear();
            粒子系统.Play();
        }
    }

    // 自动回收播放完的实例
    private void 自动回收()
    {
        // 倒序遍历激活列表，避免在遍历时修改列表
        for (int i = 激活对象列表.Count - 1; i >= 0; i--)
        {
            GameObject 检查对象 = 激活对象列表[i];

            // 检查对象是否在时间记录中
            bool 对象有记录 = 激活时间记录.ContainsKey(检查对象);

            if (对象有记录)
            {
                // 计算已激活时间
                float 已激活时间 = Time.time - 激活时间记录[检查对象];

                // 检查是否到达最短保留时间
                bool 到达回收时间 = 已激活时间 >= 最短保留时间;

                if (到达回收时间)
                {
                    // 回收该对象
                    回收实例(检查对象);

                    // 从激活列表中移除
                    激活对象列表.RemoveAt(i);

                    // 从时间记录中移除
                    激活时间记录.Remove(检查对象);
                }
            }
        }
    }

    // 回收实例到池里
    private void 回收实例(GameObject 实例)
    {
        // 停止所有粒子特效
        停止粒子特效(实例);

        // 设置为未激活状态
        实例.SetActive(false);

        // 重置位置到父对象
        实例.transform.position = transform.position;
        实例.transform.rotation = transform.rotation;

        // 检查闲置池是否达到最大数量
        bool 闲置池未满 = 闲置池.Count < 最大闲置数量;

        if (闲置池未满)
        {
            // 放回闲置池
            闲置池.Enqueue(实例);
        }
        else
        {
            // 销毁超出数量的实例
            Destroy(实例);
        }
    }

    // 停止粒子特效
    private void 停止粒子特效(GameObject 弹壳实例)
    {
        // 查找弹壳实例下的所有ParticleSystem组件
        ParticleSystem[] 粒子系统数组 = 弹壳实例.GetComponentsInChildren<ParticleSystem>();

        // 遍历并停止所有粒子系统
        foreach (ParticleSystem 粒子系统 in 粒子系统数组)
        {
            粒子系统.Stop();
        }
    }

    // 销毁所有实例
    private void 清空所有实例() { }
}
