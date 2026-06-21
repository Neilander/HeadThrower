using UnityEngine;

/// <summary>
/// 可交互物体高亮控制器
/// 对可交互物体（NPC、开关、拾取物等）添加HDR描边效果
/// 在玩家接近时自动激活高亮
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class InteractiveOutline : MonoBehaviour
{
    private const float 零值 = 0f;

    [Section("组件引用")]
    [SerializeField]
    private SpriteRenderer 精灵渲染器;

    [Section("高亮配置")]
    [SerializeField]
    private Color 高亮颜色 = new Color(0.5f, 1.5f, 0.5f, 1f); // 绿色HDR

    [SerializeField]
    [Range(1, 10)]
    private int 描边尺寸 = 2;

    [SerializeField]
    private bool 始终高亮 = false;

    [Section("检测范围")]
    [SerializeField]
    private float 检测半径 = 3f;

    [SerializeField]
    private LayerMask 玩家层级;

    private MaterialPropertyBlock 材质属性块;
    private bool 上次描边状态;
    private Transform 玩家变换;
    private bool 玩家在范围内;

    private static readonly int 描边启用属性ID = Shader.PropertyToID("_IsOutlineEnabled");
    private static readonly int 描边颜色属性ID = Shader.PropertyToID("_OutlineColor");
    private static readonly int 描边尺寸属性ID = Shader.PropertyToID("_OutlineSize");
    private static readonly int 透明度阈值属性ID = Shader.PropertyToID("_AlphaThreshold");

    private void Awake()
    {
        if (精灵渲染器 == null)
            精灵渲染器 = GetComponent<SpriteRenderer>();

        材质属性块 = new MaterialPropertyBlock();

        // 初始关闭
        精灵渲染器.GetPropertyBlock(材质属性块);
        材质属性块.SetFloat(描边启用属性ID, 0f);
        精灵渲染器.SetPropertyBlock(材质属性块);
        上次描边状态 = false;
    }

    private void Start()
    {
        // 查找玩家
        GameObject 玩家对象 = GameObject.FindGameObjectWithTag("Player");
        if (玩家对象 != null)
            玩家变换 = 玩家对象.transform;
    }

    private void Update()
    {
        if (精灵渲染器 == null)
            return;

        if (始终高亮)
        {
            启用高亮();
            return;
        }

        // 检测玩家距离
        if (玩家变换 != null)
        {
            float 距离 = Vector2.Distance(transform.position, 玩家变换.position);
            玩家在范围内 = 距离 <= 检测半径;

            if (玩家在范围内)
                启用高亮();
            else
                关闭高亮();
        }
    }

    private void 启用高亮()
    {
        if (上次描边状态)
            return;

        精灵渲染器.GetPropertyBlock(材质属性块);
        材质属性块.SetFloat(描边启用属性ID, 1f);
        材质属性块.SetColor(描边颜色属性ID, 高亮颜色);
        材质属性块.SetFloat(描边尺寸属性ID, 描边尺寸);
        材质属性块.SetFloat(透明度阈值属性ID, 0.01f);
        精灵渲染器.SetPropertyBlock(材质属性块);

        上次描边状态 = true;
    }

    private void 关闭高亮()
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
        if (精灵渲染器 != null)
        {
            精灵渲染器.GetPropertyBlock(材质属性块);
            材质属性块.SetFloat(描边启用属性ID, 0f);
            精灵渲染器.SetPropertyBlock(材质属性块);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!始终高亮)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 检测半径);
        }
    }
}
