using UnityEngine;

public enum ShakeIntensity
{
    轻微 = 1,
    适中 = 5,
    剧烈 = 10,
    爆裂 = 20,
}

public class HeadShoot : BaseInteraction
{
    private const float 零值 = 0f;
    private const float 枪械默认Z旋转角 = 0f;
    private const float 默认射击间隔 = 0.1f;
    private const float 使用子弹池默认间隔标记 = -1f;

    private const int 鼠标左键索引 = 0;

    private const string 组件引用分组名称 = "组件引用";
    private const string 基础配置分组名称 = "基础配置";
    private const string 射击设置分组名称 = "射击设置";
    private const string 特效设置分组名称 = "特效设置";

    [Section(组件引用分组名称)]
    [SerializeField]
    private PickUpHead 头部拾取组件;

    [Section(基础配置分组名称)]
    [SerializeField]
    private Vector2 初始位置;

    [Section(射击设置分组名称)]
    [SerializeField]
    private bool 使用子弹池默认射击间隔 = true;

    [Section(射击设置分组名称)]
    [SerializeField]
    private float 射击间隔 = 默认射击间隔;

    [Section(特效设置分组名称)]
    [SerializeField]
    private ParticleSystem 枪口火焰特效;

    [SerializeField]
    private ShakeIntensity 震动等级;

    [Section(特效设置分组名称)]
    [SerializeField]
    private MuzzleFlareController 弹壳特效控制器;

    [Section(特效设置分组名称)]
    [SerializeField]
    private GunGlowFeedback 枪械发光反馈;

    private Vector2 枪械朝向;

    private void Awake()
    {
        bool 头部拾取组件缺失 = 头部拾取组件 == null;
        if (头部拾取组件缺失)
        {
            头部拾取组件 = GetComponent<PickUpHead>();
        }

        if (枪械发光反馈 == null)
            枪械发光反馈 = GetComponent<GunGlowFeedback>();
    }

    private void Update()
    {
        bool 头部拾取组件缺失 = 头部拾取组件 == null;
        if (头部拾取组件缺失)
        {
            return;
        }

        bool 头部已分离 = !头部拾取组件.isPickUp;
        bool 按住射击键 = Input.GetMouseButton(鼠标左键索引);
        bool 尝试射击 = 头部已分离 && 按住射击键;

        if (尝试射击)
        {
            处理持续射击输入();
            屏幕震动();
        }
        else
        {
            重置枪械旋转();
        }
    }

    private void 屏幕震动()
    {
        CameraShakeManager.实例.触发震动((float)震动等级 / 100f);
    }

    public override bool OnInteract(InteractionSignal 交互信号)
    {
        BulletPool 子弹池 = BulletPool.Instance;

        bool 子弹池不可用 = 子弹池 == null;
        if (子弹池不可用)
        {
            return false;
        }

        float 射击间隔覆盖值 = 获取射击间隔覆盖值();
        Vector3 子弹生成位置 = 获取子弹生成世界位置();
        Quaternion 子弹生成旋转 = Quaternion.Euler(零值, 零值, 枪械默认Z旋转角);

        GameObject 子弹 = 子弹池.尝试发射子弹(
            gameObject.GetInstanceID(),
            子弹生成位置,
            子弹生成旋转,
            枪械朝向,
            射击间隔覆盖值,
            枪口火焰特效
        );

        bool 发射失败 = 子弹 == null;
        if (发射失败)
        {
            return false;
        }

        传递子弹射出方向(子弹);
        播放弹壳特效();

        // 触发枪械发光反馈的发射闪光
        if (枪械发光反馈 != null)
            枪械发光反馈.触发发射闪光();

        return true;
    }

    private void 处理持续射击输入()
    {
        Vector3 当前本地缩放 = transform.localScale;

        枪械朝向 = new Vector2(当前本地缩放.x, 当前本地缩放.y);
        枪械朝向.y = 零值;

        InteractionSignal 交互信号 = new InteractionSignal(
            gameObject,
            InteractionType.KeyPress,
            枪械朝向
        );

        OnInteract(交互信号);
    }

    private void 重置枪械旋转()
    {
        transform.rotation = Quaternion.Euler(零值, 零值, 枪械默认Z旋转角);
    }

    private float 获取射击间隔覆盖值()
    {
        float 射击间隔覆盖值 = 射击间隔;

        bool 使用默认射击间隔 = 使用子弹池默认射击间隔;
        if (使用默认射击间隔)
        {
            射击间隔覆盖值 = 使用子弹池默认间隔标记;
        }

        return 射击间隔覆盖值;
    }

    private Vector3 获取子弹生成世界位置()
    {
        Vector3 本地生成位置 = new Vector3(初始位置.x, 初始位置.y, 零值);
        Vector3 世界生成位置 = transform.TransformPoint(本地生成位置);
        return 世界生成位置;
    }

    private void 传递子弹射出方向(GameObject 子弹)
    {
        bool 子弹为空 = 子弹 == null;
        if (子弹为空)
        {
            return;
        }

        ShootOut 子弹射出组件 = 子弹.GetComponent<ShootOut>();

        bool 子弹射出组件缺失 = 子弹射出组件 == null;
        if (子弹射出组件缺失)
        {
            return;
        }

        子弹射出组件.GetSignal(枪械朝向);
    }

    private void 播放弹壳特效()
    {
        弹壳特效控制器.Play();
    }

    public GunGlowFeedback 获取枪械发光反馈()
    {
        return 枪械发光反馈;
    }
}
