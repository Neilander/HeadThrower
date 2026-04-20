using UnityEngine;

// 挂在【子弹父物体】上（有Collider+Rigidbody的物体）
public class CollisionDetector : MonoBehaviour
{
    public delegate void MapCollisionEvent(Vector2 碰撞点);
    public event MapCollisionEvent OnMapCollision;

    private const string 地图图层 = "Map";

    // 实体碰撞（墙/地面）
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(地图图层))
        {
            OnMapCollision?.Invoke(collision.contacts[0].point);
            Debug.Log("【碰撞检测器】检测到地图碰撞！", this);
        }
    }
}
