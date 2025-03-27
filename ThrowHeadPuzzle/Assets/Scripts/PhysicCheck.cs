using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PhysicCheck : MonoBehaviour
{
    [Header("参数")]
    public bool 手动修改边缘;
    public Vector2 检测偏移量;
    public Vector2 左墙偏移量;
    public Vector2 右墙偏移量;
    public float 检测范围;
    public LayerMask 检测图层;
    private CapsuleCollider2D 胶囊组件;
    [Header("状态")]
    public bool isGround;
    public bool 是否撞左墙;
    public bool 是否撞右墙;
    private void Awake()
    {
        胶囊组件 = GetComponent<CapsuleCollider2D>();

        if (!手动修改边缘)
        {
            左墙偏移量 = new Vector2(胶囊组件.bounds.size.x / 2f + 胶囊组件.offset.x, 0);
            右墙偏移量 = new Vector2(-胶囊组件.bounds.size.x / 2f + 胶囊组件.offset.x, 0);
        }
    }
    public void Update()
    {
        检查();
    }
    public void 检查()
    {
        检测偏移量 = new Vector2(-transform.localScale.x * Math.Abs(检测偏移量.x), 检测偏移量.y);
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + 检测偏移量, 检测范围, 检测图层);
        #region 撞左墙判断
        是否撞左墙 = Physics2D.OverlapCircle((Vector2)transform.position + 左墙偏移量, 检测范围, 检测图层);
        #endregion
        #region 撞右墙判断
        是否撞右墙 = Physics2D.OverlapCircle((Vector2)transform.position + 右墙偏移量, 检测范围, 检测图层);
        #endregion
    }
    //显示检测题的范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + 检测偏移量, 检测范围);
        Gizmos.DrawWireSphere((Vector2)transform.position + 左墙偏移量, 检测范围);
        Gizmos.DrawWireSphere((Vector2)transform.position + 右墙偏移量, 检测范围);
    }
}
