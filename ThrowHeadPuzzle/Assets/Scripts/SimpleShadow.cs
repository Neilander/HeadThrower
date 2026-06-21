using UnityEngine;

/// <summary>
/// 简易像素风投影生成器
/// 对目标物体通过 Physics2D.Raycast 检测地面，绘制半透明投影
/// </summary>
public class SimpleShadow : MonoBehaviour
{
    private const float 零值 = 0f;
    private const float 投影默认透明度 = 0.3f;
    private const float 最大投影距离 = 50f;
    private const int 投影排序层级偏移 = -10;

    [Section("投影配置")]
    [SerializeField]
    private Color 投影颜色 = new Color(0f, 0f, 0f, 0.3f);

    [SerializeField]
    private Vector2 投影偏移量 = new Vector2(0.15f, -0.1f);

    [SerializeField]
    private float 投影缩放比例 = 1f;

    [SerializeField]
    private LayerMask 地面层级 = -1;

    [Section("组件引用")]
    [SerializeField]
    private SpriteRenderer 投影精灵;

    [SerializeField]
    private Sprite 投影形状精灵;

    [SerializeField]
    private bool 运行时创建投影 = true;

    private SpriteRenderer 自身渲染器;

    private void Start()
    {
        自身渲染器 = GetComponent<SpriteRenderer>();

        if (!运行时创建投影)
            return;

        if (投影精灵 == null)
            创建投影精灵();

        if (投影形状精灵 == null)
            生成默认投影形状();
    }

    private void LateUpdate()
    {
        if (投影精灵 == null)
            return;

        更新投影位置();
    }

    private void 创建投影精灵()
    {
        GameObject 投影对象 = new GameObject(gameObject.name + "_投影");
        投影对象.transform.SetParent(transform.parent);
        投影对象.transform.localScale = Vector3.one;

        投影精灵 = 投影对象.AddComponent<SpriteRenderer>();
        投影精灵.color = 投影颜色;
        投影精灵.sortingOrder = -10; // 在大多数物体下方渲染
    }

    private void 生成默认投影形状()
    {
        // 根据自身大小生成一个简单的椭圆形投影纹理
        Bounds 自身包围盒 = 自身渲染器 != null ? 自身渲染器.bounds : new Bounds(transform.position, Vector3.one);
        float 宽度 = 自身包围盒.size.x * 投影缩放比例;
        float 高度 = 自身包围盒.size.y * 投影缩放比例 * 0.3f;

        // 创建一个简单的像素风圆形投影纹理
        int 纹理尺寸 = 32;
        Texture2D 纹理 = new Texture2D(纹理尺寸, 纹理尺寸);
        纹理.filterMode = FilterMode.Point;

        float 中心X = 纹理尺寸 / 2f;
        float 中心Y = 纹理尺寸 / 2f;
        float 半径 = 纹理尺寸 / 2f * 0.7f;

        for (int y = 0; y < 纹理尺寸; y++)
        {
            for (int x = 0; x < 纹理尺寸; x++)
            {
                float 距离X = (x - 中心X) / 半径;
                float 距离Y = (y - 中心Y) / (半径 * 0.5f);
                float 距离 = Mathf.Sqrt(距离X * 距离X + 距离Y * 距离Y);

                if (距离 <= 1f)
                {
                    // 从中心到边缘渐变透明度
                    float 透明度 = Mathf.Lerp(0.8f, 0f, 距离);
                    纹理.SetPixel(x, y, new Color(1f, 1f, 1f, 透明度));
                }
                else
                {
                    纹理.SetPixel(x, y, Color.clear);
                }
            }
        }
        纹理.Apply();

        投影形状精灵 = Sprite.Create(纹理, new Rect(0, 0, 纹理尺寸, 纹理尺寸), new Vector2(0.5f, 0.5f), 16);
        投影精灵.sprite = 投影形状精灵;

        // 根据物体实际大小调整投影精灵尺寸
        float 缩放X = 宽度 / (纹理尺寸 / 16f);
        float 缩放Y = 高度 / (纹理尺寸 / 16f);
        投影精灵.transform.localScale = new Vector3(缩放X, 缩放Y, 1f);
    }

    private void 更新投影位置()
    {
        Vector3 投射起点 = transform.position + new Vector3(投影偏移量.x, 零值, 零值);
        RaycastHit2D 命中 = Physics2D.Raycast(投射起点, Vector2.down, 最大投影距离, 地面层级);

        if (命中.collider != null)
        {
            float 距离 = Vector2.Distance(投射起点, 命中.point);
            Vector3 投影位置 = 命中.point + new Vector2(零值, 投影偏移量.y);
            投影位置.z = 零值;
            投影精灵.transform.position = 投影位置;

            // 根据距离调整投影透明度和缩放（越远越淡、越大）
            float 距离因子 = Mathf.Clamp01(距离 / 10f);
            Color 颜色 = 投影颜色;
            颜色.a = 投影颜色.a * Mathf.Lerp(1f, 0.3f, 距离因子);
            投影精灵.color = 颜色;

            float 缩放因子 = Mathf.Lerp(1f, 1.5f, 距离因子);
            投影精灵.transform.localScale = new Vector3(
                投影精灵.transform.localScale.x * 缩放因子 / Mathf.Max(投影精灵.transform.localScale.x, 0.01f),
                投影精灵.transform.localScale.y * 缩放因子 / Mathf.Max(投影精灵.transform.localScale.y, 0.01f),
                1f
            );
        }
        else
        {
            // 无落点时隐藏投影
            投影精灵.color = Color.clear;
        }
    }

    private void OnDestroy()
    {
        if (投影精灵 != null && 投影精灵.gameObject != null)
            Destroy(投影精灵.gameObject);
    }
}
