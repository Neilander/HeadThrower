using UnityEngine;

/// <summary>
/// 地图碰撞平台脚本
/// </summary>
public class HitPlat : MonoBehaviour
{
    // 重构1：提取魔法字符串为语义化常量
    private const string 子弹标签 = "Bullet";

    [Header("开关状态设置")]
    // 重构3：英文变量改为中文语义命名
    public bool 已开启;
    public Sprite 关闭精灵;
    public Sprite 开启精灵;

    // 重构3：英文变量改为中文语义命名
    private SpriteRenderer 精灵渲染器;

    void Awake()
    {
        精灵渲染器 = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D 碰撞对象)
    {
        Debug.Log("Bullet hitted");

        // 重构2：将if条件提取为独立bool变量
        bool 碰撞对象不是子弹 = 碰撞对象.CompareTag(子弹标签) == false;
        if (碰撞对象不是子弹)
            return;

        // 原始逻辑保留（注释状态）
        //ChangeSprite();

        // 原始逻辑保留：销毁自身
        Destroy(gameObject);
    }

    /// <summary>
    /// 切换精灵显示状态
    /// </summary>
    void 切换精灵状态()
    {
        已开启 = !已开启;
        精灵渲染器.sprite = 已开启 ? 开启精灵 : 关闭精灵;
    }
}
