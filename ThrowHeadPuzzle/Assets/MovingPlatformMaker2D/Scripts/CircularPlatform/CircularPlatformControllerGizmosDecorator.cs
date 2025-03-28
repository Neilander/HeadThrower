using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace MovingPlatformMaker2D
{
    [AddComponentMenu("Moving Platform Maker 2D/Circular Platform Controller Gizmos")]
    public class CircularPlatformControllerGizmosDecorator : MonoBehaviour
    {
        public Color gizmoColor = Color.green;

        private CircularPlatformController controller;

        void OnValidate()
        {
            controller = gameObject.GetComponent<CircularPlatformController>();
            if (controller == null && enabled)
            {
                Debug.LogWarning(gameObject.name + ": CircularPlatformController missing for adding gizmos.");
                enabled = false;
                return;
            }
        }

        void OnDrawGizmos()
        {
            if (controller == null)
                return;

#if UNITY_EDITOR
            Handles.color = gizmoColor;
            Handles.DrawWireDisc(transform.localPosition, Vector3.forward, controller.radius);
#endif
        }

    }
}