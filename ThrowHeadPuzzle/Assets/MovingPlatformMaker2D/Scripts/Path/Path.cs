using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MovingPlatformMaker2D
{

    [AddComponentMenu("Moving Platform Maker 2D/Path")]
    public class Path : MonoBehaviour
    {
        public event Action OnUpdatePointsEvent;

        public AnimationCurve easingCurve;
        public bool openStart;
        public bool openEnd;
        public Path connected;

        [SerializeField]
        private PathType type = PathType.PingPong;
        private EdgeCollider2D col;
        private PathDelegate self;
        private AnimationEasingCurve animationEasingCurve = new AnimationEasingCurve();

        void Reset()
        {
            SetPoints(Delegate.ChangeType(type, GetPoints()));
            easingCurve = AnimationCurve.Linear(0, 0, 1, 1);

            AddGizmos();
        }

        void Awake()
        {
            GetCollider();
            CalculateDirectionsAndMagnitudes();
        }

        public void AddGizmos()
        {
            PathGizmosDecorator decorator = gameObject.GetComponent<PathGizmosDecorator>();
            if (decorator == null)
            {
                gameObject.AddComponent<PathGizmosDecorator>();
            }
        }

        public EdgeCollider2D GetCollider()
        {
            if (col == null)
            {
                col = GetComponent<EdgeCollider2D>();
                if (col == null)
                {
                    col = gameObject.AddComponent<EdgeCollider2D>();
                }
                col.isTrigger = true;
            }
            return col;
        }

        public Vector2[] GetPoints()
        {
            return GetCollider().points;
        }

        public void CalculateDirectionsAndMagnitudes()
        {
            Delegate.CalculateDirectionsAndMagnitudes(GetPoints());
        }

        public float GetProgress(Vector2 other)
        {
            Vector2 local = transform.InverseTransformPoint(other);
            animationEasingCurve.SetCurve(easingCurve);
            return Delegate.GetProgress(GetPoints(), local, animationEasingCurve);
        }

        public Vector2 GetPosition(float progress)
        {
            animationEasingCurve.SetCurve(easingCurve);
            Vector3 local = Delegate.GetPosition(GetPoints(), progress, animationEasingCurve);
            return transform.TransformPoint(local);
        }

        public Vector2 GetDirection(float progress)
        {
            return Delegate.GetDirection(progress);
        }

        public Vector2 GetNextWaypoint(float progress, PathDirection direction)
        {
            return transform.TransformPoint(Delegate.GetNextWaypoint(GetPoints(), progress, direction));
        }

        public void SetPoints(Vector2[] points)
        {
            GetCollider().points = points;
            CalculateDirectionsAndMagnitudes();

            if (OnUpdatePointsEvent != null)
                OnUpdatePointsEvent();
        }

        public PathType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type != value)
                {
                    type = value;
                    SetPoints(Delegate.ChangeType(type, GetPoints()));
                }
            }
        }

        public PathDelegate Delegate
        {
            get
            {
                if (self == null)
                    self = new PathDelegate();
                return self;
            }
        }

        public float Length
        {
            get
            {
                return Delegate.Length;
            }
        }

        public bool IsCyclic()
        {
            return PathType.Cyclic.Equals(type);
        }

        public Vector2[] GetWorldPoints(Transform reference)
        {
            List<Vector2> newPoints = new List<Vector2>();

            Vector2[] points = GetPoints();

            for (int i = 0; i < points.Length; i++)
                newPoints.Add(reference.TransformPoint(points[i]));

            return newPoints.ToArray();
        }

    }

}