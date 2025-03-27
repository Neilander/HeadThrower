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
    public InputControl inputControl;//输入
    private InputAction mousePositionAction;
    private InputAction eKeyAction;
    [SerializeField] private Vector2 value_inputControl;
    [SerializeField] private Vector2 aimValue_inputControl;

    [Header("基本参数")]
    public float 速度;
    public float 跳跃高度;
    [Header("物理")]
    public CapsuleCollider2D capsuleCollider;
    public Rigidbody2D rgbody;
    [SerializeField] private bool isKeyboard;
    public int isMoveForward;//表示人物是正走/倒走状态，1代表正走，-1代表倒走


    private void Awake()
    {
        inputControl = new InputControl();
        inputControl.Enable();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        // 创建一个新的输入动作来获取鼠标位置
        mousePositionAction = new InputAction(binding: "<Mouse>/position");
        eKeyAction = new InputAction("EKeyPress", binding: "<Keyboard>/e");
        eKeyAction.Enable();
        mousePositionAction.Enable();
        //inputControl.Player.Jump.started += Jump();//旧的方法
    }

    void OnEnable()
    {
        InputSystem.onActionChange += OnInputDiviceChange;
    }

    private void OnInputDiviceChange(object obj, InputActionChange deviceChange)
    {
        if (deviceChange == InputActionChange.ActionStarted)
        {
            //Debug.Log(((InputAction)obj).activeControl.device);
            var cur_device = ((InputAction)obj).activeControl.device;
            //isKeyboard = false;
            switch (cur_device.device)
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

    private void FixedUpdate()
    {
        if (value_inputControl.magnitude != 0)
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
                Debug.Log("头.已经被投出();");

                break;
            case ThrowState.OtherHead:
                //

                break;
            case ThrowState.PickupAnimation:
                //

                break;
            case ThrowState.ThrowAnimation:
                //HeadOnBody=>ThrowAnimation
                //

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
                Debug.Log("头.投出();");
                break;

            case ThrowState.NoHead:
                //
                break;
            case ThrowState.OtherHead:
                //

                break;
            case ThrowState.PickupAnimation:
                //

                break;
            case ThrowState.ThrowAnimation:
                //

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
                //如果按下E，就投掷
                if (eKeyAction.triggered)//按下E
                {
                    StartState(ThrowState.ThrowAnimation);
                }
                break;

            case ThrowState.NoHead:
                //如果按下E，并且范围里有可以捡起的头，就捡起最近的
                if (eKeyAction.triggered)//按下E
                {
                    //PickUp(最近的头);
                    StartState(ThrowState.PickupAnimation);
                    //这里的pickup行为可以在这里执行，也可以在pickupAnimation的开始执行
                }
                break;
            case ThrowState.OtherHead:

                break;
            case ThrowState.PickupAnimation:

                break;
            case ThrowState.ThrowAnimation:

                break;
        }
    }

    #endregion

    void Update()
    {

        UpdateState();

        CheckAndChangeDirection();
    }

    public void Jump()
    {
        rgbody.AddForce(transform.up * 跳跃高度, ForceMode2D.Impulse);

    }

    public void CanMove(InputAction.CallbackContext callbackContext)
    {
        value_inputControl = callbackContext.ReadValue<Vector2>();
    }

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
        Debug.Log(rgbody.velocity.x * faceDirection);
        if (isKeyboard)//输入不为手柄时
        {
            faceDirection = FacetoMouse() > 0 ? 1 : -1;
            transform.localScale = new Vector3(faceDirection, 1, 1);
            //判断正走/倒走
            if (rgbody.velocity.x * faceDirection > 0.01)//正走
            {
                //触发正走的animator
                isMoveForward = 1;
                Debug.Log("正走");
            }
            else if (rgbody.velocity.x * faceDirection < -0.01)
            {
                //触发倒走的animator
                isMoveForward = -1;
                Debug.Log("倒走");
            }
            else if (rgbody.velocity.x * faceDirection == 0)
            {
                isMoveForward = 0;
                Debug.Log("静止");
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


    // 方法1：简洁分离所有 "Head" Tag 的子物体
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
}
