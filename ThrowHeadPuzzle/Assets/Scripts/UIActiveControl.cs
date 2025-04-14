using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActiveControl : MonoBehaviour
{

    public DeliverBoolSO _UIboolSO;
    [SerializeField]
    Transform menu_transform;
    bool _activebool;
    private void OnEnable()
    {
        _UIboolSO._boolvalue += UpdateState;
    }

    private void OnDisable()
    {
        _UIboolSO._boolvalue -= UpdateState;
    }

    private void UpdateState(bool _bool)
    {
        _activebool = _bool;
        menu_transform.gameObject.SetActive(_activebool);
    }
}
