using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public InputControl inputControl;//输入
    public Vector2 value_inputControl;

    [Header("基本参数")]
    public float 速度;
    public float 跳跃高度;
    [Header("物理")]
    public CapsuleCollider2D capsuleCollider;
    public Rigidbody2D rgbody;

    private void Awake()
    {
        inputControl = new InputControl();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        //inputControl.Player.Jump.started += Jump();//旧的方法
    }

    private void FixedUpdate()
    {
        
    }

    public void Jump()
    {
        //Debug.Log("跳跃_函数方法");
        rgbody.AddForce(transform.up * 跳跃高度, ForceMode2D.Impulse);

    }

    public void Move()
    {
        Debug.Log("1");
        value_inputControl = inputControl.Player.Move.ReadValue<Vector2>();

        rgbody.velocity = new Vector2(value_inputControl.x * 速度 * 1, rgbody.velocity.y);

        int 输入值_方向 = (int)transform.localScale.x;
        if (value_inputControl.x > 0)
            输入值_方向 = 1;
        if (value_inputControl.x < 0)
            输入值_方向 = -1;
        transform.localScale = new Vector3(输入值_方向, 1, 1);
    }
}
