using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MovingPlatformMaker2D
{

    [CustomEditor(typeof(Path))]
    public class PathEditor : Editor
    {
        private const float HANDLE_SIZE = 0.05f;

        private Path path;

        private List<Vector2> points = new List<Vector2>();

        private bool editing = false;
        private Tool tool;

        private static GUIStyle toggleButtonStyleNormal = null;
        private static GUIStyle toggleButtonStyleToggled = null;

        private IPathEditor specific;

        void OnEnable()
        {
            path = target as Path;
            if (path != null)
            {
                points = new List<Vector2>(path.GetPoints());
                specific = PathEditorFactory.Create(path);
            }

            tool = Tools.current;
        }

        void OnDisable()
        {
            Tools.current = tool;
            Tools.hidden = false;
        }

        public override void OnInspectorGUI()
        {
            AddEditButton();

            specific = PathEditorFactory.Create(path);
            specific.OnInspectorGUI();
        }

        void AddEditButton()
        {
            if (toggleButtonStyleNormal == null)
            {
                toggleButtonStyleNormal = "Button";
                toggleButtonStyleToggled = new GUIStyle(toggleButtonStyleNormal);
                toggleButtonStyleToggled.normal.background = toggleButtonStyleToggled.active.background;
            }

            GUIStyle style = !editing
                ? toggleButtonStyleNormal
                : toggleButtonStyleToggled;

            bool editButtonState = GUILayout.Toggle(editing, "Edit", style);
            if (editButtonState != editing)
            {
                Tools.hidden = editButtonState;
                SceneView.RepaintAll();

                if (editButtonState)
                {
                    tool = Tools.current;
                    Tools.current = Tool.None;
                }
                else
                    Tools.current = tool;

            }

            editing = editButtonState;
        }

        void OnSceneGUI()
        {
            Handles.color = Color.green;
            points = new List<Vector2>(path.GetPoints());

            DrawLines();

            if (editing)
            {
                DrawButtons();
            }

            Selection.activeGameObject = path.transform.gameObject;
        }

        void DrawLines()
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 current = path.transform.TransformPoint(points[i]);
                Vector2 next = path.transform.TransformPoint(points[i + 1]);
                Handles.DrawLine(current, next);
            }
        }

        void DrawButtons()
        {
            if (Event.current.control)
            {
                DrawDeleteHandles();
                return;
            }

            bool movedNode = HandleMoveNode();
            if (movedNode)
                return;

            if (Event.current.shift)
            {
                HandleAddNode();
                return;
            }

            if (LostFocus())
            {
                editing = false;
                return;
            }

        }

        bool DrawDeleteHandles()
        {
            if (specific.IsShowDeleteHandles(points))
            {
                Handles.color = Color.red;

                for (int i = 0; i < points.Count; i++)
                {
                    Vector2 currentNodeLocalPoint = path.transform.TransformPoint(points[i]);

                    bool deleteClicked = Handles.Button(currentNodeLocalPoint, Quaternion.identity, HANDLE_SIZE, HANDLE_SIZE, Handles.DotHandleCap);
                    if (deleteClicked)
                    {
                        DeletePoint(i);
                        return true;
                    }
                }
            }

            return false;
        }

        bool DrawAddNodeHandles()
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 currentNodeLocalPoint = path.transform.TransformPoint(points[i]);
                Vector2 nextNodeLocalPoint = path.transform.TransformPoint(points[i + 1]);
                Vector2 middlePoint = Vector2.Lerp(currentNodeLocalPoint, nextNodeLocalPoint, 0.5f);

                bool addClicked = Handles.Button(middlePoint, Quaternion.identity, HANDLE_SIZE * 2, HANDLE_SIZE * 2, Handles.SphereHandleCap);
                if (addClicked)
                {
                    int addIndex = i + 1;
                    Vector2 newPoint = (Vector2)path.transform.InverseTransformPoint(middlePoint);
                    AddPointInTheMiddle(newPoint, addIndex);
                    return true;
                }
            }

            return false;
        }

        bool HandleAddNode()
        {
            if (!DrawAddNodeHandles())
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    AddPointAtTheEnd();
                    return true;
                }
            }

            return false;
        }

        bool HandleMoveNode()
        {
            for (int i = 0; i < points.Count; i++)
            {
                EditorGUI.BeginChangeCheck();

                Vector2 currentNodeLocalPoint = path.transform.TransformPoint(points[i]);
                var fmh_200_87_638039676714410192 = Quaternion.identity; currentNodeLocalPoint = Handles.FreeMoveHandle(currentNodeLocalPoint, HANDLE_SIZE, EditorSnapSettings.move, Handles.DotHandleCap);

                if (Event.current.alt)
                    currentNodeLocalPoint = Snapping.Snap(currentNodeLocalPoint, EditorSnapSettings.move);

                if (EditorGUI.EndChangeCheck())
                {
                    points[i] = (Vector2)path.transform.InverseTransformPoint(currentNodeLocalPoint);
                    MovePoint(i);
                    return true;
                }
            }

            return false;
        }

        bool LostFocus()
        {
            return !Event.current.control && !Event.current.shift && Event.current.type == EventType.MouseDown;
        }

        void MovePoint(int index)
        {
            specific.MovePoint(points, index);
        }

        void AddPointAtTheEnd()
        {
            Vector2 clickPoint = path.transform.InverseTransformPoint(GetMouseClickPosition());
            specific.AddPoint(points, clickPoint, points.Count);
        }

        void AddPointInTheMiddle(Vector2 newPoint, int index)
        {
            specific.AddPoint(points, newPoint, index);
        }

        void DeletePoint(int index)
        {
            specific.DeletePoint(points, index);
        }

        Vector2 GetMouseClickPosition()
        {
            float ppp = EditorGUIUtility.pixelsPerPoint;
            Vector3 screenPoint = new Vector3(Event.current.mousePosition.x * ppp, Camera.current.pixelHeight - Event.current.mousePosition.y * ppp );
            Ray r = Camera.current.ScreenPointToRay(screenPoint);
            return r.origin;
        }

    }

}