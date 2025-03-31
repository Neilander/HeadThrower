using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    private InputAction _reloadAction;

    private void Awake()
    {
        _reloadAction = new InputAction("Reload", binding: "<Keyboard>/r");
        _reloadAction.performed += ctx => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnEnable() => _reloadAction.Enable();
    private void OnDisable() => _reloadAction.Disable();

}
