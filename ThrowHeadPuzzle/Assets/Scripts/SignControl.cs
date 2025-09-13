using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignControl : MonoBehaviour
{
    public Transform player;
    public Vector3 偏移;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    //[SerializeField] SpriteRenderer spriteRendererQ;
    public DeliverBoolSO _boolSO;
    public DeliverTransformSO transformSO;

    void Awake()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        transformSO.RaiseEvent(transform);
    }

    private void OnEnable()
    {
        _boolSO._boolvalue += UpdateState;
    }

    private void OnDisable()
    {
        _boolSO._boolvalue -= UpdateState;
    }

    void UpdateState(bool _bool)
    {
        spriteRenderer.enabled = _bool;
    }

    void Update()
    {
        transform.position = player.transform.position + 偏移;
    }
}
