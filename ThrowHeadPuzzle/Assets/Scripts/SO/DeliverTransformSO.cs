using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "事件/传递transform类型变量的事件SO")]

public class DeliverTransformSO : ScriptableObject
{
    public UnityAction<Transform> _transform;
    private Transform latestTransform;

    public void RaiseEvent(Transform temptranform)
    {
        _transform?.Invoke(temptranform);
        latestTransform = temptranform;
    }
    public Transform GetLatestTransform()
    {
        return latestTransform;
    }
}

