using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformMovement : BaseInteraction
{
    private WorldMover mover;
    [SerializeField]

    private bool isMoving = false;
    [SerializeField]

    private Transform playerTransform;
    [SerializeField]

    private bool playerOnPlatform = false;
    [SerializeField]

    private Vector3 offset;
    public Transform Qsign;
    [SerializeField]
    private Transform pos1;
    [SerializeField]
    private Transform pos2;

    private bool isPos1 = true;
    private Vector3 lastPlatformPosition; // 记录平台上一帧的位置


    private void Awake()
    {
        mover = GetComponent<WorldMover>();

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = true;
            playerTransform = other.transform;
            //offset = playerTransform.position - transform.position;
            //playerTransform = other.transform;
            // 计算玩家相对于平台的初始偏移量
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Qsign.GetComponent<SpriteRenderer>().enabled = false;

            playerOnPlatform = false;
            playerTransform = null;
            isMoving = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Find Player!");
            //Qsign.GetComponent<SpriteRenderer>().enabled = true;
            //playerTransform = other.transform;
        }
    }

    private void Update()
    {
        float platformXMovement = transform.position.x - lastPlatformPosition.x;
        if (playerOnPlatform)
        {

            if (playerTransform != null)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    isMoving = true;
                    OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
                }


                // 让玩家跟随平台移动                
                playerTransform.position = new Vector3(playerTransform.position.x + platformXMovement, playerTransform.position.y, playerTransform.position.z);
                // 动态更新offset
                //offset = playerTransform.position - transform.position;
            }

            // 玩家在平台上可以自由移动
            // Vector2 movement = movementAction.ReadValue<Vector2>();
            // playerTransform.Translate(movement * Time.deltaTime);

        }
        // 更新平台上一帧的位置
        lastPlatformPosition = transform.position;
    }

    public override bool OnInteract(InteractionSignal signal)
    {
        //Qsign.GetComponent<SpriteRenderer>().enabled = false;

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
