using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneActivator : MonoBehaviour
{

    public List<string> scenesToActivate;
    // Start is called before the first frame update

    private void Awake()
    {
        foreach (string sceneName in scenesToActivate)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            }
        }
    }
}
