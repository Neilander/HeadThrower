using UnityEngine;

/// <summary>
/// 子弹核心脚本
/// 对接EventManager，碰撞时抛出事件
/// </summary>
public class Bullet : MonoBehaviour
{
    // ===================== 语义化常量 =====================
    private const float 默认飞行速度 = 10f;
    private const float 默认存活时间 = 2f;
    private const int 默认伤害值 = 1;
    private const string 敌人标签 = "Enemy";
    private const string 障碍物标签 = "Obstacle";

    // 【事件名称常量】和反馈脚本监听的名称对应
    private const string 事件_命中敌人 = "OnBulletHitEnemy";
    private const string 事件_命中障碍物 = "OnBulletHitObstacle";

    // ===================== 中文变量 =====================
    private Rigidbody2D 子弹刚体;
    private ParticleSystem 烟雾拖尾粒子;

    // ===================== 初始化 =====================
    private void Awake()
    {
        子弹刚体 = GetComponent<Rigidbody2D>();
        // 获取自己的 ParticleSystem
        烟雾拖尾粒子 = GetComponent<ParticleSystem>();
    }

    // ===================== 发射子弹 =====================
    public void 发射子弹(Vector2 飞行方向)
    {
        Vector2 标准方向 = 飞行方向.normalized;
        子弹刚体.velocity = 标准方向 * 默认飞行速度;

        bool 拖尾存在 = (烟雾拖尾粒子 != null);
        if (拖尾存在)
            烟雾拖尾粒子.Play();

        Invoke(nameof(回收至对象池), 默认存活时间);
    }

    // ===================== 碰撞检测（抛出事件） =====================
    private void OnTriggerEnter2D(Collider2D 碰撞对象)
    {
        bool 击中敌人 = 碰撞对象.CompareTag(敌人标签);
        bool 击中障碍物 = 碰撞对象.CompareTag(障碍物标签);
        bool 事件系统可用 = (EventManager.Instance != null);

        // 击中敌人 → 抛事件
        if (击中敌人 && 事件系统可用)
        {
            EventManager.Instance.触发事件(事件_命中敌人);
        }

        // 击中障碍物 → 抛事件
        if (击中障碍物 && 事件系统可用)
        {
            EventManager.Instance.触发事件(事件_命中障碍物);
        }

        // 回收子弹
        回收至对象池();
    }

    // ===================== 回收 =====================
    private void 回收至对象池()
    {
        CancelInvoke();
        子弹刚体.velocity = Vector2.zero;

        bool 拖尾存在 = (烟雾拖尾粒子 != null);
        if (拖尾存在)
            烟雾拖尾粒子.Stop();

        gameObject.SetActive(false);
    }
}
