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
}