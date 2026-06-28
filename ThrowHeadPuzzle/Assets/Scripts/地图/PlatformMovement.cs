using UnityEngine;

public class PlatformMovement : BaseInteraction
{
    #region 常量定义
    private const float 最小移动阈值 = 0.0001f;

    [Section("引用组件", "#FF9800")]
    private WorldMover _移动器;
    private Transform _玩家变换组件;
    private Transform _玩家根物体;
    private Rigidbody2D _玩家刚体; // 用于物理移动（问题3）

    private Quaternion _上一帧平台旋转; // 记录上一帧旋转，用于旋转同步（问题4）

    [Section("路径坐标", "#00BCD4")]
    [SerializeField]
    private Transform 路径点1;

    [SerializeField]
    private Transform 路径点2;

    [Section("状态监控", "#9C27B0")]
    [SerializeField]
    private bool _玩家是否在平台上 = false;
    private Vector3 _玩家基础世界缩放;

    [SerializeField]
    private bool _当前位于点1 = true;

    [Section("交互事件资产", "#E91E63")]
    public DeliverBoolSO 提示框显示事件SO;
    public DeliverTransformSO 提示框位置SO;

    [SerializeField]
    private Transform _备用父物体; // 离开平台后要附着的目标父物体

    private Vector3 _上一帧平台位置;
    #endregion

    private void Awake()
    {
        _移动器 = GetComponent<WorldMover>();
    }

    private void Start()
    {
        GameObject 目标父物体 = GameObject.Find("人物相关父类");
        if (目标父物体 != null)
            _备用父物体 = 目标父物体.transform;
    }

    private void Update()
    {
        // 1. 处理交互按键
        // bool 具备交互条件 = _玩家是否在平台上 && _玩家变换组件 != null;
        bool 具备交互条件 = _玩家是否在平台上 && _玩家根物体 != null;
        if (具备交互条件)
        {
            bool 玩家按下Q键 = Input.GetKeyDown(KeyCode.Q);
            if (玩家按下Q键)
            {
                OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
            }
        }
    }

    private void OnDisable()
    {
        if (_玩家根物体 != null && _玩家是否在平台上)
        {
            // 尝试将玩家移出，如果平台已销毁，直接设为 null 父级
            Transform 新父 = _备用父物体 != null ? _备用父物体 : null;
            try
            {
                _玩家根物体.SetParent(新父, worldPositionStays: true);
            }
            catch
            {
                // 若仍出错，忽略（极端情况）
            }
            _玩家是否在平台上 = false;
            _玩家根物体 = null;
            _玩家刚体 = null;
        }
    }

    private void LateUpdate()
    {
        if (!_玩家是否在平台上 || _玩家根物体 == null)
            return;

        // 获取当前朝向符号（由 PlayerController 控制，只改变符号）
        float 朝向符号 = Mathf.Sign(_玩家根物体.localScale.x);
        if (朝向符号 == 0)
            朝向符号 = 1f; // 安全兜底

        // 计算补偿缩放（保持世界缩放不变）
        Vector3 平台缩放 = transform.lossyScale;
        Vector3 目标缩放 = new Vector3(
            _玩家基础世界缩放.x / 平台缩放.x,
            _玩家基础世界缩放.y / 平台缩放.y,
            _玩家基础世界缩放.z / 平台缩放.z
        );

        // 应用朝向符号（只改变 X 的正负，保留幅度）
        目标缩放.x *= 朝向符号;

        _玩家根物体.localScale = 目标缩放;
    }

    private void OnTriggerEnter2D(Collider2D 碰撞体)
    {
        bool 碰撞对象不是玩家 = !碰撞体.CompareTag("Player");
        if (碰撞对象不是玩家)
            return; // 早退，简化代码

        Transform 玩家根 = 碰撞体.transform;
        _玩家是否在平台上 = true;
        _玩家根物体 = 玩家根;
        _玩家刚体 = 玩家根.GetComponent<Rigidbody2D>();

        _玩家基础世界缩放 = 玩家根.lossyScale;

        // 将玩家设为平台子物体（位置旋转自动跟随）
        玩家根.SetParent(transform, worldPositionStays: true);

        提示框显示事件SO.RaiseEvent(true);
    }

    private void OnTriggerExit2D(Collider2D 碰撞体)
    {
        bool 碰撞对象不是玩家 = !碰撞体.CompareTag("Player");
        if (碰撞对象不是玩家)
            return;
        if (_玩家根物体 != null)
        {
            bool 在平台仍处于激活状态 = gameObject.activeInHierarchy;
            if (在平台仍处于激活状态)
            {
                Transform 新父 = _备用父物体 != null ? _备用父物体 : null;
                _玩家根物体.SetParent(新父, worldPositionStays: true);
            }
        }

        _玩家是否在平台上 = false;
        _玩家根物体 = null;
        _玩家刚体 = null;
        提示框显示事件SO.RaiseEvent(false);
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
