using UnityEngine;

public class HeadShoot : BaseInteraction
{
    private const float 零值 = 0f;
    private const float 枪械默认Z旋转角 = 0f;
    private const int 鼠标左键索引 = 0;

    [Header("基础配置")]
    [SerializeField]
    private Vector2 初始位置;
    private Vector2 枪械朝向;

    [Header("射击设置")]
    [SerializeField]
    private float 射击间隔 = 0.1f; // 连发速度（越小越快）
    private float 下一次射击时间; // 冷却计时器

    void Update()
    {
        bool 头部已分离 = !transform.GetComponent<PickUpHead>().isPickUp;
        bool 按住射击键 = Input.GetMouseButton(鼠标左键索引);

        if (头部已分离 && 按住射击键 && Time.time >= 下一次射击时间)
        {
            枪械朝向 = transform.localScale;
            枪械朝向.y = 零值;
            OnInteract(new InteractionSignal(gameObject, InteractionType.KeyPress, 枪械朝向));
            下一次射击时间 = Time.time + 射击间隔;
        }
        else
        {
            transform.rotation = Quaternion.Euler(零值, 零值, 零值);
        }
    }

    public override bool OnInteract(InteractionSignal 交互信号)
    {
        // 传入：位置、旋转、射击方向
        GameObject 子弹 = BulletPool.Instance.获取子弹(
            transform.TransformPoint(初始位置),
            Quaternion.Euler(零值, 零值, 枪械默认Z旋转角),
            枪械朝向 // 射击方向
        );

        if (子弹 != null)
        {
            // 修正：直接传递枪械朝向，解决报错
            子弹.GetComponent<ShootOut>()?.GetSignal(枪械朝向);
        }
        return true;
    }
}
