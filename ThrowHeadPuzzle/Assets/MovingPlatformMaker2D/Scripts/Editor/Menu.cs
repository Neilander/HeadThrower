using System;
using UnityEngine;
using UnityEditor;

namespace MovingPlatformMaker2D
{

    public class Menu
    {

        [MenuItem("GameObject/Moving Platform Maker 2D/Path", false, 0)]
        static void AddMenuCreatePath()
        {
            GameObject path = new GameObject("Path");
            path.AddComponent<Path>();
            Undo.RegisterCreatedObjectUndo(path, "Created " + path.name);

            SetPosition(path.transform);

            Selection.activeGameObject = path;
        }

        [MenuItem("GameObject/Moving Platform Maker 2D/Path Follower", false, 0)]
        static void AddMenuCreatePathFollower()
        {
            GameObject follower = new GameObject("Path Follower");
            follower.AddComponent<PathFollower>();
            Undo.RegisterCreatedObjectUndo(follower, "Created " + follower.name);

            SetPosition(follower.transform);

            Selection.activeGameObject = follower;
        }

        [MenuItem("GameObject/Moving Platform Maker 2D/Path Follower Trigger", false, 0)]
        static void AddMenuCreatePathFollowerTrigger()
        {
            GameObject trigger = new GameObject("Path Follower Trigger");
            trigger.AddComponent<PathFollowerTrigger>();
            Undo.RegisterCreatedObjectUndo(trigger, "Created " + trigger.name);

            SetPosition(trigger.transform);

            Selection.activeGameObject = trigger;
        }

        [MenuItem("GameObject/Moving Platform Maker 2D/Circular Platform", false, 0)]
        static void AddMenuCreateCircularPlatform()
        {

            GameObject circular = new GameObject("Circular Platform");
            circular.AddComponent<CircularPlatformController>();
            Undo.RegisterCreatedObjectUndo(circular, "Created " + circular.name);

            SetPosition(circular.transform);

            Selection.activeGameObject = circular;
        }

        static void SetPosition(Transform transform)
        {
            if (Selection.activeGameObject == null)
            {
                transform.position = GetSpawnPos();
            }
            else
            {
                transform.parent = Selection.activeGameObject.transform;
                transform.localPosition = Vector3.zero;
            }
        }

        static Vector3 GetSpawnPos()
        {
            Plane plane = new Plane(new Vector3(0, 0, -1), 0);
            float dist = 0;
            Vector3 result = new Vector3(0, 0, 0);
            Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
            if (plane.Raycast(ray, out dist))
            {
                result = ray.GetPoint(dist);
            }
            return new Vector3(result.x, result.y, 0);
        }
    }

}