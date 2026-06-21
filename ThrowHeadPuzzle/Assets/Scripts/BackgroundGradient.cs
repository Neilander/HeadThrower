using UnityEngine;

/// <summary>
/// 背景渐变控制器
/// 在场景中创建一个全屏渐变Quad，实现背景从上到下的色彩渐变
/// 使用 SpriteRenderer 替代相机清除，实现更有层次感的背景
/// </summary>
public class BackgroundGradient : MonoBehaviour
{
    private const int 渐变纹理尺寸 = 16;

    [Section("渐变颜色配置")]
    [SerializeField]
    private Color 顶部颜色 = new Color(0.05f, 0.05f, 0.12f, 1f); // 深空蓝

    [SerializeField]
    private Color 底部颜色 = new Color(0.12f, 0.18f, 0.28f, 1f); // 稍亮深蓝

    [Section("渲染设置")]
    [SerializeField]
    private string 排序层名 = "Background";

    [SerializeField]
    private int 排序序号 = -100;

    [Section("视差效果")]
    [SerializeField]
    private bool 启用视差效果 = true;

    [SerializeField]
    [Range(0f, 1f)]
    private float 视差跟随速度 = 0.1f;

    private SpriteRenderer 渐变渲染器;
    private Camera 主相机;
    private Vector3 初始位置;

    private void Start()
    {
        主相机 = Camera.main;

        // 创建渐变游戏对象
        GameObject 渐变对象 = new GameObject("_背景渐变");
        渐变对象.transform.SetParent(transform);

        渐变渲染器 = 渐变对象.AddComponent<SpriteRenderer>();
        渐变渲染器.sortingLayerName = 排序层名;
        渐变渲染器.sortingOrder = 排序序号;

        // 生成渐变纹理
        生成渐变纹理();

        // 调整大小覆盖整个相机视野
        if (主相机 != null && 主相机.orthographic)
        {
            float 高度 = 主相机.orthographicSize * 2f;
            float 宽度 = 高度 * 主相机.aspect;
            渐变对象.transform.localScale = new Vector3(宽度 * 3f, 高度 * 3f, 1f);
            初始位置 = transform.position;
            渐变对象.transform.position = 初始位置;
        }
    }

    private void LateUpdate()
    {
        if (启用视差效果 && 主相机 != null && 渐变渲染器 != null)
        {
            // 视差滚动：以极慢速度跟随相机
            Vector3 目标位置 = 主相机.transform.position;
            目标位置.z = 0f;
            transform.position = Vector3.Lerp(transform.position, 目标位置, 视差跟随速度 * Time.deltaTime);
        }
    }

    private void 生成渐变纹理()
    {
        Texture2D 纹理 = new Texture2D(渐变纹理尺寸, 渐变纹理尺寸);
        纹理.filterMode = FilterMode.Point;
        纹理.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < 渐变纹理尺寸; y++)
        {
            float t = (float)y / (渐变纹理尺寸 - 1);
            Color 像素颜色 = Color.Lerp(底部颜色, 顶部颜色, t);

            for (int x = 0; x < 渐变纹理尺寸; x++)
            {
                纹理.SetPixel(x, y, 像素颜色);
            }
        }
        纹理.Apply();

        渐变渲染器.sprite = Sprite.Create(纹理, new Rect(0, 0, 渐变纹理尺寸, 渐变纹理尺寸), new Vector2(0.5f, 0.5f), 1);
        渐变渲染器.color = Color.white;
    }

    /// <summary>
    /// 外部调用更新渐变颜色
    /// </summary>
    public void 更新渐变颜色(Color 新顶部颜色, Color 新底部颜色)
    {
        顶部颜色 = 新顶部颜色;
        底部颜色 = 新底部颜色;
        if (渐变渲染器 != null)
            生成渐变纹理();
    }
}
