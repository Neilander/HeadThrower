using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class PlayerController : MonoBehaviour
{
    #region 常量
    // 移动相关阈值：速度大于此值视为正向移动
    private const float 移动阈值 = 0.6f;
    // 朝向相关阈值：速度乘积大于此值视为正向行走
    private const float 正走阈值 = 0.01f;
    // 手柄摇杆阈值：摇杆输入大于此值视为有效输入
    private const float 摇杆阈值 = 0.03f;
    #endregion

    #region 公共变量
    [SerializeField] public InputControl 输入控制;
    [SerializeField] private Vector2 输入值;
    [SerializeField] private Vector2 瞄准值;

    [Header("基本参数")]
    public float 速度;
    public float 跳跃高度;
    [Header("物理")]
    public CapsuleCollider2D 胶囊碰撞器;
    public Rigidbody2D 刚体;
    public PhysicCheck 物理检测;
    [SerializeField] private bool 是否键盘;
    public int 移动方向状态;
    public RigidbodyController 头部控制器;
    [SerializeField] Transform 提示图标;
    #endregion

    private InputAction 鼠标位置动作;
    private InputAction 交互动作;
    private InputAction 跳跃动作;

    private void Awake()
    {
        输入控制 = new InputControl();
        胶囊碰撞器 = GetComponent<CapsuleCollider2D>();

        // 使用 InputSystem 获取鼠标位置和交互输入
        鼠标位置动作 = 输入控制.UI.Point;
        交互动作 = 输入控制.Player.Interact;
        跳跃动作 = 输入控制.Player.Jump;

        交互动作.Enable();
        跳跃动作.Enable();

        // 查找子物体中的头部
        foreach (Transform child in transform)
        {
            bool 是头部 = child.CompareTag("Head");
            if (是头部)
            {
                头部控制器 = child.GetComponent<RigidbodyController>();
            }
        }
    }

    void Update()
    {
        更新状态机();
        更新朝向();
    }

    void OnEnable()
    {
        InputSystem.onActionChange += 当输入设备变化;
        输入控制.Enable();
    }

    void OnDisable()
    {
        InputSystem.onActionChange -= 当输入设备变化;
        输入控制.Disable();
    }

    private void 当输入设备变化(object obj, InputActionChange 设备变化)
    {
        bool 是动作开始 = 设备变化 == InputActionChange.ActionStarted;
        if (是动作开始)
        {
            var 当前设备 = ((InputAction)obj).activeControl.device;
            bool 是键盘 = 当前设备 is Keyboard;
            bool 是鼠标 = 当前设备 is Mouse;
            bool 是手柄 = 当前设备 is XInputController;

            if (是键盘 || 是鼠标)
            {
                是否键盘 = true;
            }
            if (是手柄)
            {
                是否键盘 = false;
            }
        }
    }

    public bool 可以移动 = true;
    public float 空中停止操控的时间;

    private IEnumerator 停止空中移动()
    {
        可以移动 = false;
        yield return new WaitForSeconds(空中停止操控的时间);
        可以移动 = true;
    }

    private void FixedUpdate()
    {
        if (可以移动)
        {
            刚体.velocity = new Vector2(输入值.x * 速度 * Time.deltaTime, 刚体.velocity.y);
        }
    }

    #region 状态机

    public enum 投掷状态
    {
        头上有头,
        没头,
        别人的头,
        投掷动画中,
        捡起动画中
    }

    [Header("当前状态")]
    public 投掷状态 当前状态 = 投掷状态.头上有头;

    public void 开始状态(投掷状态 新状态)
    {
        结束状态(当前状态);
        switch (新状态)
        {
            case 投掷状态.头上有头:
                break;
            case 投掷状态.没头:
                break;
            case 投掷状态.别人的头:
                break;
            case 投掷状态.捡起动画中:
                bool 有触发器 = 当前触发器 != null;
                if (有触发器)
                {
                    当前触发器.GetComponent<PickUpHead>().OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
                }
                break;
            case 投掷状态.投掷动画中:
                break;
        }
        当前状态 = 新状态;
    }

    private void 结束状态(投掷状态 上个状态)
    {
        switch (上个状态)
        {
            case 投掷状态.头上有头:
                break;
            case 投掷状态.没头:
                // 捡起头后隐藏提示图标
                bool 有提示图标 = 提示图标 != null;
                if (有提示图标)
                {
                    SpriteRenderer 渲染器 = 提示图标.GetComponent<SpriteRenderer>();
                    bool 有渲染器 = 渲染器 != null;
                    if (有渲染器) 渲染器.enabled = false;
                }
                break;
            case 投掷状态.别人的头:
                break;
            case 投掷状态.捡起动画中:
                break;
            case 投掷状态.投掷动画中:
                添加头部刚体();
                分离头部();
                // 执行投掷
                Vector2 瞄准方向 = new Vector2(获取瞄准向量().x, 0).normalized;
                PickUpHead 捡起头部 = 头部控制器.transform.GetComponent<PickUpHead>();
                捡起头部.isPickUp = false;
                头部控制器.Throw(瞄准方向);
                头部控制器 = null;
                当前触发器 = null;
                // 停止玩家移动一段时间
                StartCoroutine(停止空中移动());
                // 施加反作用力
                施加反作用力(瞄准方向);
                break;
        }
    }

    private void 更新状态机()
    {
        switch (当前状态)
        {
            case 投掷状态.头上有头:
                // 鼠标右键投掷
                bool 按下投掷键 = 输入控制.Player.Interact.triggered;
                if (按下投掷键)
                {
                    开始状态(投掷状态.投掷动画中);
                }
                break;
            case 投掷状态.没头:
                // 按下交互键且范围内有头时捡起
                bool 按下交互键 = 交互动作.triggered;
                if (按下交互键)
                {
                    bool 可以捡起头部 = 可以捡起();
                    if (可以捡起头部)
                    {
                        开始状态(投掷状态.捡起动画中);
                    }
                }
                break;
            case 投掷状态.别人的头:
                // 鼠标右键投掷别人的头
                bool 按下投掷键2 = 输入控制.Player.Interact.triggered;
                if (按下投掷键2)
                {
                    开始状态(投掷状态.投掷动画中);
                }
                break;
            case 投掷状态.捡起动画中:
                // 检查是否是自己的头
                bool 是自己的头 = 检查是我的头();
                if (是自己的头) 
                    开始状态(投掷状态.头上有头);
                else 
                    开始状态(投掷状态.别人的头);
                break;
            case 投掷状态.投掷动画中:
                开始状态(投掷状态.没头);
                break;
        }
    }

    #endregion

    #region 移动

    public void 跳跃()
    {
        bool 在地面上 = 物理检测.isGround;
        if (在地面上)
            刚体.AddForce(transform.up * 跳跃高度, ForceMode2D.Impulse);
    }

    public void 处理移动输入(InputAction.CallbackContext 上下文)
    {
        输入值 = 上下文.ReadValue<Vector2>();
    }

    #endregion

    #region 人物朝向

    public void 处理瞄准输入(InputAction.CallbackContext 上下文)
    {
        瞄准值 = 上下文.ReadValue<Vector2>();
    }

    /// <summary>
    /// 计算鼠标相对于人物的的水平方向
    /// </summary>
    private int 获取鼠标朝向()
    {
        Vector2 鼠标屏幕坐标 = 鼠标位置动作.ReadValue<Vector2>();
        Vector3 鼠标世界坐标 = Camera.main.ScreenToWorldPoint(new Vector3(鼠标屏幕坐标.x, 鼠标屏幕坐标.y, 0));
        float 相对位置X = 鼠标世界坐标.x - transform.position.x;
        bool 在右侧 = 相对位置X > 0;
        return 在右侧 ? 1 : -1;
    }

    /// <summary>
    /// 获取瞄准方向向量
    /// </summary>
    private Vector2 获取瞄准向量()
    {
        if (是否键盘)
        {
            Vector2 鼠标屏幕坐标 = 鼠标位置动作.ReadValue<Vector2>();
            Vector3 鼠标世界坐标 = Camera.main.ScreenToWorldPoint(new Vector3(鼠标屏幕坐标.x, 鼠标屏幕坐标.y, 0));
            return 鼠标世界坐标 - transform.position;
        }
        else
        {
            return 瞄准值;
        }
    }

    /// <summary>
    /// 更新人物朝向
    /// </summary>
    private void 更新朝向()
    {
        // 判断当前状态是否有头
        bool 头上有头 = 当前状态 == 投掷状态.头上有头;
        bool 有别人的头 = 当前状态 == 投掷状态.别人的头;
        bool 投掷动画中 = 当前状态 == 投掷状态.投掷动画中;
        bool 有头状态 = 头上有头 || 有别人的头 || 投掷动画中;

        if (有头状态)
        {
            // 使用键盘时按鼠标方向，使用手柄时按摇杆方向
            int 朝向;
            if (是否键盘)
            {
                朝向 = 获取鼠标朝向();
            }
            else
            {
                bool 摇杆向右 = 瞄准值.x > 摇杆阈值;
                bool 摇杆向左 = 瞄准值.x < -摇杆阈值;
                if (摇杆向右) 朝向 = 1;
                else if (摇杆向左) 朝向 = -1;
                else 朝向 = (int)transform.localScale.x;
            }
            transform.localScale = new Vector3(朝向, 1, 1);

            // 判断正走/倒走/静止
            float 速度绝对值 = Mathf.Abs(刚体.velocity.x);
            bool 有移动速度 = 速度绝对值 > 移动阈值;
            if (有移动速度)
            {
                float 速度乘积 = 刚体.velocity.x * 朝向;
                bool 正向移动 = 速度乘积 > 正走阈值;
                移动方向状态 = 正向移动 ? 1 : -1;
            }
            else
            {
                移动方向状态 = 0;
            }
        }
        else
        {
            // 无头时根据移动速度调整朝向
            float 当前速度X = 刚体.velocity.x;
            bool 正在向右移动 = 当前速度X > 移动阈值;
            bool 正在向左移动 = 当前速度X < -移动阈值;

            if (正在向右移动)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (正在向左移动)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    #endregion

    #region 分离头部

    /// <summary>
    /// 分离所有头部子物体
    /// </summary>
    private void 分离头部()
    {
        var 所有子物体 = GetComponentsInChildren<Transform>();
        foreach (var child in 所有子物体)
        {
            bool 不是自己 = child != transform;
            bool 是头部 = child.CompareTag("Head");
            bool 符合条件 = 不是自己 && 是头部;
            if (符合条件)
            {
                child.SetParent(null);
            }
        }
    }

    /// <summary>
    /// 为分离的头部添加刚体
    /// </summary>
    private void 添加头部刚体()
    {
        foreach (Transform child in transform)
        {
            bool 是头部 = child.CompareTag("Head");
            if (是头部)
            {
                头部控制器 = child.GetComponent<RigidbodyController>();
                bool 有控制器 = 头部控制器 != null;
                if (有控制器)
                {
                    头部控制器.AddRGbody();
                }
                return;
            }
        }
    }

    [SerializeField] int 反作用力;

    /// <summary>
    /// 施加反作用力
    /// </summary>
    private void 施加反作用力(Vector2 方向)
    {
        刚体.velocity = Vector2.zero;
        刚体.AddForce(-方向.normalized * 反作用力, ForceMode2D.Impulse);
    }

    #endregion

    #region 捡起头部

    [SerializeField] public Collider2D 当前触发器;

    /// <summary>
    /// 检测是否可以捡起头部
    /// </summary>
    public bool 可以捡起()
    {
        bool 有触发器 = 当前触发器 != null;
        bool 是头部触发器 = 有触发器 && 当前触发器.CompareTag("Head");
        return 是头部触发器;
    }

    /// <summary>
    /// 检查是否是玩家自己的头
    /// </summary>
    public bool 检查是我的头()
    {
        bool 有触发器 = 当前触发器 != null;
        if (!有触发器) return false;
        
        RigidbodyController 控制器 = 当前触发器.GetComponentInParent<RigidbodyController>();
        bool 有控制器 = 控制器 != null;
        bool 是自己的头 = 有控制器 && 控制器.isMyHead;
        return 是自己的头;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        当前触发器 = other;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        bool 是当前触发器 = other == 当前触发器;
        if (是当前触发器)
        {
            当前触发器 = null;
        }
    }

    #endregion
}
