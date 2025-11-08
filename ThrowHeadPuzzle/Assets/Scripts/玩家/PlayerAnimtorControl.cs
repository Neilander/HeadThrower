using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimtorControl : MonoBehaviour
{
    public Animator 玩家动画控制器;

    [SerializeField]
    private Rigidbody2D rb;

    void Update()
    {
        玩家动画控制器.SetFloat("玩家移速", Mathf.Abs(rb.velocity.x));
    }
}
