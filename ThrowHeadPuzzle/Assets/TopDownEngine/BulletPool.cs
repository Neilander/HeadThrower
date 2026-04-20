using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [Header("子弹池配置")]
    public GameObject 子弹预制体; // 绑定你的KoalaGunBullet
    public int 池大小 = 20; // 你要求的20发子弹
    private Queue<GameObject> 子弹池;

    // ====================== 【新增内容】开始 ======================
    // 新增：存储活跃子弹的飞行数据（方向+计时器）
    private struct BulletInfo
    {
        public Vector2 飞行方向;
        public float 计时器;
    }

    // 新增：绑定子弹对象和它的信息
    private Dictionary<GameObject, BulletInfo> 活跃子弹 = new Dictionary<GameObject, BulletInfo>();

    [Header("新增：子弹参数")]
    public float 子弹速度 = 12f;
    public float 存活时间 = 2f;

    // ====================== 【新增内容】结束 ======================

    private void Awake()
    {
        // 单例（多场景安全）
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 切换场景不销毁子弹池
        DontDestroyOnLoad(gameObject);

        // 初始化：预生成20个子弹（核心！只生成一次）
        子弹池 = new Queue<GameObject>();
        if (子弹预制体 != null)
        {
            初始化子弹池();
        }
    }

    // 预生成20个子弹，全部隐藏
    private void 初始化子弹池()
    {
        for (int i = 0; i < 池大小; i++)
        {
            GameObject 子弹 = Instantiate(子弹预制体);
            子弹.SetActive(false);
            子弹.transform.SetParent(transform);
            子弹池.Enqueue(子弹);
        }
    }

    // ====================== 【重载：兼容原有调用，新增方向】 ======================
    // 原有方法完全保留！新增一个带方向的获取子弹方法
    public GameObject 获取子弹(Vector3 位置, Quaternion 旋转, Vector2 方向)
    {
        if (子弹池.Count == 0)
        {
            Debug.LogWarning("子弹池空了！");
            return null;
        }

        GameObject 可用子弹 = 子弹池.Dequeue();
        可用子弹.SetActive(true);
        可用子弹.transform.position = 位置;
        可用子弹.transform.rotation = 旋转;

        // ========== 新增：调试日志 - 看子弹是否进了活跃字典 ==========
        Debug.Log("【子弹入池】" + 可用子弹.name + " 方向：" + 方向);

        活跃子弹.Add(可用子弹, new BulletInfo { 飞行方向 = 方向.normalized, 计时器 = 0 });

        // 新增：打印当前字典里的子弹数量
        Debug.Log("当前活跃子弹数量：" + 活跃子弹.Count);

        return 可用子弹;
    }

    // ====================== 【原有方法完全不动】 ======================
    // 从池子拿子弹（激活，不实例化）
    public GameObject 获取子弹(Vector3 位置, Quaternion 旋转)
    {
        if (子弹池.Count == 0)
        {
            Debug.LogWarning("子弹池空了！");
            return null;
        }

        GameObject 可用子弹 = 子弹池.Dequeue();
        可用子弹.SetActive(true);
        可用子弹.transform.position = 位置;
        可用子弹.transform.rotation = 旋转;
        return 可用子弹;
    }

    // 回收子弹（隐藏，放回池子）
    public void 回收子弹(GameObject 子弹)
    {
        子弹.SetActive(false);
        子弹池.Enqueue(子弹);

        // ====================== 【新增：回收时清理数据】 ======================
        if (活跃子弹.ContainsKey(子弹))
        {
            活跃子弹.Remove(子弹);
        }
    }

    // ====================== 【新增：统一管理子弹飞行+回收】 ======================
    private void Update()
    {
        List<GameObject> 待回收子弹 = new List<GameObject>();

        // 把 foreach 换成 for，直接操作原数据，修复结构体值拷贝问题
        var 子弹键列表 = new List<GameObject>(活跃子弹.Keys);
        for (int i = 0; i < 子弹键列表.Count; i++)
        {
            GameObject 子弹 = 子弹键列表[i];
            if (!活跃子弹.ContainsKey(子弹))
                continue;

            // 直接取字典里的原始数据
            BulletInfo 信息 = 活跃子弹[子弹];
            // 飞行
            子弹.transform.Translate(信息.飞行方向 * 子弹速度 * Time.deltaTime, Space.World);
            // 计时器真正累加！
            信息.计时器 += Time.deltaTime;
            // 把修改后的值塞回字典
            活跃子弹[子弹] = 信息;

            // 判断超时
            if (信息.计时器 >= 存活时间)
            {
                Debug.Log("【子弹超时】计时器：" + 信息.计时器);
                待回收子弹.Add(子弹);
            }
        }

        // 回收
        foreach (var 子弹 in 待回收子弹)
        {
            Debug.Log("【执行回收】" + 子弹.name);
            回收子弹(子弹);
        }
    }
}
