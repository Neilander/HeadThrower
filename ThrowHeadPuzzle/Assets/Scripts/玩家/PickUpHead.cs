using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PickUpHead : BaseInteraction
{
    public PlayerController playerController;
    public bool isPickUp;
    public DeliverBoolSO eventE;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) &&
         playerController.curstate == PlayerController.ThrowState.NoHead &&
         playerController.CanPickUp() &&
         playerController._currentTrigger.transform == transform
         )
        {
            OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
        }
    }

    [SerializeField] Vector3 头偏移量 = new Vector3(0, 0.5f, 0);
    public override bool OnInteract(InteractionSignal signal)
    {
        RigidbodyController controller = gameObject.GetComponent<RigidbodyController>();
        //移除刚体
        controller.DeAddRGbody();
        //与身体组合，变成子对象
        controller.transform.SetParent(signal.source.transform);
        //设置正确的位置
        controller.transform.localPosition = 头偏移量;
        controller.transform.eulerAngles = new Vector3(0, 0, 0);
        controller.transform.localScale = new Vector3(1, 1, 1);
        isPickUp = true;
        return true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            eventE.Change_E_State(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            eventE.Change_E_State(false);
        }
    }
}