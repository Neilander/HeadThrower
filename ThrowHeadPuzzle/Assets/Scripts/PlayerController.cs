using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;



public class PlayerController : MonoBehaviour
{
    public InputControl inputControl;//输入
    private InputAction mousePositionAction;
    [SerializeField] private Vector2 value_inputControl;
    [SerializeField] private Vector2 aimValue_inputControl;

    [Header("基本参数")]
    public float 速度;
    public float 跳跃高度;
    [Header("物理")]
    public CapsuleCollider2D capsuleCollider;
    public Rigidbody2D rgbody;
    [SerializeField] private bool isKeyboard;


    private void Awake()
    {
        inputControl = new InputControl();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        // 创建一个新的输入动作来获取鼠标位置
        mousePositionAction = new InputAction(binding: "<Mouse>/position");
        mousePositionAction.Enable();
        inputControl.Enable();
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
            Debug.Log(((InputAction)obj).activeControl.device);
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
        HeadOnBody,
        NoHead,
        OtherHead,
        ThrowAnimation,
        PickupAnimation
    }

    ThrowState curstate = ThrowState.HeadOnBody;

    //外部可以调用的切换状态方法
    public void StartState(ThrowState newState)
    {
        EndState(curstate);
        switch (newState)
        {
            //执行不同状态的开始行为
        }
        curstate = newState;
    }

    //内部调用的结束状态方法
    private void EndState(ThrowState lastState)
    {
        switch (lastState)
        {
            //执行不同状态的结束行为
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
                if (true)//按下E
                {
                    StartState(ThrowState.ThrowAnimation);
                }
                break;

            case ThrowState.NoHead:
                //如果按下E，并且范围里有可以捡起的头，就捡起最近的
                if (true)//按下E
                {
                    //PickUp(最近的头);
                    StartState(ThrowState.PickupAnimation);
                    //这里的pickup行为可以在这里执行，也可以在pickupAnimation的开始执行
                }
                break;
        }
    }

    #endregion

    void Update()
    {

        UpdateState();

        int faceDirection = (int)transform.localScale.x;
        if (isKeyboard)//输入不为手柄时
        {
            faceDirection = FacetoMouse() > 0 ? 1 : -1;
            transform.localScale = new Vector3(faceDirection, 1, 1);
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

    public void Jump()
    {
        rgbody.AddForce(transform.up * 跳跃高度, ForceMode2D.Impulse);

    }

    public void CanMove(InputAction.CallbackContext callbackContext)
    {
        value_inputControl = callbackContext.ReadValue<Vector2>();
    }

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
}
