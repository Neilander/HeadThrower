using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyController : MonoBehaviour
{
    [Section("头部属性")]
    public bool isMyHead;

    [Section("投掷力度")]
    public int 投掷力度;

    public void AddRGbody()
    {
        if (GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 4f;
            rb.mass = 1f;
            rb.freezeRotation = true;
            Debug.Log("已添加Rigidbody2D组件");
        }
        else
        {
            Debug.LogWarning("对象已经拥有Rigidbody2D组件");
        }
    }

    public void DeAddRGbody()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Destroy(rb);
            Debug.Log("已移除Rigidbody2D组件");
        }
        else
        {
            Debug.LogWarning("对象没有Rigidbody2D组件可以移除");
        }
    }

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