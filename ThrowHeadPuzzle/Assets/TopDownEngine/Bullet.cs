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

    // 🔥 修复：获取子物体的烟雾拖尾（唯一修改点）
    [SerializeField]
    private ParticleSystem 烟雾拖尾粒子;

    // 🔥 新增：碰撞冲击粒子
    [SerializeField]
    private ParticleSystem 冲击粒子;

    // ===================== 初始化 =====================
    private void Awake()
    {
        //子弹刚体 = GetComponent<Rigidbody2D>();
    }

    public void 播放拖尾特效()
    {
        bool 拖尾存在 = (烟雾拖尾粒子 != null);
        if (拖尾存在)
            烟雾拖尾粒子.Play(); // 🔥 移动时持续播放拖尾
        else
        {
            Debug.LogWarning("【子弹】没有找到拖尾粒子！");
        }
    }

    public void 播放冲击特效()
    {
        // 核心诊断日志

        if (冲击粒子 != null)
        {
            冲击粒子.Clear();
            冲击粒子.transform.position = transform.position;
            冲击粒子.Play();
        }
        else
        {
            Debug.LogWarning("【Bullet】冲击粒子为Null，无法播放", this);
        }
    }
}
