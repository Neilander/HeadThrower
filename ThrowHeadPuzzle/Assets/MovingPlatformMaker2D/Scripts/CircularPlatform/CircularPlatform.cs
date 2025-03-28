using UnityEngine;
using System.Collections;

namespace MovingPlatformMaker2D
{
    [AddComponentMenu("Moving Platform Maker 2D/Circular Platform")]
    public class CircularPlatform : MonoBehaviour
    {

        public float degreesPerSecond;

        void Update()
        {
            transform.localPosition = Quaternion.AngleAxis(degreesPerSecond * Time.deltaTime, Vector3.forward) * transform.localPosition;
        }

        public float DegreesPerSecond
        {
            set
            {
                degreesPerSecond = value;
            }
        }
    }

}