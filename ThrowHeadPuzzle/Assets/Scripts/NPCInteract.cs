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
    // public string sceneToLoad;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
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
}
