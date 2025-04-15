using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteract : BaseInteraction
{
    public DeliverBoolSO _keyboolSO;
    public DeliverBoolSO _UIboolSO;

    [SerializeField]
    bool playerInArea;

    private bool isLoaded = false;

    private GameObject player;
    // public string sceneToLoad;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _keyboolSO.RaiseEvent(true);
            player = collision.gameObject;
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
        if (Input.GetKeyDown(KeyCode.E) && playerInArea&& !isLoaded)
        {
            OnInteract(new InteractionSignal(player, InteractionType.KeyPress));
            //Debug.Log("来");
            isLoaded = true;
           
        }
    }

    public override bool OnInteract(InteractionSignal signal)
    {
        player.GetComponent<PlayerController>().SwitchAllActionStage(false);
        LoadUIScene();
        _UIboolSO._boolvalue += OnRaiseEvent;
        return true;
    }

    public void OnRaiseEvent(bool ifOpen)
    {
        if (!ifOpen)
        {
            isLoaded = false;
            player.GetComponent<PlayerController>().SwitchAllActionStage(true);
            _UIboolSO._boolvalue -= OnRaiseEvent;
        }
    }

    private void LoadUIScene()
    {
        Debug.Log("加载UI");
        _UIboolSO.RaiseEvent(true);
    }
}
