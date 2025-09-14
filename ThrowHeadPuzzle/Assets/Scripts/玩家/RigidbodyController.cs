using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyController : MonoBehaviour
{
    public bool isMyHead;

    /// <summary>
    /// 为当前游戏对象添加Rigidbody2D组件
    /// </summary>
    public void AddRGbody()
    {
        // 检查是否已经存在Rigidbody2D组件
        if (GetComponent<Rigidbody2D>() == null)
        {
            // 添加Rigidbody2D组件
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            // 可以在这里设置一些默认属性
            rb.gravityScale = 4f;  // 默认重力缩放
            rb.mass = 1f;         // 默认质量
            // 冻结Z轴旋转
            rb.freezeRotation = true;
            Debug.Log("已添加Rigidbody2D组件");
        }
        else
        {
            Debug.LogWarning("对象已经拥有Rigidbody2D组件");
        }
    }

    /// <summary>
    /// 移除当前游戏对象的Rigidbody2D组件
    /// </summary>
    public void DeAddRGbody()
    {
        // 获取Rigidbody2D组件
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // 销毁Rigidbody2D组件
            Destroy(rb);
            Debug.Log("已移除Rigidbody2D组件");
        }
        else
        {
            Debug.LogWarning("对象没有Rigidbody2D组件可以移除");
        }
    }

    public int 投掷力度;
    public void Throw(Vector2 aim)
    {
        if (GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
            Debug.Log("投掷" + aim.normalized * 投掷力度);
            rgbody.AddForce(aim.normalized * 投掷力度, ForceMode2D.Impulse);
        }
    }
}
