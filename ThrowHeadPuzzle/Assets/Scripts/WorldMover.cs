using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldMover : MonoBehaviour
{
    #region 常量定义
    private const float 初始时间 = 0f;
    private const float 完成进度 = 1.0f;
    #endregion

    [Section("移动模块配置", "#4CAF50")]
    public float 移动时长 = 1.0f;
    public AnimationCurve 移动曲线 = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Section("事件触发控制", "#2196F3")]
    public List<UnityEvent> 到达目的地触发事件;
    public int 当前事件索引 = 0;

    private bool _是否正在移动 = false;
    private Vector3 _起始位置;
    private Vector3 _目标位置;
    private float _已流逝时间 = 0;

    private void FixedUpdate()
    {
        bool 处于移动状态 = _是否正在移动;
        if (处于移动状态)
        {
            执行平滑移动();
        }
    }

    /// <summary>
    /// 开始平滑移动到指定位置
    /// </summary>
    public void 移动至(Vector3 目的地坐标, int 关联事件索引 = 0)
    {
        _起始位置 = transform.position;
        _目标位置 = 目的地坐标;
        _已流逝时间 = 初始时间;
        _是否正在移动 = true;
        当前事件索引 = 关联事件索引;
    }

    private void 执行平滑移动()
    {
        bool 移动未结束 = _是否正在移动;
        if (!移动未结束)
            return;

        _已流逝时间 += Time.deltaTime;
        float 移动进度 = Mathf.Clamp01(_已流逝时间 / 移动时长);
        float 曲线插值 = 移动曲线.Evaluate(移动进度);

        transform.position = Vector3.Lerp(_起始位置, _目标位置, 曲线插值);

        bool 已到达终点 = 移动进度 >= 完成进度;
        if (已到达终点)
        {
            完成移动过程();
        }
    }

    private void 完成移动过程()
    {
        transform.position = _目标位置;
        _是否正在移动 = false;

        bool 事件列表有效 = 到达目的地触发事件 != null;
        bool 索引在范围内 = 当前事件索引 >= 0 && 当前事件索引 < 到达目的地触发事件.Count;

        if (事件列表有效 && 索引在范围内)
        {
            到达目的地触发事件[当前事件索引]?.Invoke();
        }
    }
}