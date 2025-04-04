using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SignControl : MonoBehaviour
{
    public Transform player;
    public Vector3 偏移;
    [SerializeField] SpriteRenderer spriteRendererE;
    [SerializeField] SpriteRenderer spriteRendererQ;
    public DeliverBoolSO eventboolSO;

    private void OnEnable()
    {
        eventboolSO.eventE.AddListener(UpdateStateE);
        eventboolSO.eventQ.AddListener(UpdateStateQ);
    }

    private void OnDisable()
    {
        eventboolSO.eventE.RemoveListener(UpdateStateE);
        eventboolSO.eventQ.RemoveListener(UpdateStateQ);
    }

    void UpdateStateE(bool _bool)
    {
        spriteRendererE.enabled = _bool;
    }
    void UpdateStateQ(bool _bool)
    {
        spriteRendererQ.enabled = _bool;
    }
    void Update()
    {
        transform.position = player.transform.position + 偏移;
    }
}
