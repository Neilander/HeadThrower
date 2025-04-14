using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootOut : BaseInteraction
{
    [SerializeField] private float bullet_speed;
    [SerializeField] private float disapper_time = 4.0f;
    [SerializeField] public bool canShoot = false;

    void Awake()
    {
        canShoot = false;
    }

    private void Shoot(InteractionSignal signal)
    {
        transform.localPosition = (Vector2)signal.data * bullet_speed * Time.deltaTime + (Vector2)transform.localPosition;
    }
    void Update()
    {
        if (canShoot)
        {
            if (transform.parent != null)
            {
                transform.SetParent(null);
                // 启动协程，延迟n秒后销毁物体
                StartCoroutine(DestroyAfterSeconds(disapper_time));
            }

            Shoot(temp_signal);
            //开始一个携程，超过4s以后销毁这个子弹
        }
    }

    private InteractionSignal temp_signal;
    public void GetSignal(InteractionSignal signal)
    {
        temp_signal = signal;
    }

    /// <summary>
    /// 协程方法，实现延迟销毁物体的逻辑
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    IEnumerator DestroyAfterSeconds(float seconds)
    {
        // 等待指定的秒数
        yield return new WaitForSeconds(seconds);

        // 销毁当前游戏对象
        Destroy(gameObject);
    }
}
