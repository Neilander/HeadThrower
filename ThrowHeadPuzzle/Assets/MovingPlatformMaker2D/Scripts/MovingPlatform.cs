using UnityEngine;
using System.Collections;

namespace MovingPlatformMaker2D
{

    [AddComponentMenu("Moving Platform Maker 2D/Moving Platform")]
    public class MovingPlatform : MonoBehaviour
    {

        public LayerMask layerMask;

        void Reset()
        {
            layerMask = LayerMask.GetMask(new string[] { "Player" });
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            checkCollision(other);
        }

        void OnCollisionStay2D(Collision2D other)
        {
            checkCollision(other);
        }

        void OnCollisionExit2D(Collision2D other)
        {
            if (layerMask.value == 0 || Utils.IsInLayerMask(other.gameObject.layer, layerMask))
            {
                if (other.transform.parent != null && transform == other.transform.parent.transform)
                {
                    other.transform.parent = null;
                }
            }
        }

        private void checkCollision(Collision2D other)
        {
            if (layerMask.value == 0 || Utils.IsInLayerMask(other.gameObject.layer, layerMask))
            {
                foreach (ContactPoint2D contact in other.contacts)
                {
                    Debug.DrawRay(contact.point, contact.normal, Color.red);
                    if (contact.normal.y < 0)
                    {
                        if (other.rigidbody.velocity.y <= 0)
                        {
                            other.transform.parent = transform;
                        }
                        return;
                    }
                }
            }
        }


    }

}