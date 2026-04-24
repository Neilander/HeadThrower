using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [Header("子弹池配置")]
    public GameObject 子弹预制体;
    public int 池大小 = 20;
    private Queue<GameObject> 子弹池;

    private struct BulletInfo
    {
        public Vector2 飞行方向;
        public float 计时器;
    }

    private Dictionary<GameObject, BulletInfo> 活跃子弹 = new Dictionary<GameObject, BulletInfo>();

    [Header("新增：子弹参数")]
    public float 子弹速度 = 12f;
    public float 存活时间 = 2f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        子弹池 = new Queue<GameObject>();
        if (子弹预制体 != null)
        {
            初始化子弹池();
        }
    }

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

    public GameObject 获取子弹(Vector3 位置, Quaternion 旋转, Vector2 方向)
    {
        if (子弹池.Count == 0)
        {
            return null;
        }
        GameObject 可用子弹 = 子弹池.Dequeue();
        可用子弹.SetActive(true);
        SpriteRenderer 子弹图片 = 可用子弹.GetComponentInChildren<SpriteRenderer>();
        子弹图片.enabled = true;
        可用子弹.transform.position = 位置;
        可用子弹.transform.rotation = 旋转;

        // ===================== 【修正版】保留原始缩放，仅翻转朝向 =====================
        Vector3 原始缩放 = 可用子弹.transform.localScale;
        float 朝向符号 = Mathf.Sign(方向.x);
        // 核心：用原始缩放的绝对值 × 方向符号 → 大小不变，仅改左右
        可用子弹.transform.localScale = new Vector3(
            Mathf.Abs(原始缩放.x) * 朝向符号,
            原始缩放.y,
            原始缩放.z
        );

        if (活跃子弹.ContainsKey(可用子弹))
        {
            活跃子弹.Remove(可用子弹);
        }
        活跃子弹.Add(可用子弹, new BulletInfo { 飞行方向 = 方向.normalized, 计时器 = 0 });

        // ===================== 物理移动核心：设置刚体速度（替换手动位移） =====================
        Rigidbody2D 子弹刚体 = 可用子弹.GetComponent<Rigidbody2D>();
        if (子弹刚体 != null)
        {
            子弹刚体.velocity = 方向.normalized * 子弹速度;
        }

        Bullet 子弹脚本 = 可用子弹.GetComponentInChildren<Bullet>();
        if (子弹脚本 != null)
        {
            子弹脚本.enabled = true;
            子弹脚本.播放拖尾特效();
        }
        return 可用子弹;
    }

    // 回收子弹（隐藏，放回池子）
    public void 回收子弹(GameObject 子弹)
    {
        if (子弹 == null || !子弹.activeSelf)
            return;

        Rigidbody2D 子弹刚体 = 子弹.GetComponent<Rigidbody2D>();
        if (子弹刚体 != null)
        {
            子弹刚体.velocity = Vector2.zero;
        }

        // ===================== 诊断日志 START =====================
        Bullet 子弹脚本 = 子弹.GetComponentInChildren<Bullet>();
        if (子弹脚本 != null)
        {
            子弹脚本.播放冲击特效();
        }
        // ===================== 诊断日志 END =====================

        if (活跃子弹.ContainsKey(子弹))
        {
            活跃子弹.Remove(子弹);
        }

        // 子弹.SetActive(false);
        // 子弹池.Enqueue(子弹);
        SpriteRenderer 子弹图片 = 子弹.GetComponentInChildren<SpriteRenderer>();
        子弹图片.enabled = false;
        StartCoroutine(安全延迟隐藏(子弹));
    }

    private IEnumerator 安全延迟隐藏(GameObject 待回收子弹)
    {
        // 等待0.4秒，让冲击特效完整播放
        yield return new WaitForSeconds(0.35f);

        // 等待结束后，再隐藏子弹+放回池子
        待回收子弹.SetActive(false);
        子弹池.Enqueue(待回收子弹);
    }

    private void Update()
    {
        List<GameObject> 待回收子弹 = new List<GameObject>();

        var 子弹键列表 = new List<GameObject>(活跃子弹.Keys);
        for (int i = 0; i < 子弹键列表.Count; i++)
        {
            GameObject 子弹 = 子弹键列表[i];
            if (!活跃子弹.ContainsKey(子弹))
                continue;

            BulletInfo 信息 = 活跃子弹[子弹];
            // ===================== 删除了手动位移代码，完全由物理引擎控制移动 =====================

            信息.计时器 += Time.deltaTime;
            活跃子弹[子弹] = 信息;

            if (信息.计时器 >= 存活时间)
            {
                待回收子弹.Add(子弹);
            }
        }

        foreach (var 子弹 in 待回收子弹)
        {
            回收子弹(子弹);
        }
    }
}
