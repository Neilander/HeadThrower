using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteract : BaseInteraction
{
    public DeliverBoolSO _keyboolSO;

    [Header("界面显示状态")]
    [Tooltip("跨场景传送显示状态的文件")]
    public DeliverBoolSO _UIboolSO;

    [Header("物品ID传递")]
    [Tooltip("跨场景传送物品ID数据的文件")]
    public DeliverintSO _UIintSO;

    [SerializeField]
    bool playerInArea;

    [SerializeField]
    public int ItemID;

    // public string sceneToLoad;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _UIintSO.RaiseEvent(ItemID);
            //Debug.Log(ItemID);
            _keyboolSO.RaiseEvent(true);
            playerInArea = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _keyboolSO.RaiseEvent(false);
            playerInArea = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInArea)
        {
            OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress));
        }
    }

    public override bool OnInteract(InteractionSignal signal)
    {
        LoadUIScene();
        return true;
    }

    private void LoadUIScene()
    {
        Debug.Log("加载UI");
        _UIboolSO.RaiseEvent(true);
    }

    // private void OnEnable()
    // {
    //     _UIboolSO._boolvalue += DeliverItemID;
    // }

    // private void OnDisable()
    // {
    //     _UIboolSO._boolvalue -= DeliverItemID;
    // }

    // private void DeliverItemID(bool _bool)
    // {
    //     _UIintSO.RaiseEvent(ItemID);
    // }
}
