using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MovingPlatformMaker2D
{
    public class PathEditorFactory
    {
        public static IPathEditor Create(Path path)
        {
            switch (path.Type)
            {
                case PathType.PingPong:
                    return new PingPongPathEditor(path);
                case PathType.Connected:
                    return new ConnectedPathEditor(path);
                case PathType.Cyclic:
                    return new CyclicPathEditor(path);
                default:
                    return new IPathEditor(path);
            }
        }
    }
    public class IPathEditor
    {
        protected Path path;

        public IPathEditor(Path path)
        {
            this.path = path;
        }

        public virtual void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            AnimationCurve easingCurve = EditorGUILayout.CurveField("Easing curve", path.easingCurve);
            PathType type = (PathType)EditorGUILayout.EnumPopup("Type", path.Type);

            if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
            {
                RecordChange("Change path properties");

                path.Type = type;
                path.easingCurve = easingCurve;
            }
        }

        public void RecordChange(String cause)
        {
            Undo.RecordObject(path, cause);
            Undo.RecordObject(path.GetCollider(), cause);
        }

        public virtual void MovePoint(List<Vector2> points, int index)
        {
            UpdatePoints(points);
        }

        public virtual void AddPoint(List<Vector2> points, Vector2 newPoint, int index)
        {
            points.Insert(index, newPoint);

            UpdatePoints(points);
        }

        public virtual void DeletePoint(List<Vector2> points, int index)
        {
            if (points.Count <= 2)
                return;

            points.RemoveAt(index);
            
            UpdatePoints(points);

        }

        public virtual bool IsShowDeleteHandles(List<Vector2> points)
        {
            // The edge collider needs at least 2 points to be valid
            return points.Count > 2;
        }

        public void UpdatePoints(List<Vector2> points)
        {
            RecordChange("Update points");

            path.SetPoints(points.ToArray());
        }
    }


    class PingPongPathEditor : IPathEditor
    {
        public PingPongPathEditor(Path path) : base(path)
        {
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            AnimationCurve easingCurve = EditorGUILayout.CurveField("Easing curve", path.easingCurve);
            PathType type = (PathType)EditorGUILayout.EnumPopup("Type", path.Type);
            bool openStart = EditorGUILayout.ToggleLeft("Open start", path.openStart);
            bool openEnd = EditorGUILayout.ToggleLeft("Open end", path.openEnd);

            if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
            {
                RecordChange("Change path properties");

                path.Type = type;
                path.easingCurve = easingCurve;
                path.openStart = openStart;
                path.openEnd = openEnd;
            }
        }
    }

    class ConnectedPathEditor : IPathEditor
    {

        public ConnectedPathEditor(Path path) : base(path)
        {
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            AnimationCurve easingCurve = EditorGUILayout.CurveField("Easing curve", path.easingCurve);
            PathType type = (PathType)EditorGUILayout.EnumPopup("Type", path.Type);
            Path connected = (Path)EditorGUILayout.ObjectField(path.connected, typeof(Path), true);

            if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
            {
                RecordChange("Change path properties");

                path.Type = type;
                path.easingCurve = easingCurve;
                path.connected = connected;
            }
        }

    }

    class CyclicPathEditor : IPathEditor
    {

        public CyclicPathEditor(Path path) : base(path)
        {
        }

        public override void MovePoint(List<Vector2> points, int index)
        {
            //Move start and end points together

            int last = points.Count - 1;
            if (index == 0)
            {
                points[last] = points[0];
            }
            if (index == last)
            {
                points[0] = points[last];
            }

            UpdatePoints(points);
        }

        public override void AddPoint(List<Vector2> points, Vector2 newPoint, int index)
        {
            if (path.IsCyclic())
            {
                if (index == 0 || index == points.Count)
                {
                    index = points.Count - 1;
                }
            }

            points.Insert(index, newPoint);

            UpdatePoints(points);
        }

        public override void DeletePoint(List<Vector2> points, int index)
        {
            if (points.Count <= 3)
                return;



            if (index == 0 || index == points.Count - 1)
            {
                points.RemoveAt(0);
                points.RemoveAt(points.Count - 1);
                points.Add(points[0]);
            }
            else
            {
                points.RemoveAt(index);
            }

            UpdatePoints(points);
        }

        public override bool IsShowDeleteHandles(List<Vector2> points)
        {
            // Two points are always at the same position
            // So to preserve the behaviour of the other types
            // It needs to have more than 3
            return points.Count > 3;
        }


    }
}