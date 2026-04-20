using UnityEngine;

public class ShootOut : MonoBehaviour
{
    [Header("子弹速度")]
    public float 子弹速度 = 15f;

    [Header("自动回收时间（秒）")]
    public float 存活时间 = 1.5f;

    private Vector2 _发射方向;
    private float _计时器;

    // 接收射击方向
    public void GetSignal(Vector2 方向)
    {
        _发射方向 = 方向.normalized; // 标准化方向（必加，否则不动）
        _计时器 = 0;
    }

    private void Update()
    {
        // 2D子弹标准移动方式（修复原地不动）
        transform.Translate(_发射方向 * 子弹速度 * Time.deltaTime, Space.World);

        // 计时回收（修复子弹永久存在）
        _计时器 += Time.deltaTime;
        if (_计时器 >= 存活时间)
        {
            BulletPool.Instance.回收子弹(gameObject);
        }
    }

    // 碰撞后立刻回收
    private void OnTriggerEnter2D(Collider2D other)
    {
        BulletPool.Instance.回收子弹(gameObject);
    }
}
