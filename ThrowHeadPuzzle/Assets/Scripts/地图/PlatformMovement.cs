using UnityEngine;

public class PlatformMovement : BaseInteraction
{
    #region 常量定义
    private const float 最小移动阈值 = 0.0001f;
    #endregion

    [Section("引用组件", "#FF9800")]
    private WorldMover _移动器;
    private Transform _玩家变换组件;

    [Section("路径坐标", "#00BCD4")]
    [SerializeField]
    private Transform 路径点1;

    [SerializeField]
    private Transform 路径点2;

    [Section("状态监控", "#9C27B0")]
    [SerializeField]
    private bool _玩家是否在平台上 = false;

    [SerializeField]
    private bool _当前位于点1 = true;

    [Section("交互事件资产", "#E91E63")]
    public DeliverBoolSO 提示框显示事件SO;
    public DeliverTransformSO 提示框位置SO;

    private Vector3 _上一帧平台位置;

    private void Awake()
    {
        _移动器 = GetComponent<WorldMover>();
    }

    private void Start()
    {
        // 初始化记录位置
        _上一帧平台位置 = transform.position;
    }

    private void Update()
    {
        // 1. 处理交互按键
        bool 具备交互条件 = _玩家是否在平台上 && _玩家变换组件 != null;
        if (具备交互条件)
        {
            bool 玩家按下Q键 = Input.GetKeyDown(KeyCode.Q);
            if (玩家按下Q键)
            {
                OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
            }
        }
    }

    /// <summary>
    /// 核心逻辑：使用 LateUpdate 解决滞后，并手动同步位移规避缩放问题
    /// </summary>
    private void LateUpdate()
    {
        Vector3 当前时刻平台位置 = transform.position;

        // 计算本帧平台在世界空间产生的位移差
        Vector3 平台位移矢量 = 当前时刻平台位置 - _上一帧平台位置;

        // 提取判断条件
        bool 平台产生了实质移动 = 平台位移矢量.sqrMagnitude > 最小移动阈值;
        bool 需要带动玩家移动 = 平台产生了实质移动 && _玩家是否在平台上 && _玩家变换组件 != null;

        if (需要带动玩家移动)
        {
            // 【关键改进】：直接修改世界坐标，不建立父子关系，不继承缩放
            _玩家变换组件.position += 平台位移矢量;
        }

        // 记录本帧结束时的位置，供下一帧对比
        _上一帧平台位置 = 当前时刻平台位置;
    }

    private void OnTriggerEnter2D(Collider2D 碰撞体)
    {
        bool 碰撞对象是玩家 = 碰撞体.CompareTag("Player");
        if (碰撞对象是玩家)
        {
            _玩家是否在平台上 = true;
            _玩家变换组件 = 碰撞体.transform;

            // 禁止使用 SetParent，解决缩放变形 Bug
            提示框显示事件SO.RaiseEvent(true);
        }
    }

    private void OnTriggerExit2D(Collider2D 碰撞体)
    {
        bool 碰撞对象是玩家 = 碰撞体.CompareTag("Player");
        if (碰撞对象是玩家)
        {
            _玩家是否在平台上 = false;
            _玩家变换组件 = null;
            提示框显示事件SO.RaiseEvent(false);
        }
    }

    public override bool OnInteract(InteractionSignal 信号)
    {
        bool 信号类型不符 = 信号.type != InteractionType.KeyPress;
        if (信号类型不符)
            return false;

        if (_当前位于点1)
        {
            _移动器.移动至(路径点2.position);
        }
        else
        {
            _移动器.移动至(路径点1.position);
        }

        _当前位于点1 = !_当前位于点1;
        return true;
    }
}
