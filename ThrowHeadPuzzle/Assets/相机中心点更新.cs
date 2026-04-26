using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 相机中心点更新 : MonoBehaviour
{
    // ======== 常量定义 ========

    private const float 默认相机Z深度 = -10f;
    private const float 默认平滑跟随速度 = 10f;
    private const int 中点除数 = 2;
    private const float 零值 = 0f;

    // ======== 分组名称常量 ========

    private const string 目标追踪分组 = "目标追踪";
    private const string 相机深度分组 = "相机深度";
    private const string 平滑跟随分组 = "平滑跟随";

    // ======== 序列化字段 ========

    [Section(目标追踪分组)]
    [SerializeField]
    private Transform 人物身体;

    [Section(目标追踪分组)]
    [SerializeField]
    private Transform 人物头部;

    [Section(相机深度分组)]
    [SerializeField]
    private float 相机Z深度 = 默认相机Z深度;

    [Section(平滑跟随分组)]
    [SerializeField]
    private bool 启用平滑跟随 = true;

    [Section(平滑跟随分组)]
    [SerializeField]
    private float 平滑跟随速度 = 默认平滑跟随速度;

    // ======== Unity 生命周期 ========

    private void LateUpdate()
    {
        更新相机位置();
    }

    // ======== 内部方法 ========

    /// <summary>
    /// 每帧更新相机位置，根据配置决定是否启用平滑过渡
    /// </summary>
    private void 更新相机位置()
    {
        bool 任一目标缺失 = 人物身体 == null || 人物头部 == null;
        if (任一目标缺失)
        {
            return;
        }

        Vector3 目标中心位置 = 计算两目标中心点();

        bool 需要平滑过渡 = 启用平滑跟随;
        if (需要平滑过渡)
        {
            平滑移动到目标位置(目标中心位置);
        }
        else
        {
            直接设置到目标位置(目标中心位置);
        }
    }

    /// <summary>
    /// 计算两个追踪目标的世界坐标中心点，Z 轴使用配置的相机深度
    /// </summary>
    private Vector3 计算两目标中心点()
    {
        Vector3 身体位置 = 人物身体.position;
        Vector3 头部位置 = 人物头部.position;

        float 中心点X = (身体位置.x + 头部位置.x) / 中点除数;
        float 中心点Y = (身体位置.y + 头部位置.y) / 中点除数;

        Vector3 目标中心点 = new Vector3(中心点X, 中心点Y, 相机Z深度);
        return 目标中心点;
    }

    /// <summary>
    /// 使用基于指数衰减的 Lerp 平滑移动相机，跟随速度与帧率无关
    /// </summary>
    private void 平滑移动到目标位置(Vector3 目标位置)
    {
        float 插值系数 = 1f - Mathf.Exp(-平滑跟随速度 * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, 目标位置, 插值系数);
    }

    /// <summary>
    /// 直接将相机设置到目标位置，无任何过渡
    /// </summary>
    private void 直接设置到目标位置(Vector3 目标位置)
    {
        transform.position = 目标位置;
    }
}
