using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCInteract : BaseInteraction
{
    public DeliverBoolSO eventE;
    [SerializeField]
    bool playerInArea;
    public string sceneToLoad;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            eventE.Change_E_State(true);
            playerInArea = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            eventE.Change_E_State(false);
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
        LoadScene();
        return true;
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        }
        else
        {
            Debug.LogError("场景名称未设置，无法加载场景。");
        }
    }
}
