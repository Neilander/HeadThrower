using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class PickUpHead : BaseInteraction
{
    public PlayerController 玩家控制器;
    public bool isPickUp;
    public DeliverBoolSO 事件E;


    void Update()
    {
        // 检查各条件
        bool 按下交互键 = 玩家控制器.输入控制.Player.Interact.triggered;
        bool 没头状态 = 玩家控制器.当前状态 == PlayerController.投掷状态.没头;
        bool 可以捡起头部 = 玩家控制器.可以捡起();
        bool 当前物体是被触发器 = 玩家控制器.当前触发器 != null && 玩家控制器.当前触发器.transform == transform;

        // 所有条件满足时触发交互
        bool 满足所有条件 = 按下交互键 && 没头状态 && 可以捡起头部 && 当前物体是被触发器;
        if (满足所有条件)
        {
            OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
        }
    }

    [SerializeField] Vector3 头偏移量 = new Vector3(0, 0.5f, 0);
    public override bool OnInteract(InteractionSignal signal)
    {
        RigidbodyController controller = gameObject.GetComponent<RigidbodyController>();
        bool 有控制器 = controller != null;
        
        if (有控制器)
        {
            controller.DeAddRGbody();
            controller.transform.SetParent(signal.source.transform);
            controller.transform.localPosition = 头偏移量;
            controller.transform.eulerAngles = new Vector3(0, 0, 0);
            controller.transform.localScale = new Vector3(1, 1, 1);
            isPickUp = true;
        }
        return true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        bool 是玩家 = collision.CompareTag("Player");
        if (是玩家)
        {
            事件E.RaiseEvent(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        bool 是玩家 = collision.CompareTag("Player");
        if (是玩家)
        {
            事件E.RaiseEvent(false);
        }
    }
}
