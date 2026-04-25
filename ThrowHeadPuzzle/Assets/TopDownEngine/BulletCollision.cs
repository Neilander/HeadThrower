using UnityEngine;

// 【唯一挂载点：子弹根物体（KoalaGunBullet）】
public class BulletCollision : MonoBehaviour
{
    private const string 地图层 = "map";

    // 激活时强制唤醒刚体（解决不碰撞BUG）
    private void OnEnable()
    {
        GetComponent<Rigidbody2D>().WakeUp();
    }

    // 撞墙必触发（2D实体碰撞）
    private void OnCollisionEnter2D(Collision2D 碰撞信息)
    {
        bool 碰撞了地图 = 碰撞信息.gameObject.layer == LayerMask.NameToLayer(地图层);
        bool 是否播放冲击特效 = 碰撞了地图; // 只有碰地图才播放特效
        // 立即回收子弹（不再超时）
        BulletPool.Instance.回收子弹(gameObject, 是否播放冲击特效);
    }
}
