using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // 添加LINQ支持
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class PlayerController : MonoBehaviour
{
    // 重构说明：
    // 1) 将魔法数字提取为语义化常量。
    // 2) 将所有 if 控制流条件提取为上一行 bool 变量。
    // 3) 优先将可安全重命名的英文变量改为中文语义命名。
    private const int 右键鼠标按钮索引 = 1;
    private const int 正向朝向值 = 1;
    private const int 反向朝向值 = -1;
    private const int 静止朝向值 = 0;
    private const float 朝向切换速度阈值 = 0.6f;
    private const float 行走判定速度阈值 = 0.01f;
    private const float 手柄朝向输入阈值 = 0.03f;
    private const float 屏幕坐标深度值 = 0f;
    private const float 缩放轴默认值 = 1f;

    #region 公共变量
    [SerializeField]
    public InputControl inputControl; //输入
    private InputAction 鼠标位置输入动作;
    private InputAction 鼠标左键输入动作;
    private InputAction E键输入动作;

    [SerializeField]
    private Vector2 移动输入值;

    [SerializeField]
    private Vector2 瞄准输入值;

    [Header("基本参数")]
    public float 速度;
    public float 跳跃高度;

    [Header("物理")]
    public CapsuleCollider2D capsuleCollider;
    public Rigidbody2D rgbody;
    public PhysicCheck physicCheck;
    public GameObject 默认头部;

    [SerializeField]
    private bool 当前为键鼠输入;
    public int isMoveForward; //表示人物是正走/倒走状态，1代表正走，-1代表倒走
    public RigidbodyController rbController;

    [SerializeField]
    Transform 交互提示标识;
    #endregion

    private void Awake()
    {
        inputControl = new InputControl();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        // 创建一个新的输入动作来获取鼠标位置
        鼠标位置输入动作 = new InputAction(binding: "<Mouse>/position");
        鼠标左键输入动作 = new InputAction(
            name: "LeftClick",
            type: InputActionType.Button,
            binding: "<Mouse>/leftButton"
        //interactions: "press(duration=0.1)"  // 添加点击持续阈值
        );
        E键输入动作 = new InputAction("EKeyPress", binding: "<Keyboard>/e");
        E键输入动作.Enable();
        鼠标左键输入动作.Enable();
        鼠标位置输入动作.Enable();
        //inputControl.Player.Jump.started += Jump();//旧的方法
        foreach (Transform child in transform)
        {
            // 检查子物体的tag是否为"Head"
            bool 子物体是头部标签 = child.CompareTag("Head");
            if (子物体是头部标签)
            {
                // 获取子物体上的RigidbodyController组件
                rbController = child.GetComponent<RigidbodyController>();
            }
        }
    }

    void Update()
    {
        //状态机更新
        UpdateState();

        //当人没有头时不改变朝向
        bool 当前状态允许根据输入更新朝向 =
            curstate == ThrowState.HeadOnBody
            || curstate == ThrowState.OtherHead
            || curstate == ThrowState.ThrowAnimation;
        if (当前状态允许根据输入更新朝向)
            CheckAndChangeDirection();
        else
        {
            int 人物朝向值;
            bool 速度超过正向朝向阈值 = rgbody.velocity.x > 朝向切换速度阈值;
            if (速度超过正向朝向阈值)
            {
                人物朝向值 = 正向朝向值;
                transform.localScale = new Vector3(人物朝向值, 缩放轴默认值, 缩放轴默认值);
            }

            bool 速度低于反向朝向阈值 = rgbody.velocity.x < -朝向切换速度阈值;
            if (速度低于反向朝向阈值)
            {
                人物朝向值 = 反向朝向值;
                transform.localScale = new Vector3(人物朝向值, 缩放轴默认值, 缩放轴默认值);
            }
        }
    }

    void OnEnable()
    {
        InputSystem.onActionChange += OnInputDiviceChange;
        inputControl.Enable();
    }

    private void OnInputDiviceChange(object obj, InputActionChange deviceChange)
    {
        bool 输入动作已开始 = deviceChange == InputActionChange.ActionStarted;
        if (输入动作已开始)
        {
            //Debug.Log(((InputAction)obj).activeControl.device);
            var 当前输入设备 = ((InputAction)obj).activeControl.device;
            //isKeyboard = false;
            switch (当前输入设备)
            {
                case Keyboard:
                    当前为键鼠输入 = true;
                    break;
                case Mouse:
                    当前为键鼠输入 = true;
                    break;
                case XInputController:
                    当前为键鼠输入 = false;
                    break;
            }
        }
    }

    public bool canMove = true;
    public float 空中停止操控的时间;

    /// <summary>
    /// 用于在扔出脑袋后停止一段时间的输入
    /// </summary>
    /// <returns></returns>
    private IEnumerator StopMoveInAir()
    {
        // 开始协程后将 canMove 设置为 false
        canMove = false;

        // 等待指定的时间
        yield return new WaitForSeconds(空中停止操控的时间);

        // 时间到后将 canMove 设置为 true
        canMove = true;

        // 协程执行完毕，自动停止
    }

    private void FixedUpdate()
    {
        bool 当前允许移动 = canMove;
        if (当前允许移动) //value_inputControl.magnitude != 0 &&
        {
            rgbody.velocity = new Vector2(
                移动输入值.x * 速度 * Time.deltaTime,
                rgbody.velocity.y
            );
        }
    }

    #region 状态机

    public enum ThrowState
    {
        HeadOnBody, //移动 瞄准 默认状态
        NoHead, //移动
        OtherHead, //移动 瞄准
        ThrowAnimation, //该状态结束后应该进入nohead状态
        PickupAnimation,
    }

    [Header("目前状态")]
    public ThrowState curstate = ThrowState.HeadOnBody;

    //外部可以调用的切换状态方法
    public void StartState(ThrowState newState)
    {
        EndState(curstate);
        switch (newState)
        {
            //执行不同状态的开始行为
            case ThrowState.HeadOnBody:
                //

                break;

            case ThrowState.NoHead:
                //ThrowAnimation=>NoHead
                //Debug.Log("头.已经被投出();");

                break;
            case ThrowState.OtherHead:
                //

                break;
            case ThrowState.PickupAnimation:
                //把头拼上
                _currentTrigger
                    .GetComponent<PickUpHead>()
                    .OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
                break;
            case ThrowState.ThrowAnimation:
                //HeadOnBody=>ThrowAnimation
                //播放瞄准动画

                break;
        }
        curstate = newState;
    }

    //内部调用的结束状态方法
    private void EndState(ThrowState lastState)
    {
        switch (lastState)
        {
            //执行不同状态的结束行为
            //执行不同状态的开始行为
            case ThrowState.HeadOnBody:
                //HeadOnBody=>ThrowAnimation
                //TODO: 播放投掷动画
                //使用  head.投掷(); 找到目前身上的头，调用这个头组件中的 投掷()
                //Debug.Log("头.投出();");
                break;

            case ThrowState.NoHead:
                //PickUp(最近的头);
                //捡起头后取消显示提示
                SpriteRenderer 提示图标渲染器 = 交互提示标识.GetComponent<SpriteRenderer>();
                提示图标渲染器.enabled = false; //取消激活 SpriteRenderer
                break;
            case ThrowState.OtherHead:
                //

                break;
            case ThrowState.PickupAnimation:

                break;
            case ThrowState.ThrowAnimation:
                // 分离头 增加刚体组件
                HeadAddrgbody();
                DetachHeads();
                //抛出
                Vector2 瞄准方向 = new Vector2(GetVectorAim().x, 屏幕坐标深度值).normalized;
                // 获取物体上的PickUpHead组件
                PickUpHead 头部拾取组件 = rbController.transform.GetComponent<PickUpHead>();
                头部拾取组件.isPickUp = false;
                rbController.Throw(瞄准方向);
                rbController = null;
                _currentTrigger = null;
                //停止输入
                StartCoroutine(StopMoveInAir());
                //反作用力函数
                CounterForce(瞄准方向);
                break;
        }
    }

    //内部调用的状态Update
    private void UpdateState()
    {
        switch (curstate)
        {
            //执行不同状态的Update，如：
            case ThrowState.HeadOnBody:
                //如果鼠标右键，就投掷
                //if (eKeyAction.triggered)//按下E或者点击 || mouseAction.triggered
                bool 右键按下准备投掷 = Input.GetMouseButtonDown(右键鼠标按钮索引);
                if (右键按下准备投掷)
                {
                    StartState(ThrowState.ThrowAnimation);
                }
                break;

            case ThrowState.NoHead:
                //如果按下E，并且范围里有可以捡起的头，就捡起最近的
                bool E键被触发 = E键输入动作.triggered;
                if (E键被触发) //按下E或者点击 || mouseAction.triggered
                {
                    //满足 检测到可拾取的头 collider触发器
                    bool 当前可拾取头部 = CanPickUp();
                    if (当前可拾取头部)
                    {
                        StartState(ThrowState.PickupAnimation);
                    }
                }
                break;
            case ThrowState.OtherHead:
                //如果按下E，就投掷
                //if (eKeyAction.triggered)//按下E或者点击 || mouseAction.triggered
                bool 右键按下投掷其他头部 = Input.GetMouseButtonDown(右键鼠标按钮索引);
                if (右键按下投掷其他头部)
                {
                    StartState(ThrowState.ThrowAnimation);
                }
                break;
            case ThrowState.PickupAnimation:
                //结束拾取，检查是否是自己的头
                bool 拾取到的是自己的头 = CheckMyHead();
                if (拾取到的是自己的头)
                    StartState(ThrowState.HeadOnBody);
                else
                    StartState(ThrowState.OtherHead);
                break;
            case ThrowState.ThrowAnimation:
                //进入nohead状态
                StartState(ThrowState.NoHead);
                break;
        }
    }

    #endregion

    #region 移动
    public void Jump()
    {
        bool 当前处于地面 = physicCheck.isGround;
        if (当前处于地面)
            rgbody.AddForce(transform.up * 跳跃高度, ForceMode2D.Impulse);
    }

    public void ValueMove(InputAction.CallbackContext callbackContext)
    {
        移动输入值 = callbackContext.ReadValue<Vector2>();
    }
    #endregion

    #region 人物朝向
    /// <summary>
    /// 在使用手柄时更新期望的人物朝向
    /// </summary>
    /// <param name="callbackContext"></param>
    public void AimDirection(InputAction.CallbackContext callbackContext)
    {
        瞄准输入值 = callbackContext.ReadValue<Vector2>();
    }

    /// <summary>
    /// 在输入设备为键盘时，用于计算鼠标相对于摄像机的坐标x位置减去当前对象的x位置，并将Scale正确设置
    /// </summary>
    public int FacetoMouse()
    {
        // 获取鼠标位置
        Vector2 鼠标屏幕坐标 = 鼠标位置输入动作.ReadValue<Vector2>();
        // 将鼠标屏幕坐标转换为世界坐标，2D游戏中z轴设为0
        Vector3 鼠标世界坐标 = Camera.main.ScreenToWorldPoint(
            new Vector3(鼠标屏幕坐标.x, 鼠标屏幕坐标.y, 屏幕坐标深度值)
        );
        bool 鼠标位于角色右侧 = (鼠标世界坐标.x - transform.position.x) > 屏幕坐标深度值;
        int 人物朝向值 = 鼠标位于角色右侧 ? 正向朝向值 : 反向朝向值;
        return 人物朝向值;
    }

    /// <summary>
    /// 获取并返回人物瞄准的向量
    /// </summary>
    /// <returns></returns>
    public Vector2 GetVectorAim()
    {
        bool 当前输入设备为键鼠 = 当前为键鼠输入;
        if (当前输入设备为键鼠)
        {
            // 获取鼠标位置
            Vector2 鼠标屏幕坐标 = 鼠标位置输入动作.ReadValue<Vector2>();
            // 将鼠标屏幕坐标转换为世界坐标，2D游戏中z轴设为0
            Vector3 鼠标世界坐标 = Camera.main.ScreenToWorldPoint(
                new Vector3(鼠标屏幕坐标.x, 鼠标屏幕坐标.y, 屏幕坐标深度值)
            );
            Vector2 瞄准方向向量 = 鼠标世界坐标 - transform.position;
            return 瞄准方向向量;
        }
        else
        {
            return 瞄准输入值;
        }
    }

    /// <summary>
    /// 调整人物朝向
    /// </summary>
    public void CheckAndChangeDirection()
    {
        int 人物朝向值 = (int)transform.localScale.x;
        //Debug.Log(rgbody.velocity.x * 人物朝向值);
        bool 当前输入设备为键鼠 = 当前为键鼠输入;
        if (当前输入设备为键鼠) //输入不为手柄时
        {
            bool 鼠标朝向为右侧 = FacetoMouse() > 屏幕坐标深度值;
            人物朝向值 = 鼠标朝向为右侧 ? 正向朝向值 : 反向朝向值;
            transform.localScale = new Vector3(人物朝向值, 缩放轴默认值, 缩放轴默认值);
            //判断正走/倒走
            bool 正向行走中 = rgbody.velocity.x * 人物朝向值 > 行走判定速度阈值;
            if (正向行走中) //正走
            {
                //触发正走的animator
                isMoveForward = 正向朝向值;
                //Debug.Log("正走");
            }
            else
            {
                bool 反向行走中 = rgbody.velocity.x * 人物朝向值 < -行走判定速度阈值;
                if (反向行走中)
                {
                    //触发倒走的animator
                    isMoveForward = 反向朝向值;
                    //Debug.Log("倒走");
                }
                else
                {
                    bool 当前静止不动 = rgbody.velocity.x * 人物朝向值 == 屏幕坐标深度值;
                    if (当前静止不动)
                    {
                        isMoveForward = 静止朝向值;
                        //Debug.Log("静止");
                    }
                }
            }
        }
        else
        {
            bool 手柄输入指向右侧 = 瞄准输入值.x > 手柄朝向输入阈值;
            if (手柄输入指向右侧)
                人物朝向值 = 正向朝向值;
            bool 手柄输入指向左侧 = 瞄准输入值.x < -手柄朝向输入阈值;
            if (手柄输入指向左侧)
                人物朝向值 = 反向朝向值;
            transform.localScale = new Vector3(人物朝向值, 缩放轴默认值, 缩放轴默认值);
        }
    }
    #endregion

    #region 分离头部
    /// <summary>
    /// 简洁分离所有 "Head" Tag 的子物体
    /// </summary>
    // 直接在原方法中写完整实现（放弃扩展方法）
    public void DetachHeads()
    {
        var 所有子物体变换组件 = GetComponentsInChildren<Transform>();
        foreach (var child in 所有子物体变换组件)
        {
            bool 子物体不是自身 = child != transform;
            bool 子物体是头部标签 = child.CompareTag("Head");
            bool 子物体可分离为头部 = 子物体不是自身 && 子物体是头部标签;
            if (子物体可分离为头部)
                child.SetParent(null);
        }
    }

    /// <summary>
    /// 为分离的头部添加rigidbody2d
    /// </summary>
    public void HeadAddrgbody()
    {
        // 遍历所有子物体
        foreach (Transform child in transform)
        {
            // 检查子物体的tag是否为"Head"
            bool 子物体是头部标签 = child.CompareTag("Head");
            if (子物体是头部标签)
            {
                // 获取子物体上的RigidbodyController组件
                rbController = child.GetComponent<RigidbodyController>();

                bool 找到了刚体控制器 = rbController != null;
                if (找到了刚体控制器)
                {
                    // 执行 AddRGbody()
                    rbController.AddRGbody();
                    Debug.Log($"在子物体 {child.name} 上成功添加了rgbody");
                }
                else
                {
                    Debug.LogWarning($"子物体 {child.name} 上没有找到RigidbodyController组件");
                }

                // 找到第一个符合条件的子物体后返回（如果只需要处理第一个）
                return;
            }
        }

        // 如果没有找到符合条件的子物体
        Debug.LogWarning($"没有找到tag为'Head'的子物体");
    }

    [SerializeField]
    int 反作用力;

    /// <summary>
    /// 在扔出脑袋时对自己施加反作用力
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void CounterForce(Vector2 aim)
    {
        rgbody.velocity = Vector2.zero;
        rgbody.AddForce(-aim.normalized * 反作用力, ForceMode2D.Impulse);
    }

    #endregion

    #region 捡起头部
    [SerializeField]
    public Collider2D _currentTrigger; // 当前所在的触发器

    /// <summary>
    /// 检测周围物体是否能拾取（也就是玩家是否位于"Head"触发器内）
    /// </summary>
    /// <returns></returns>
    public bool CanPickUp()
    {
        return _currentTrigger != null && _currentTrigger.CompareTag("Head");
    }

    public bool CheckMyHead()
    {
        // 获取触发器所在物体的 RigidbodyController 组件
        RigidbodyController 头部控制器 =
            _currentTrigger.GetComponentInParent<RigidbodyController>();

        bool 找到头部控制器 = 头部控制器 != null;
        bool 当前控制器是我的头 = 找到头部控制器 && 头部控制器.isMyHead;
        return 当前控制器是我的头;
    }

    // 触发器进入时记录
    private void OnTriggerStay2D(Collider2D other)
    {
        _currentTrigger = other;
    }

    // 触发器退出时清空记录
    private void OnTriggerExit2D(Collider2D other)
    {
        bool 退出触发器是当前记录对象 = other == _currentTrigger;
        if (退出触发器是当前记录对象)
        {
            _currentTrigger = null;
        }
    }
    #endregion
}
