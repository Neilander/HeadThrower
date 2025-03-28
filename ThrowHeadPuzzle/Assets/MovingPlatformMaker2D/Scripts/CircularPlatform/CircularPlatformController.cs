using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MovingPlatformMaker2D
{
    [AddComponentMenu("Moving Platform Maker 2D/Circular Platform Controller")]
    public class CircularPlatformController : MonoBehaviour
    {
        [HideInInspector]
        [Obsolete("Now the gizmos are in a separate component: CircularPlatformControllerGizmosDecorator")]
        public Color gizmoColor = Color.green;
        public float degreesPerSecond = 65.0f;
        public float radius = 3f;
        public int numberOfPlatforms = 1;
        public GameObject platformPrefab;
        public bool previewActive = true;

        void Reset()
        {
            previewActive = false;
            AddGizmos();
        }

        void Awake()
        {
            if (platformPrefab == null)
            {
                Debug.LogWarning("CircularPlatformController must have a platform prefab");
                enabled = false;
                return;
            }

            UpdatePlatforms();
        }

        public void AddGizmos()
        {
            CircularPlatformControllerGizmosDecorator decorator = gameObject.GetComponent<CircularPlatformControllerGizmosDecorator>();
            if (decorator == null)
            {
                decorator = gameObject.AddComponent<CircularPlatformControllerGizmosDecorator>();
                decorator.gizmoColor = gizmoColor;
            }
        }

        void OnValidate()
        {
            radius = Mathf.Clamp(radius, 0f, float.MaxValue);
            numberOfPlatforms = Mathf.Clamp(numberOfPlatforms, 0, int.MaxValue);
        }

        public void UpdatePlatforms()
        {
            DestroyExistingPlatforms();
            CreatePlatforms();
        }

        public void DestroyExistingPlatforms()
        {
            CircularPlatform[] platforms = GetComponentsInChildren<CircularPlatform>();
            foreach (CircularPlatform platform in platforms)
            {
                if (Application.isPlaying)
                    Destroy(platform.gameObject);
                else
                    DestroyImmediate(platform.gameObject);
            }
        }

        public void CreatePlatforms()
        {
            if (numberOfPlatforms <= 0)
                return;

            if (platformPrefab == null)
                return;

            float angle = 360 / numberOfPlatforms;
            for (int x = 0; x < numberOfPlatforms; x++)
            {
                GameObject platform = Instantiate(platformPrefab, transform) as GameObject;
                platform.hideFlags = HideFlags.HideInHierarchy;

                CircularPlatform circularPlatform = platform.AddComponent<CircularPlatform>();
                circularPlatform.DegreesPerSecond = degreesPerSecond;

                platform.transform.localPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle * x), Mathf.Sin(Mathf.Deg2Rad * angle * x), 0) * radius;
            }
        }

    }

}