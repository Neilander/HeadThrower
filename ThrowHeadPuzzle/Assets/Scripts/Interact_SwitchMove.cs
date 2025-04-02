using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorldMover))]
public class Interact_SwitchMove : BaseInteraction
{
    private WorldMover mover;

    [SerializeField]
    private Transform pos1;
    [SerializeField]
    private Transform pos2;
    public Transform Qsign;

    private bool isPos1 = true;
    void Start()
    {
        mover = GetComponent<WorldMover>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));

        }
    }

    public override bool OnInteract(InteractionSignal signal)
    {
        Qsign.GetComponent<SpriteRenderer>().enabled = false;

        if (signal.type != InteractionType.KeyPress)
            return false;
        if (isPos1)
        {
            mover.MoveTo(pos2.position);
        }
        else
        {
            mover.MoveTo(pos1.position);
        }
        isPos1 = !isPos1;
        return true;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //执行按键提示
            Qsign.GetComponent<SpriteRenderer>().enabled = true;
            //执行同步人物左右移动
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { //执行按键提示
            Qsign.GetComponent<SpriteRenderer>().enabled = false;

            //执行停止同步人物左右移动}
        }
    }
}