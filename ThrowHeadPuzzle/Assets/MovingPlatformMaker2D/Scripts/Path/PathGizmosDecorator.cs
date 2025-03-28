using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MovingPlatformMaker2D
{

    [AddComponentMenu("Moving Platform Maker 2D/Path Gizmos Decorator")]
    public class PathGizmosDecorator : MonoBehaviour
    {

        public Color color = Color.green;

        private Path path;

        void OnValidate()
        {
            path = GetComponent<Path>();
            if (path == null && enabled)
            {
                Debug.LogWarning(gameObject.name + ": Path not found for this decorator.");
                enabled = false;
                return;
            }
        }

        void OnDrawGizmos()
        {
            if (path == null)
                return;

            Vector2[] points = path.GetWorldPoints(transform);

            Gizmos.color = color;

            for (int i = 0; i < points.Length - 1; i++)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }

            if (path.Type == PathType.Connected && path.connected != null)
            {
                Gizmos.DrawLine(points[points.Length - 1], path.connected.GetWorldPoints(path.connected.transform)[0]);
            }
        }

    }

}