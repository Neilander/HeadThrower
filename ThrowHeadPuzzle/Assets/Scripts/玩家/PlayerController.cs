using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using System.Linq; // 添加LINQ支持



public class PlayerController : MonoBehaviour
{
    #region 公共变量
    [SerializeField] public InputControl inputControl;//输入
    private InputAction mousePositionAction;
    private InputAction mouseAction;
    private InputAction eKeyAction;
    [SerializeField] private Vector2 value_inputControl;
    [SerializeField] private Vector2 aimValue_inputControl;

    [Header("基本参数")]
    public float 速度;
    public float 跳跃高度;
    [Header("物理")]
    public CapsuleCollider2D capsuleCollider;
    public Rigidbody2D rgbody;
    public PhysicCheck physicCheck;
    [SerializeField] private bool isKeyboard;
    public int isMoveForward;//表示人物是正走/倒走状态，1代表正走，-1代表倒走
    public RigidbodyController rbController;
    [SerializeField] Transform ESign;
    #endregion

    private void Awake()
    {
        inputControl = new InputControl();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        // 创建一个新的输入动作来获取鼠标位置
        mousePositionAction = new InputAction(binding: "<Mouse>/position");
        mouseAction = new InputAction(
            name: "LeftClick",
            type: InputActionType.Button,
            binding: "<Mouse>/leftButton"
        //interactions: "press(duration=0.1)"  // 添加点击持续阈值
        );
        eKeyAction = new InputAction("EKeyPress", binding: "<Keyboard>/e");
        eKeyAction.Enable();
        mouseAction.Enable();
        mousePositionAction.Enable();
        //inputControl.Player.Jump.started += Jump();//旧的方法
        foreach (Transform child in transform)
        {
            // 检查子物体的tag是否为"Head"
            if (child.CompareTag("Head"))
            {
                // 获取子物体上的RigidbodyController组件
                rbController = child.GetComponent<RigidbodyController>();
            }
        }
    }
    void Update()
    {
<<<<<<< HEAD:ThrowHeadPuzzle/Assets/Scripts/PlayerController.cs
        //ValueMove();
=======
        //状态机更新
>>>>>>> Lxy_add_GPT:ThrowHeadPuzzle/Assets/Scripts/玩家/PlayerController.cs
        UpdateState();

        //当人没有头时不改变朝向
        if (curstate == ThrowState.HeadOnBody || curstate == ThrowState.OtherHead || curstate == ThrowState.ThrowAnimation)
            CheckAndChangeDirection();
        else
        {
            int faceDirection;
            if (rgbody.velocity.x > 0.6)
            {
                faceDirection = 1;
                transform.localScale = new Vector3(faceDirection, 1, 1);
            }

            if (rgbody.velocity.x < -0.6)
            {
                faceDirection = -1;
                transform.localScale = new Vector3(faceDirection, 1, 1);
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
        if (deviceChange == InputActionChange.ActionStarted)
        {
            //Debug.Log(((InputAction)obj).activeControl.device);
            var cur_device = ((InputAction)obj).activeControl.device;
            //isKeyboard = false;
            switch (cur_device)
            {
                case Keyboard:
                    isKeyboard = true;
                    break;
                case Mouse:
                    isKeyboard = true;
                    break;
                case XInputController:
                    isKeyboard = false;
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
        if (canMove)//value_inputControl.magnitude != 0 && 
        {
            rgbody.velocity = new Vector2(value_inputControl.x * 速度 * Time.deltaTime, rgbody.velocity.y);
        }
    }

    #region 状态机

    public enum ThrowState
    {
        HeadOnBody,//移动 瞄准 默认状态
        NoHead,//移动
        OtherHead,//移动 瞄准
        ThrowAnimation,//该状态结束后应该进入nohead状态
        PickupAnimation
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
                _currentTrigger.GetComponent<PickUpHead>().OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
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
                SpriteRenderer signRenderer = ESign.GetComponent<SpriteRenderer>();
                signRenderer.enabled = false;  //取消激活 SpriteRenderer
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
                Vector2 aim = new Vector2(GetVectorAim().x, 0).normalized;
                // 获取物体上的PickUpHead组件
                PickUpHead _picUpHead = rbController.transform.GetComponent<PickUpHead>();
                _picUpHead.isPickUp = false;
                rbController.Throw(aim);
                rbController = null;
                _currentTrigger = null;
                //停止输入
                StartCoroutine(StopMoveInAir());
                //反作用力函数
                CounterForce(aim);
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
                if (Input.GetMouseButtonDown(1))
                {
                    StartState(ThrowState.ThrowAnimation);
                }
                break;

            case ThrowState.NoHead:
                // if (CanPickUp())
                // {
                //     //Debug.Log("可以拾取这个Head！");
                //     SpriteRenderer signRenderer = ESign.GetComponent<SpriteRenderer>();
                //     signRenderer.enabled = true;  //激活 SpriteRenderer
                // }
                // else
                // {
                //     SpriteRenderer signRenderer = ESign.GetComponent<SpriteRenderer>();
                //     signRenderer.enabled = false;  //取消激活 SpriteRenderer
                // }
                
                //如果按下E，并且范围里有可以捡起的头，就捡起最近的

                if (eKeyAction.triggered)//按下E或者点击 || mouseAction.triggered
                {
                    //满足 检测到可拾取的头 collider触发器
                    if (CanPickUp())
                    {
                        StartState(ThrowState.PickupAnimation);
                    }
                }
                break;
            case ThrowState.OtherHead:
                //如果按下E，就投掷
                //if (eKeyAction.triggered)//按下E或者点击 || mouseAction.triggered
                if (Input.GetMouseButtonDown(1))
                {
                    StartState(ThrowState.ThrowAnimation);
                }
                break;
            case ThrowState.PickupAnimation:
                //结束拾取，检查是否是自己的头
                if (CheckMyHead()) StartState(ThrowState.HeadOnBody);
                else StartState(ThrowState.OtherHead);
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
        if (physicCheck.isGround)
            rgbody.AddForce(transform.up * 跳跃高度, ForceMode2D.Impulse);
    }

    public void ValueMove(InputAction.CallbackContext callbackContext)
    {
        value_inputControl = callbackContext.ReadValue<Vector2>();
    }
    #endregion

    #region 人物朝向
    /// <summary>
    /// 在使用手柄时更新期望的人物朝向
    /// </summary>
    /// <param name="callbackContext"></param>
    public void AimDirection(InputAction.CallbackContext callbackContext)
    {
        aimValue_inputControl = callbackContext.ReadValue<Vector2>();
    }

    /// <summary>
    /// 在输入设备为键盘时，用于计算鼠标相对于摄像机的坐标x位置减去当前对象的x位置，并将Scale正确设置
    /// </summary>
    public int FacetoMouse()
    {
        // 获取鼠标位置
        Vector2 mouseScreenPos = mousePositionAction.ReadValue<Vector2>();
        // 将鼠标屏幕坐标转换为世界坐标，2D游戏中z轴设为0
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        int direaction = (mouseWorldPos.x - transform.position.x) > 0 ? 1 : -1;
        return direaction;
    }

    /// <summary>
    /// 获取并返回人物瞄准的向量
    /// </summary>
    /// <returns></returns>
    public Vector2 GetVectorAim()
    {
        if (isKeyboard)
        {
            // 获取鼠标位置
            Vector2 mouseScreenPos = mousePositionAction.ReadValue<Vector2>();
            // 将鼠标屏幕坐标转换为世界坐标，2D游戏中z轴设为0
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
            Vector2 direaction = mouseWorldPos - transform.position;
            return direaction;
        }
        else
        {
            return aimValue_inputControl;
        }
    }

    /// <summary>
    /// 调整人物朝向
    /// </summary>
    public void CheckAndChangeDirection()
    {
        int faceDirection = (int)transform.localScale.x;
        //Debug.Log(rgbody.velocity.x * faceDirection);
        if (isKeyboard)//输入不为手柄时
        {
            faceDirection = FacetoMouse() > 0 ? 1 : -1;
            transform.localScale = new Vector3(faceDirection, 1, 1);
            //判断正走/倒走
            if (rgbody.velocity.x * faceDirection > 0.01)//正走
            {
                //触发正走的animator
                isMoveForward = 1;
                //Debug.Log("正走");
            }
            else if (rgbody.velocity.x * faceDirection < -0.01)
            {
                //触发倒走的animator
                isMoveForward = -1;
                //Debug.Log("倒走");
            }
            else if (rgbody.velocity.x * faceDirection == 0)
            {
                isMoveForward = 0;
                //Debug.Log("静止");
            }
        }
        else
        {
            if (aimValue_inputControl.x > 0.03)
                faceDirection = 1;
            if (aimValue_inputControl.x < -0.03)
                faceDirection = -1;
            transform.localScale = new Vector3(faceDirection, 1, 1);
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
        var allChildren = GetComponentsInChildren<Transform>();
        foreach (var child in allChildren)
        {
            if (child != transform && child.CompareTag("Head"))
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
            if (child.CompareTag("Head"))
            {
                // 获取子物体上的RigidbodyController组件
                rbController = child.GetComponent<RigidbodyController>();

                if (rbController != null)
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

    [SerializeField] int 反作用力;

    /// <summary>
    /// 在扔出脑袋时对自己施加反作用力
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void CounterForce(Vector2 aim)
    {
        rgbody.velocity = new Vector2(0, 0);
        rgbody.AddForce(-aim.normalized * 反作用力, ForceMode2D.Impulse);
    }

    #endregion

    #region 捡起头部
    [SerializeField] public Collider2D _currentTrigger; // 当前所在的触发器

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
        RigidbodyController controller = _currentTrigger.GetComponentInParent<RigidbodyController>();

        return controller != null && controller.isMyHead;
    }

    // 触发器进入时记录
    private void OnTriggerStay2D(Collider2D other)
    {
        _currentTrigger = other;
    }

    // 触发器退出时清空记录
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == _currentTrigger)
        {
            _currentTrigger = null;
        }
    }

    // [SerializeField] Vector3 头偏移量;
    // public void AttachHeads()
    // {
    //     RigidbodyController controller = _currentTrigger.GetComponentInParent<RigidbodyController>();
    //     //移除刚体
    //     controller.DeAddRGbody();
    //     //与身体组合，变成子对象
    //     controller.transform.SetParent(transform);
    //     //设置正确的位置
    //     controller.transform.localPosition = 头偏移量;
    //     controller.transform.eulerAngles = new Vector3(0, 0, 0);
    //     controller.transform.localScale = new Vector3(1, 1, 1);
    // }
    #endregion
}