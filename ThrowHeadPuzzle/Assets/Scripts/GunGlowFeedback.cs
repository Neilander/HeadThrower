using UnityEngine;

/// <summary>
/// 枪械HDR描边发光反馈
/// 头部与身体分离时激活脉冲发光效果，提示枪械可交互
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(HeadShoot))]
public class GunGlowFeedback : MonoBehaviour
{
    private const float 零值 = 0f;
    private const float 发光脉冲速度 = 3f;
    private const float 发射闪光增强倍率 = 3f;
    private const float 最小发光强度 = 0.2f;
    private const float 最大发光强度 = 1.5f;
    private const float 外部描边默认尺寸 = 3f;

    [Section("组件引用")]
    [SerializeField]
    private SpriteRenderer 精灵渲染器;

    [Section("发光配置")]
    [SerializeField]
    private Color 发光颜色 = new Color(3f, 2f, 0f, 1f); // HDR暖黄色

    [SerializeField]
    [Range(1, 10)]
    private int 描边尺寸 = 3;

    [SerializeField]
    private bool 启用发光 = true;

    private HeadShoot 头部射击组件;
    private PickUpHead 头部拾取组件;
    private MaterialPropertyBlock 材质属性块;
    private bool 上次描边状态;
    private float 当前发光强度;
    private float 发射闪光计时器;

    private static readonly int 描边启用属性ID = Shader.PropertyToID("_IsOutlineEnabled");
    private static readonly int 描边颜色属性ID = Shader.PropertyToID("_OutlineColor");
    private static readonly int 描边尺寸属性ID = Shader.PropertyToID("_OutlineSize");
    private static readonly int 透明度阈值属性ID = Shader.PropertyToID("_AlphaThreshold");

    private void Awake()
    {
        // 自动获取引用
        if (精灵渲染器 == null)
            精灵渲染器 = GetComponent<SpriteRenderer>();

        头部射击组件 = GetComponent<HeadShoot>();
        头部拾取组件 = GetComponent<PickUpHead>();

        if (头部拾取组件 == null)
            头部拾取组件 = GetComponentInParent<PickUpHead>();

        材质属性块 = new MaterialPropertyBlock();

        // 初始关闭描边
        精灵渲染器.GetPropertyBlock(材质属性块);
        材质属性块.SetFloat(描边启用属性ID, 0f);
        精灵渲染器.SetPropertyBlock(材质属性块);
        上次描边状态 = false;
    }

    private void Update()
    {
        if (!启用发光 || 精灵渲染器 == null || 头部拾取组件 == null)
            return;

        // 发射闪光计时器衰减
        if (发射闪光计时器 > 零值)
            发射闪光计时器 -= Time.deltaTime;

        bool 头部分离 = !头部拾取组件.isPickUp;

        if (头部分离)
        {
            更新发光脉冲();
        }
        else
        {
            关闭发光();
        }
    }

    /// <summary>
    /// 外部调用：标记发射闪光，在射击瞬间增强发光
    /// </summary>
    public void 触发发射闪光()
    {
        发射闪光计时器 = 0.15f;
    }

    private void 更新发光脉冲()
    {
        // 计算脉冲强度（正弦波）
        float 脉冲值 = Mathf.Sin(Time.time * 发光脉冲速度);
        当前发光强度 = Mathf.Lerp(最小发光强度, 最大发光强度, 脉冲值 * 0.5f + 0.5f);

        // 发射闪光叠加
        float 发射增强 = 发射闪光计时器 > 零值 ? 发射闪光增强倍率 : 1f;
        float 最终强度 = 当前发光强度 * 发射增强;

        // 应用材质属性
        精灵渲染器.GetPropertyBlock(材质属性块);
        材质属性块.SetFloat(描边启用属性ID, 1f);
        材质属性块.SetColor(描边颜色属性ID, 发光颜色 * 最终强度);
        材质属性块.SetFloat(描边尺寸属性ID, 描边尺寸);
        材质属性块.SetFloat(透明度阈值属性ID, 0.01f);
        精灵渲染器.SetPropertyBlock(材质属性块);

        上次描边状态 = true;
    }

    private void 关闭发光()
    {
        if (!上次描边状态)
            return;

        精灵渲染器.GetPropertyBlock(材质属性块);
        材质属性块.SetFloat(描边启用属性ID, 0f);
        精灵渲染器.SetPropertyBlock(材质属性块);

        上次描边状态 = false;
    }

    private void OnDisable()
    {
        // 禁用时确保描边关闭
        if (精灵渲染器 != null)
        {
            精灵渲染器.GetPropertyBlock(材质属性块);
            材质属性块.SetFloat(描边启用属性ID, 0f);
            精灵渲染器.SetPropertyBlock(材质属性块);
        }
    }
}
