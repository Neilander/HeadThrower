using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HeadShoot : BaseInteraction
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private GameObject curbullet;

    [SerializeField]
    private Vector2 枪械朝向;

    [SerializeField]
    private Vector2 初始位置;

    [SerializeField]
    private Vector3 初始角度;

    public float FaceToMouse()
    {
        //当捡起来作为头的时候，不断瞄准鼠标
        //枪械朝向 = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        枪械朝向 = 枪械朝向.normalized;

        float z_Angle = Mathf.Atan2(枪械朝向.x, 枪械朝向.y) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, -z_Angle + 90);
        return -z_Angle + 90;
    }

    void Update()
    {
        bool headDetached = !transform.GetComponent<PickUpHead>().isPickUp;
        bool shootKeyDown = Input.GetMouseButtonDown(0);
        if (headDetached && shootKeyDown)
        {
            Debug.Log("shoot");
            枪械朝向 = transform.localScale;
            枪械朝向.y = 0;

            //重装弹药
            OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress, 枪械朝向));
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public override bool OnInteract(InteractionSignal signal)
    {
        //Shoot(signal);
        //先传递InteractionSignal signal射击方向
        //再使得canshoot变量为true
        初始角度 = new Vector3(0, 0, 90);
        GameObject bullet_reload = Instantiate(
            bullet,
            初始位置,
            Quaternion.Euler(初始角度),
            transform
        );
        bullet_reload.transform.localPosition = 初始位置;
        bullet_reload.transform.SetParent(transform, true);
        FindBullet();
        curbullet.GetComponent<ShootOut>().GetSignal(signal);
        return true;
    }

    /// <summary>
    /// 用于找到子物体中的子弹
    /// </summary>
    public void FindBullet()
    {
        foreach (Transform childTransform in transform)
        {
            // 检查子对象的tag是否为"Bullet"
            if (childTransform.CompareTag("Bullet"))
            {
                // Debug.Log("找到tag为Bullet的子对象: " + childTransform.name);
                // 在这里可以对找到的对象进行其他操作，比如修改属性等
                curbullet = childTransform.gameObject;
                // 例如：childObject.SetActive(false);
            }
        }
    }
}
