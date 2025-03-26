using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public InputControl inputControl;//输入
    private InputAction mousePositionAction;
    [SerializeField] private Vector2 value_inputControl;

    [Header("基本参数")]
    public float 速度;
    public float 跳跃高度;
    [Header("物理")]
    public CapsuleCollider2D capsuleCollider;
    public Rigidbody2D rgbody;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        // 创建一个新的输入动作来获取鼠标位置
        mousePositionAction = new InputAction(binding: "<Mouse>/position");
        mousePositionAction.Enable();
        //inputControl.Player.Jump.started += Jump();//旧的方法
    }

    private void FixedUpdate()
    {
        if (value_inputControl.magnitude != 0)
        {
            rgbody.velocity = new Vector2(value_inputControl.x * 速度 * Time.deltaTime, rgbody.velocity.y);
        }

    }

    void Update()
    {
        int faceDirection = (int)transform.localScale.x;
        if (FacetoMouse() > 0)
            faceDirection = 1;
        if (FacetoMouse() < 0)
            faceDirection = -1;
        transform.localScale = new Vector3(faceDirection, 1, 1);
    }

    public void Jump()
    {
        //Debug.Log("跳跃_函数方法");
        rgbody.AddForce(transform.up * 跳跃高度, ForceMode2D.Impulse);

    }

    public void CanMove(InputAction.CallbackContext callbackContext)
    {
        value_inputControl = callbackContext.ReadValue<Vector2>();
    }

    /// <summary>
    /// 用于计算鼠标相对于摄像机的坐标x位置减去当前对象的x位置，并将Scale正确设置
    /// </summary>
    public int FacetoMouse()
    {
        // 获取鼠标位置
        Vector2 mouseScreenPos = mousePositionAction.ReadValue<Vector2>();
        // 将鼠标屏幕坐标转换为世界坐标，2D游戏中z轴设为0
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        int direaction = mouseWorldPos.x - transform.position.x > 0 ? 1 : -1;
        return direaction;
    }
}
