using UnityEngine;

/// <summary>
/// 告示牌内容显示控制器
/// 在告示牌上方附加图标提示，用于玩法教程和路径指引
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SignContentDisplay : MonoBehaviour
{
    private const float 零值 = 0f;
    private const int 图标排序偏移 = 1;

    [Section("图标配置")]
    [SerializeField]
    private Sprite 主图标;

    [SerializeField]
    private Sprite 副图标;

    [SerializeField]
    private Vector2 图标偏移量 = new Vector2(0f, 0.5f);

    [SerializeField]
    private float 图标尺寸 = 0.5f;

    [Section("动画效果")]
    [SerializeField]
    private bool 启用浮动动画 = true;

    [SerializeField]
    private float 浮动幅度 = 0.1f;

    [SerializeField]
    private float 浮动速度 = 1.5f;

    private SpriteRenderer 主图标渲染器;
    private SpriteRenderer 副图标渲染器;
    private GameObject 图标父对象;
    private Vector3 图标基准位置;

    private void Start()
    {
        创建图标显示();
    }

    private void LateUpdate()
    {
        if (主图标渲染器 == null)
            return;

        if (启用浮动动画)
        {
            // 轻微上下浮动
            float Y偏移 = Mathf.Sin(Time.time * 浮动速度) * 浮动幅度;
            Vector3 浮动位置 = 图标基准位置;
            浮动位置.y += Y偏移;
            主图标渲染器.transform.position = 浮动位置;
        }
    }

    private void 创建图标显示()
    {
        SpriteRenderer 基础渲染器 = GetComponent<SpriteRenderer>();
        int 基础排序序号 = 基础渲染器.sortingOrder;

        // 创建图标父对象
        图标父对象 = new GameObject(gameObject.name + "_图标");
        图标父对象.transform.SetParent(transform);
        图标父对象.transform.localPosition = 图标偏移量;
        图标基准位置 = 图标父对象.transform.position;

        // 主图标
        if (主图标 != null)
        {
            主图标渲染器 = 创建图标精灵(主图标, 基础排序序号 + 图标排序偏移);
        }

        // 副图标（可选，在下方显示）
        if (副图标 != null)
        {
            副图标渲染器 = 创建图标精灵(副图标, 基础排序序号 + 图标排序偏移);
            副图标渲染器.transform.localPosition = new Vector3(零值, -图标尺寸 * 1.2f, 零值);
            副图标渲染器.transform.localScale = Vector3.one * 图标尺寸 * 0.7f;
        }

        if (主图标渲染器 != null)
            主图标渲染器.transform.localScale = Vector3.one * 图标尺寸;
    }

    private SpriteRenderer 创建图标精灵(Sprite 精灵, int 排序序号)
    {
        GameObject 图标对象 = new GameObject(精灵.name);
        图标对象.transform.SetParent(图标父对象.transform);
        图标对象.transform.localPosition = Vector3.zero;

        SpriteRenderer 渲染器 = 图标对象.AddComponent<SpriteRenderer>();
        渲染器.sprite = 精灵;
        渲染器.sortingOrder = 排序序号;
        渲染器.color = Color.white;

        return 渲染器;
    }

    /// <summary>
    /// 设置图标（外部调用）
    /// </summary>
    public void 设置图标(Sprite 新主图标, Sprite 新副图标 = null)
    {
        主图标 = 新主图标;
        副图标 = 新副图标;

        // 重新创建
        if (主图标渲染器 != null)
            Destroy(主图标渲染器.gameObject);

        if (副图标渲染器 != null)
            Destroy(副图标渲染器.gameObject);

        创建图标显示();
    }

    /// <summary>
    /// 设置可见性
    /// </summary>
    public void 设置可见性(bool 可见)
    {
        if (图标父对象 != null)
            图标父对象.SetActive(可见);
    }
}
