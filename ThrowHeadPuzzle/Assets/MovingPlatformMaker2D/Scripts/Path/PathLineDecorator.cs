using UnityEngine;
using System.Collections;

namespace MovingPlatformMaker2D
{

    [AddComponentMenu("Moving Platform Maker 2D/Path Line Decorator")]
    [RequireComponent(typeof(Path))]
    public class PathLineDecorator : MonoBehaviour
    {

        public Material material;
        [Range(0, 1)]
        public float lineWidth = 0.03f;
        private Path path;

        [HideInInspector]
        public bool changed = true;

        private LineRenderer lr;

        void OnValidate()
        {
            if (material == null && enabled)
            {
                Debug.LogWarning(gameObject.name + ": PathLineDecorator must have a material.");
                enabled = false;
                return;
            }
        }

        void Update()
        {
            UpdateLines();
        }

        public void UpdateLines()
        {
            DeleteLines();

            Vector2[] points = GetPath().GetPoints();

            GetLineRenderer().positionCount = points.Length;
            for (int i = 0; i < points.Length; i++)
                GetLineRenderer().SetPosition(i, points[i]);

            if (GetPath().Type == PathType.Connected && GetPath().connected != null)
            {
                GetLineRenderer().positionCount = points.Length + 1;
                Vector2 connectedPoint = GetPath().connected.GetWorldPoints(GetPath().connected.transform)[0];
                GetLineRenderer().SetPosition(points.Length, transform.InverseTransformPoint(connectedPoint));
            }

            GetLineRenderer().material = material;
            GetLineRenderer().startWidth = lineWidth;
            GetLineRenderer().endWidth = lineWidth;
        }

        void DeleteLines()
        {
            Transform lines = transform.Find("Lines");
            while (lines != null)
            {
                DestroyImmediate(lines.gameObject);
                lines = transform.Find("Lines");
            }
        }

        private LineRenderer GetLineRenderer()
        {
            if (lr == null)
            {
                lr = GetComponent<LineRenderer>();
                if (lr == null)
                    lr = gameObject.AddComponent<LineRenderer>();

                lr.useWorldSpace = false;
                lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lr.receiveShadows = false;

            }
            return lr;
        }

        private Path GetPath()
        {
            if (path == null)
            {
                path = GetComponent<Path>();
                path.OnUpdatePointsEvent += UpdateLines;
            }
            return path;
        }

    }

}