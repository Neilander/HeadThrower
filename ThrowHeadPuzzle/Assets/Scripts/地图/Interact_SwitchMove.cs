using UnityEngine;

[RequireComponent(typeof(WorldMover))]
public class Interact_SwitchMove : BaseInteraction
{
    #region 常量定义
    private const KeyCode 交互按键 = KeyCode.Q;
    #endregion

    [Section("组件引用", "#4CAF50")]
    private WorldMover _移动组件;

    [Section("路径坐标配置", "#2196F3")]
    [SerializeField]
    private Transform 位置1;

    [SerializeField]
    private Transform 位置2;

    [Section("交互提示视觉", "#FF9800")]
    public Transform 按键提示物体;

    private bool _是否处于位置1 = true;
    private SpriteRenderer _提示图标渲染器;

    private void Start()
    {
        // 获取移动组件引用
        _移动组件 = GetComponent<WorldMover>();

        bool 提示物体存在 = 按键提示物体 != null;
        if (提示物体存在)
        {
            _提示图标渲染器 = 按键提示物体.GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        // 提取控制流条件
        bool 玩家按下交互键 = Input.GetKeyDown(交互按键);

        if (玩家按下交互键)
        {
            // 统一调用交互接口
            OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
        }
    }

    public override bool OnInteract(InteractionSignal 信号)
    {
        bool 渲染器有效 = _提示图标渲染器 != null;
        if (渲染器有效)
        {
            _提示图标渲染器.enabled = false;
        }

        // 提取信号类型判断
        bool 信号类型不匹配 = 信号.type != InteractionType.KeyPress;
        if (信号类型不匹配)
            return false;

        // 执行移动逻辑判断
        bool 目标是位置2 = _是否处于位置1;
        if (目标是位置2)
        {
            // 修复错误：调用重构后的“移动至”方法
            _移动组件.移动至(位置2.position);
        }
        else
        {
            _移动组件.移动至(位置1.position);
        }

        // 切换状态
        _是否处于位置1 = !_是否处于位置1;
        return true;
    }

    private void OnTriggerEnter2D(Collider2D 碰撞体)
    {
        bool 碰到了玩家 = 碰撞体.CompareTag("Player");
        if (碰到了玩家)
        {
            bool 渲染器存在 = _提示图标渲染器 != null;
            if (渲染器存在)
            {
                _提示图标渲染器.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D 碰撞体)
    {
        bool 玩家离开 = 碰撞体.CompareTag("Player");
        if (玩家离开)
        {
            bool 渲染器存在 = _提示图标渲染器 != null;
            if (渲染器存在)
            {
                _提示图标渲染器.enabled = false;
            }
        }
    }
}
