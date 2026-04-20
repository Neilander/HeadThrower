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
        // 必打印日志！只要碰撞就出
        Debug.Log($"【子弹碰撞】撞到物体：{碰撞信息.gameObject.name}");

        // 判断是否撞到地图
        if (碰撞信息.gameObject.layer == LayerMask.NameToLayer(地图层))
        {
            Debug.Log("【子弹碰撞】撞到地图！");

            // 播放粒子特效
            ParticlesFeedback 粒子 = GetComponentInChildren<ParticlesFeedback>();
            if (粒子 != null)
            {
                粒子.播放反馈(碰撞信息.contacts[0].point);
            }

            // 立即回收子弹（不再超时）
            BulletPool.Instance.回收子弹(gameObject);
        }
    }
}
