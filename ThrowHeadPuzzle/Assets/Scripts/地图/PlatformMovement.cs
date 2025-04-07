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
    public DeliverBoolSO QsignboolSO;

    public DeliverTransformSO QsignSO;
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
    private void OnEnable()
    {
        // 尝试在未触发事件时读取最新的 Transform 值
        Transform receivedTransform = QsignSO.GetLatestTransform();
        if (receivedTransform != null)
        {
            HandleTransformReceived(receivedTransform);
        }
        else
        {
            // 如果还未存储 Transform 值，订阅事件
            QsignSO._transform += HandleTransformReceived;
        }
        // // 订阅 _transform 事件
        // QsignSO._transform += HandleTransformReceived;
    }

    private void OnDisable()
    {
        // 取消订阅 _transform 事件，防止内存泄漏
        QsignSO._transform -= HandleTransformReceived;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = true;
            playerTransform = other.transform;
            QsignboolSO.RaiseEvent(true);
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
            QsignboolSO.RaiseEvent(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

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
                playerTransform.position =
                new Vector3(playerTransform.position.x + platformXMovement,
                playerTransform.position.y,
                playerTransform.position.z);
            }

        }
        // 更新平台上一帧的位置
        lastPlatformPosition = transform.position;
    }

    /// <summary>
    /// 在这里处理接收到的 Transform 值
    /// </summary>
    /// <param name="receivedTransform"></param>
    private void HandleTransformReceived(Transform receivedTransform)
    {
        Debug.Log("Received Transform: " + receivedTransform.name);
        // 在这里进行其他操作
        Qsign = receivedTransform;
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
