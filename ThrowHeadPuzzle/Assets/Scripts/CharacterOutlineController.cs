using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 角色轮廓高光控制器
/// 在头部与身体分离时为角色所有可见部分添加HDR描边轮廓，提高可见度
/// 自动检测子物体中的 SpriteRenderer 并统一控制
/// </summary>
public class CharacterOutlineController : MonoBehaviour
{
    private const float 零值 = 0f;

    [Section("轮廓配置")]
    [SerializeField]
    private Color 轮廓颜色 = new Color(1.5f, 1.5f, 1.5f, 1f); // 浅灰HDR

    [SerializeField]
    [Range(1, 10)]
    private int 描边尺寸 = 1;

    [SerializeField]
    private bool 头部已分离时启用轮廓 = true;

    [SerializeField]
    private PlayerController 玩家控制器;

    // 所有需要描边的 SpriteRenderer（自动收集）
    private List<SpriteRenderer> 所有精灵渲染器 = new List<SpriteRenderer>();
    private MaterialPropertyBlock 材质属性块;
    private bool 上次描边状态;

    private static readonly int 描边启用属性ID = Shader.PropertyToID("_IsOutlineEnabled");
    private static readonly int 描边颜色属性ID = Shader.PropertyToID("_OutlineColor");
    private static readonly int 描边尺寸属性ID = Shader.PropertyToID("_OutlineSize");
    private static readonly int 透明度阈值属性ID = Shader.PropertyToID("_AlphaThreshold");

    private void Awake()
    {
        if (玩家控制器 == null)
            玩家控制器 = GetComponent<PlayerController>();

        材质属性块 = new MaterialPropertyBlock();

        // 收集所有子物体中有精灵的渲染器（排除头部/枪械）
        收集可见精灵渲染器();
    }

    private void 收集可见精灵渲染器()
    {
        所有精灵渲染器.Clear();

        SpriteRenderer 自身渲染器 = GetComponent<SpriteRenderer>();
        if (自身渲染器 != null && 自身渲染器.sprite != null)
            所有精灵渲染器.Add(自身渲染器);

        // 遍历所有子物体，收集 SpriteRenderer
        foreach (Transform 子物体 in transform)
        {
            // 跳过头部（gun_head）的子物体
            if (子物体.CompareTag("Head"))
                continue;

            SpriteRenderer 子渲染器 = 子物体.GetComponent<SpriteRenderer>();
            if (子渲染器 != null && 子渲染器.sprite != null)
                所有精灵渲染器.Add(子渲染器);
        }

        // 初始关闭所有轮廓
        foreach (var 渲染器 in 所有精灵渲染器)
        {
            渲染器.GetPropertyBlock(材质属性块);
            材质属性块.SetFloat(描边启用属性ID, 0f);
            渲染器.SetPropertyBlock(材质属性块);
        }
        上次描边状态 = false;
    }

    private void Update()
    {
        if (玩家控制器 == null || 所有精灵渲染器.Count == 0)
            return;

        bool 头部已分离 = 玩家控制器.curstate == PlayerController.ThrowState.NoHead;
        bool 需要描边 = 头部已分离时启用轮廓 && 头部已分离;

        if (需要描边)
        {
            启用轮廓();
        }
        else
        {
            关闭轮廓();
        }
    }

    private void 启用轮廓()
    {
        if (上次描边状态)
            return;

        foreach (var 渲染器 in 所有精灵渲染器)
        {
            渲染器.GetPropertyBlock(材质属性块);
            材质属性块.SetFloat(描边启用属性ID, 1f);
            材质属性块.SetColor(描边颜色属性ID, 轮廓颜色);
            材质属性块.SetFloat(描边尺寸属性ID, 描边尺寸);
            材质属性块.SetFloat(透明度阈值属性ID, 0.01f);
            渲染器.SetPropertyBlock(材质属性块);
        }

        上次描边状态 = true;
    }

    private void 关闭轮廓()
    {
        if (!上次描边状态)
            return;

        foreach (var 渲染器 in 所有精灵渲染器)
        {
            渲染器.GetPropertyBlock(材质属性块);
            材质属性块.SetFloat(描边启用属性ID, 0f);
            渲染器.SetPropertyBlock(材质属性块);
        }

        上次描边状态 = false;
    }

    private void OnDisable()
    {
        foreach (var 渲染器 in 所有精灵渲染器)
        {
            if (渲染器 != null)
            {
                渲染器.GetPropertyBlock(材质属性块);
                材质属性块.SetFloat(描边启用属性ID, 0f);
                渲染器.SetPropertyBlock(材质属性块);
            }
        }
    }
}
