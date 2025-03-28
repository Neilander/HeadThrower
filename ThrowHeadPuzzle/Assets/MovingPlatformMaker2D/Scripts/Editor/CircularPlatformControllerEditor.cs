using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace MovingPlatformMaker2D
{
    [CustomEditor(typeof(CircularPlatformController))]
    public class CircularPlatformControllerEditor : Editor
    {

        private CircularPlatformController controller;

        void OnEnable()
        {
            controller = target as CircularPlatformController;
            controller.AddGizmos(); //Backwards compatibility - To be removed in a future version

            UpdatePlatforms();

            Undo.undoRedoPerformed += UpdatePlatforms;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= UpdatePlatforms;

            if (controller != null)
            {
                if (!controller.previewActive)
                    controller.DestroyExistingPlatforms();
            }
        }

        void UpdatePlatforms()
        {
            controller.UpdatePlatforms();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            DrawDefaultInspector();

            if (EditorGUI.EndChangeCheck())
            {
                UpdatePlatforms();
            }
        }
    }
}