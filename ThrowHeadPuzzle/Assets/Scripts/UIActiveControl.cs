using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActiveControl : MonoBehaviour
{
    [Header("界面显示状态")]
    [Tooltip("跨场景传送显示状态的文件")]
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
    }

    void Update()
    {
        menu_transform.gameObject.SetActive(_activebool);
    }
}
