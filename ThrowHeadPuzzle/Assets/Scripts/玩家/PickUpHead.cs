using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PickUpHead : BaseInteraction
{
    // 重构说明：
    // 1) 将魔法数字提取为语义化常量。
    // 2) 将所有 if 控制流条件提取为上一行 bool 变量。
    // 3) 优先将可安全重命名的英文变量改为中文语义命名。
    private const float 头部默认偏移Y = 0.5f;
    private const float 欧拉角默认值 = 0f;
    private const float 缩放默认值 = 1f;
    private const KeyCode 交互按键 = KeyCode.E;

    public PlayerController playerController;
    public bool isPickUp;
    public DeliverBoolSO eventE;


    void Update()
    {
        bool 按下交互键且满足拾取条件 =
            Input.GetKeyDown(交互按键)
            && playerController.curstate == PlayerController.ThrowState.NoHead
            && playerController.CanPickUp()
            && playerController._currentTrigger.transform == transform;
        if (按下交互键且满足拾取条件)
        {
            OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
        }
    }

    [SerializeField] Vector3 头偏移量 = new Vector3(欧拉角默认值, 头部默认偏移Y, 欧拉角默认值);
    public override bool OnInteract(InteractionSignal signal)
    {
        RigidbodyController 头部刚体控制器 = gameObject.GetComponent<RigidbodyController>();
        //移除刚体
        头部刚体控制器.DeAddRGbody();
        //与身体组合，变成子对象
        头部刚体控制器.transform.SetParent(signal.source.transform);
        //设置正确的位置
        头部刚体控制器.transform.localPosition = 头偏移量;
        头部刚体控制器.transform.eulerAngles = new Vector3(欧拉角默认值, 欧拉角默认值, 欧拉角默认值);
        头部刚体控制器.transform.localScale = new Vector3(缩放默认值, 缩放默认值, 缩放默认值);
        isPickUp = true;
        return true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        bool 进入触发器的是玩家 = collision.CompareTag("Player");
        if (进入触发器的是玩家)
        {
            eventE.RaiseEvent(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        bool 离开触发器的是玩家 = collision.CompareTag("Player");
        if (离开触发器的是玩家)
        {
            eventE.RaiseEvent(false);
        }
    }
}