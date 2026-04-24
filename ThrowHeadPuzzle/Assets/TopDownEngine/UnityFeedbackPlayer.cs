using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 纯Unity原生武器组件，100%还原原MM Weapon的所有配置
/// 全中文命名，移除所有MoreMountains依赖，保留所有配置项
/// </summary>
public class NativeWeapon : MonoBehaviour
{
    #region 常量定义 (提取魔法数字)
    private const float 默认射击间隔 = 0.1f;
    private const float 默认装填时间 = 2f;
    private const float 默认后坐力 = 30f;
    private const float 默认移动倍率 = 0.5f;
    #endregion

    #region 枚举定义 (中文枚举，修复重名问题)
    /// <summary>
    /// 射击触发模式
    /// </summary>
    public enum 触发模式枚举
    {
        自动,
        手动,
        半自动,
    }

    /// <summary>
    /// 生成点轮询模式
    /// </summary>
    public enum 生成点模式枚举
    {
        顺序轮询,
        随机,
    }
    #endregion

    #region 1. ID 面板 (蓝色分组)
    [Tooltip("武器名称")]
    public string 武器名称 = "";

    [Tooltip("当前是否激活该武器")]
    public bool 当前激活 = true;
    #endregion

    #region 2. Use 面板 (绿色分组)
    [Header("<color=#4CAF50>▶  射击/使用逻辑</color>")]
    [Tooltip("<color=#4CAF50>■</color> 启用输入授权")]
    public bool 输入授权 = true;

    [Tooltip("<color=#4CAF50>■</color> 射击触发模式")]
    public 触发模式枚举 触发模式 = 触发模式枚举.自动;

    [Tooltip("<color=#4CAF50>■</color> 使用前延迟（秒）")]
    public float 使用前延迟 = 0f;

    [Tooltip("<color=#4CAF50>■</color> 允许在按住时松开中断使用延迟")]
    public bool 允许使用前延迟中断 = true;

    [Tooltip("<color=#4CAF50>■</color> 两次射击之间的最小间隔（秒）")]
    public float 射击间隔 = 默认射击间隔;

    [Tooltip("<color=#4CAF50>■</color> 允许松开时中断间隔计时")]
    public bool 允许间隔中断 = true;

    [Header("<color=#4CAF50>▶ 连发模式</color>")]
    [Tooltip("<color=#4CAF50>■</color> 启用连发模式")]
    public bool 启用连发模式 = false;

    [Tooltip("<color=#4CAF50>■</color> 每次连发的子弹数量")]
    public int 连发长度 = 3;

    [Tooltip("<color=#4CAF50>■</color> 连发内单发之间的间隔（秒）")]
    public float 连发间隔 = 0.1f;
    #endregion

    #region 3. Magazine 面板 (橙色分组)
    [Header("<color=#FF9800>▶  弹匣/弹药系统</color>")]
    [Tooltip("<color=#FF9800>■</color> 是否为弹匣式武器")]
    public bool 弹匣式武器 = false;

    [Tooltip("<color=#FF9800>■</color> 弹匣容量")]
    public int 弹匣容量 = 30;

    [Tooltip("<color=#FF9800>■</color> 空弹匣时自动装填")]
    public bool 自动装填 = false;

    [Tooltip("<color=#FF9800>■</color> 装填是否需要手动输入")]
    public bool 装填需手动输入 = false;

    [Tooltip("<color=#FF9800>■</color> 装填耗时（秒）")]
    public float 装填耗时 = 默认装填时间;

    [Tooltip("<color=#FF9800>■</color> 每发消耗的弹药数量")]
    public int 每发消耗弹药 = 1;

    [Tooltip("<color=#FF9800>■</color> 空弹时自动销毁武器")]
    public bool 空弹自动销毁 = false;

    [Tooltip("<color=#FF9800>■</color> 空弹后延迟销毁时间（秒）")]
    public float 空弹销毁延迟 = 1f;

    [Tooltip("<color=#FF9800>■</color> 无弹药时禁止尝试装填")]
    public bool 无弹时禁止装填 = false;

    [Tooltip("<color=#FF9800>■</color> 当前已装填的弹数")]
    public int 当前已装填弹数 = 0;
    #endregion

    #region 4. Position 面板 (青色分组)
    [Header("<color=#00BCD4>▶  武器位置与朝向</color>")]
    [Tooltip("<color=#00BCD4>■</color> 相对角色的附件偏移")]
    public Vector3 武器附件偏移 = Vector3.zero;

    [Tooltip("<color=#00BCD4>■</color> 角色翻转时同步武器翻转")]
    public bool 角色翻转时同步武器翻转 = true;

    [Tooltip("<color=#00BCD4>■</color> 右朝向时的翻转补偿值")]
    public Vector3 右朝向翻转值 = new Vector3(1, 1, 1);

    [Tooltip("<color=#00BCD4>■</color> 左朝向时的翻转补偿值")]
    public Vector3 左朝向翻转值 = new Vector3(-1, 1, 1);

    [Tooltip("<color=#00BCD4>■</color> 专用使用姿态变换点")]
    public Transform 武器使用变换点;

    [Tooltip("<color=#00BCD4>■</color> 武器是否应随角色翻转")]
    public bool 武器应翻转 = true;
    #endregion

    #region 5. IK 面板 (紫色分组)
    [Header("<color=#9C27B0>▶  反向动力学 IK</color>")]
    [Tooltip("<color=#9C27B0>■</color> 左手IK控制点")]
    public Transform 左手IK点;

    [Tooltip("<color=#9C27B0>■</color> 右手IK控制点")]
    public Transform 右手IK点;
    #endregion

    #region 6. Movement 面板 (黄色分组)
    [Header("<color=#FFEB3B>▶  使用时移动控制</color>")]
    [Tooltip("<color=#FFEB3B>■</color> 攻击时修改移动速度")]
    public bool 攻击时修改移动 = true;

    [Tooltip("<color=#FFEB3B>■</color> 移动速度倍率（0.5=50%速度）")]
    public float 移动速度倍率 = 默认移动倍率;

    [Tooltip("<color=#FFEB3B>■</color> 使用时完全禁止移动")]
    public bool 使用时完全禁止移动 = false;

    [Tooltip("<color=#FFEB3B>■</color> 使用时完全禁止瞄准")]
    public bool 使用时完全禁止瞄准 = false;
    #endregion

    #region 7. Recoil 面板 (红色分组)
    [Header("<color=#F44336>▶  后坐力</color>")]
    [Tooltip("<color=#F44336>■</color> 后坐力强度")]
    public float 后坐力 = 默认后坐力;
    #endregion

    #region 8. Animation 面板 (粉色分组)
    [Header("<color=#E91E63>▶  动画系统</color>")]
    [Tooltip("<color=#E91E63>■</color> 关联的Animator组件")]
    public Animator 动画控制器;

    [Tooltip("<color=#E91E63>■</color> 执行动画参数合法性检查")]
    public bool 执行动画检查 = false;

    [Tooltip("<color=#E91E63>■</color> 镜像角色动画参数")]
    public bool 镜像动画参数 = false;

    [Header("<color=#E91E63>▶ 动画参数映射</color>")]
    [Tooltip("<color=#E91E63>■</color> 武器动画ID")]
    public int 武器动画ID = 0;

    [Tooltip("<color=#E91E63>■</color> 待机动画参数名")]
    public string 待机动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 开始使用动画参数名")]
    public string 开始使用动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 使用前延迟动画参数名")]
    public string 使用前延迟动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 单次使用动画参数名")]
    public string 单次使用动画参数 = "SingleUse";

    [Tooltip("<color=#E91E63>■</color> 持续使用动画参数名")]
    public string 持续使用动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 间隔动画参数名")]
    public string 间隔动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 停止使用动画参数名")]
    public string 停止使用动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 开始装填动画参数名")]
    public string 开始装填动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 装填动画参数名")]
    public string 装填动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 停止装填动画参数名")]
    public string 停止装填动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 武器角度动画参数名")]
    public string 武器角度动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 相对武器角度动画参数名")]
    public string 相对武器角度动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 装备动画参数名")]
    public string 装备动画参数 = "";

    [Tooltip("<color=#E91E63>■</color> 中断动画参数名")]
    public string 中断动画参数 = "";
    #endregion

    #region 9. Feedbacks 面板 (深紫分组)
    [Header("<color=#673AB7>▶  事件反馈系统</color>")]
    [Tooltip("<color=#673AB7>■</color> 武器开始反馈")]
    public FeedbackItem 武器开始反馈;

    [Tooltip("<color=#673AB7>■</color> 武器使用/射击反馈")]
    public FeedbackItem 武器使用反馈;

    [Tooltip("<color=#673AB7>■</color> 武器使用备用反馈")]
    public FeedbackItem 武器使用备用反馈;

    [Tooltip("<color=#673AB7>■</color> 武器停止反馈")]
    public FeedbackItem 武器停止反馈;

    [Tooltip("<color=#673AB7>■</color> 武器换弹反馈")]
    public FeedbackItem 武器换弹反馈;

    [Tooltip("<color=#673AB7>■</color> 武器需要换弹反馈")]
    public FeedbackItem 武器需要换弹反馈;

    [Tooltip("<color=#673AB7>■</color> 武器无法换弹反馈")]
    public FeedbackItem 武器无法换弹反馈;
    #endregion

    #region 10. Settings 面板 (灰色分组)
    [Header("<color=#607D8B>▶  全局设置</color>")]
    [Tooltip("<color=#607D8B>■</color> 启动时自动初始化")]
    public bool 启动时初始化 = true;

    [Tooltip("<color=#607D8B>■</color> 是否可中断")]
    public bool 可中断 = false;

    [Tooltip("<color=#607D8B>■</color> 当前是否为翻转状态")]
    public bool 当前翻转 = true;
    #endregion

    #region 11. Projectiles 面板 (深蓝分组)
    [Header("<color=#3F51B5>🔹 抛射物系统</color>")]
    [Tooltip("<color=#3F51B5>■</color> 抛射物生成偏移")]
    public Vector3 抛射物生成偏移 = new Vector3(1.98f, 0.07f, 0f);

    [Tooltip("<color=#3F51B5>■</color> 默认抛射物发射方向")]
    public Vector3 默认抛射物方向 = new Vector3(0, 0, 1f);

    [Tooltip("<color=#3F51B5>■</color> 每发射出的子弹数量")]
    public int 每发射弹数 = 1;

    [Header("<color=#3F51B5>▶ 生成点设置</color>")]
    [Tooltip("<color=#3F51B5>■</color> 额外的生成点列表")]
    public List<Transform> 生成点列表 = new List<Transform>();

    [Tooltip("<color=#3F51B5>■</color> 生成点轮询模式")]
    public 生成点模式枚举 生成点模式 = 生成点模式枚举.顺序轮询;

    [Header("<color=#3F51B5>▶ 散布设置</color>")]
    [Tooltip("<color=#3F51B5>■</color> 散布范围")]
    public Vector3 散布范围 = new Vector3(0, 0, 10f);

    [Tooltip("<color=#3F51B5>■</color> 射击时武器随散布旋转")]
    public bool 散布时旋转武器 = true;

    [Tooltip("<color=#3F51B5>■</color> 启用随机散布")]
    public bool 启用随机散布 = true;

    [Header("<color=#3F51B5>▶ 对象池</color>")]
    [Tooltip("<color=#3F51B5>■</color> 子弹对象池")]
    public NativeObjectPooler 子弹对象池;

    [Header("<color=#3F51B5>▶ 生成反馈</color>")]
    [Tooltip("<color=#3F51B5>■</color> 生成时的反馈列表")]
    public List<FeedbackItem> 生成反馈列表 = new List<FeedbackItem>();
    #endregion

    #region 运行时状态
    // 内部运行状态
    private float _上次射击时间 = -999f;
    private int _当前生成点索引 = 0;
    private bool _正在装填 = false;
    private bool _正在射击 = false;
    #endregion

    #region 初始化
    private void Start()
    {
        bool 需要初始化 = 启动时初始化;
        if (需要初始化)
        {
            初始化武器();
        }
    }

    /// <summary>
    /// 初始化武器
    /// </summary>
    public void 初始化武器()
    {
        _上次射击时间 = -999f;
        _当前生成点索引 = 0;
        _正在装填 = false;
        _正在射击 = false;

        // 初始化对象池
        bool 对象池已配置 = 子弹对象池 != null;
        if (对象池已配置)
        {
            子弹对象池.初始化池();
        }
    }
    #endregion

    #region 核心射击逻辑
    /// <summary>
    /// 尝试射击
    /// </summary>
    [ContextMenu("测试：射击")]
    public void 尝试射击()
    {
        // 提取所有条件判断变量
        bool 武器未激活 = !当前激活;
        bool 输入未授权 = !输入授权;
        bool 正在装填中 = _正在装填;
        bool 冷却中 = Time.time - _上次射击时间 < 射击间隔;
        bool 弹药不足 = 检查弹药不足();

        if (武器未激活 || 输入未授权 || 正在装填中 || 冷却中 || 弹药不足)
        {
            return;
        }

        // 执行射击
        执行射击();
    }

    private bool 检查弹药不足()
    {
        bool 是弹匣武器 = 弹匣式武器;
        if (是弹匣武器)
        {
            return 当前已装填弹数 < 每发消耗弹药;
        }
        return false;
    }

    private void 执行射击()
    {
        _上次射击时间 = Time.time;
        _正在射击 = true;

        // 消耗弹药
        消耗弹药();

        // 生成子弹
        生成抛射物();

        // 播放反馈
        播放射击反馈();

        // 触发动画
        触发单次使用动画();

        // 应用后坐力
        应用后坐力();

        // 检查是否需要自动装填
        bool 需要自动装填 = 弹匣式武器 && 自动装填 && 当前已装填弹数 <= 0;
        if (需要自动装填)
        {
            开始装填();
        }
    }

    private void 消耗弹药()
    {
        bool 是弹匣武器 = 弹匣式武器;
        if (是弹匣武器)
        {
            当前已装填弹数 -= 每发消耗弹药;
        }
    }
    #endregion

    #region 抛射物生成
    private void 生成抛射物()
    {
        for (int i = 0; i < 每发射弹数; i++)
        {
            // 计算生成点
            Transform 生成点 = 获取当前生成点();

            // 计算方向和散布
            Vector3 最终方向 = 计算最终方向();

            // 从对象池获取子弹
            GameObject 子弹 = 子弹对象池.获取对象();
            if (子弹 == null)
                continue;

            // 设置子弹位置和方向
            初始化子弹(子弹, 生成点.position, 最终方向);
        }

        // 更新生成点索引
        更新生成点索引();
    }

    private Transform 获取当前生成点()
    {
        bool 有额外生成点 = 生成点列表.Count > 0;
        if (有额外生成点)
        {
            return 生成点列表[_当前生成点索引];
        }
        return transform;
    }

    private Vector3 计算最终方向()
    {
        Vector3 基础方向 = 默认抛射物方向;

        bool 启用随机 = 启用随机散布;
        if (启用随机)
        {
            // 添加随机散布
            float x = UnityEngine.Random.Range(-散布范围.x, 散布范围.x);
            float y = UnityEngine.Random.Range(-散布范围.y, 散布范围.y);
            float z = UnityEngine.Random.Range(-散布范围.z, 散布范围.z);
            基础方向 += new Vector3(x, y, z);
        }

        return 基础方向.normalized;
    }

    private void 初始化子弹(GameObject 子弹, Vector3 位置, Vector3 方向)
    {
        子弹.transform.position = 位置 + 抛射物生成偏移;
        子弹.transform.forward = 方向;
        子弹.SetActive(true);

        // 初始化子弹脚本
        NativeProjectile 子弹脚本 = 子弹.GetComponent<NativeProjectile>();
        if (子弹脚本 != null)
        {
            子弹脚本.初始化(方向, 子弹对象池);
        }
    }

    private void 更新生成点索引()
    {
        bool 有额外生成点 = 生成点列表.Count > 0;
        if (!有额外生成点)
            return;

        bool 顺序模式 = 生成点模式 == 生成点模式枚举.顺序轮询;
        if (顺序模式)
        {
            _当前生成点索引++;
            if (_当前生成点索引 >= 生成点列表.Count)
            {
                _当前生成点索引 = 0;
            }
        }
        else
        {
            // 随机模式
            _当前生成点索引 = UnityEngine.Random.Range(0, 生成点列表.Count);
        }
    }
    #endregion

    #region 装填逻辑
    /// <summary>
    /// 开始装填
    /// </summary>
    [ContextMenu("测试：装填")]
    public void 开始装填()
    {
        // 条件判断
        bool 正在装填中 = _正在装填;
        bool 弹匣已满 = 当前已装填弹数 >= 弹匣容量;
        bool 无弹禁止装填 = 无弹时禁止装填;

        if (正在装填中 || 弹匣已满 || 无弹禁止装填)
        {
            // 播放无法装填反馈
            播放无法装填反馈();
            return;
        }

        _正在装填 = true;
        StartCoroutine(装填协程());
    }

    private IEnumerator 装填协程()
    {
        // 播放装填开始反馈
        播放装填开始反馈();

        yield return new WaitForSeconds(装填耗时);

        // 完成装填
        当前已装填弹数 = 弹匣容量;
        _正在装填 = false;

        // 播放装填完成反馈
        播放装填完成反馈();
    }
    #endregion

    #region 反馈系统
    private void 播放射击反馈()
    {
        if (武器使用反馈 != null)
        {
            执行单个反馈(武器使用反馈);
        }
    }

    private void 播放装填开始反馈()
    {
        if (武器换弹反馈 != null)
        {
            执行单个反馈(武器换弹反馈);
        }
    }

    private void 播放无法装填反馈()
    {
        if (武器无法换弹反馈 != null)
        {
            执行单个反馈(武器无法换弹反馈);
        }
    }

    private void 播放装填完成反馈()
    {
        // 装填完成后播放需要换弹的反馈
        if (武器需要换弹反馈 != null)
        {
            执行单个反馈(武器需要换弹反馈);
        }
    }

    // 通用反馈执行逻辑
    private void 执行单个反馈(FeedbackItem 单个反馈)
    {
        if (单个反馈 == null)
            return;

        bool 有延迟 = 单个反馈.延迟 > 0;
        if (有延迟)
        {
            StartCoroutine(延迟执行反馈(单个反馈));
            return;
        }

        执行反馈内容(单个反馈);
    }

    private IEnumerator 延迟执行反馈(FeedbackItem 单个反馈)
    {
        yield return new WaitForSeconds(单个反馈.延迟);
        执行反馈内容(单个反馈);
    }

    private void 执行反馈内容(FeedbackItem 单个反馈)
    {
        bool 是粒子特效 = 单个反馈.目标粒子 != null;
        bool 是音效特效 = 单个反馈.音效片段 != null;
        bool 是生成物体 = 单个反馈.生成预制体 != null;

        if (是粒子特效)
        {
            播放粒子系统(单个反馈);
        }
        else if (是音效特效)
        {
            播放音效剪辑(单个反馈);
        }
        else if (是生成物体)
        {
            生成特效物体(单个反馈);
        }
    }

    private void 播放粒子系统(FeedbackItem 单个反馈)
    {
        单个反馈.目标粒子.Clear(true);
        单个反馈.目标粒子.Play(true);
    }

    private void 播放音效剪辑(FeedbackItem 单个反馈)
    {
        GameObject 临时音频物体 = new GameObject($"TempAudio_{单个反馈.特效名称}");
        临时音频物体.transform.position = transform.position;

        AudioSource 音频源 = 临时音频物体.AddComponent<AudioSource>();
        音频源.clip = 单个反馈.音效片段;
        音频源.volume = 单个反馈.音量;
        音频源.Play();

        Destroy(临时音频物体, 单个反馈.音效片段.length + 0.1f);
    }

    private void 生成特效物体(FeedbackItem 单个反馈)
    {
        Vector3 生成位置 = transform.position + 单个反馈.生成偏移;
        Quaternion 旋转偏移 = Quaternion.Euler(单个反馈.生成旋转偏移);
        Quaternion 生成旋转 = transform.rotation * 旋转偏移;

        GameObject 生成的物体 = Instantiate(单个反馈.生成预制体, 生成位置, 生成旋转);
        生成的物体.name = 单个反馈.特效名称;

        if (单个反馈.作为子物体)
        {
            生成的物体.transform.SetParent(transform);
        }

        bool 需要自动销毁 = 单个反馈.自动销毁时间 > 0;
        if (需要自动销毁)
        {
            Destroy(生成的物体, 单个反馈.自动销毁时间);
        }
    }
    #endregion

    #region 动画与后坐力
    private void 触发单次使用动画()
    {
        bool 动画控制器已配置 = 动画控制器 != null;
        bool 有单次使用参数 = !string.IsNullOrEmpty(单次使用动画参数);

        if (动画控制器已配置 && 有单次使用参数)
        {
            动画控制器.SetTrigger(单次使用动画参数);
        }
    }

    private void 应用后坐力()
    {
        // 这里可以根据你的角色控制器实现后坐力
        // 比如给角色添加反向的力，或者相机震动
        // 这里保留接口，你可以根据自己的项目扩展
    }
    #endregion

    #region 数据结构
    /// <summary>
    /// 单个反馈的配置项
    /// </summary>
    [Serializable]
    public class FeedbackItem
    {
        [Header("基础设置")]
        public string 特效名称 = "反馈";
        public float 延迟 = 0f;

        [Header("反馈类型")]
        public ParticleSystem 目标粒子;
        public AudioClip 音效片段;
        public float 音量 = 1f;
        public GameObject 生成预制体;

        [Header("生成设置")]
        public Vector3 生成偏移 = Vector3.zero;
        public Vector3 生成旋转偏移 = Vector3.zero;
        public bool 作为子物体 = true;
        public float 自动销毁时间 = 2f;
    }
    #endregion
}

#region 原生对象池实现 (替代MMSimpleObjectPooler)
/// <summary>
/// 纯原生对象池，替代原来的MMSimpleObjectPooler
/// </summary>
public class NativeObjectPooler : MonoBehaviour
{
    [Tooltip("要池化的预制体")]
    public GameObject 池化预制体;

    [Tooltip("池的初始大小")]
    public int 初始池大小 = 10;

    [Tooltip("是否可以动态扩展池")]
    public bool 允许动态扩展 = true;

    private Queue<GameObject> _对象池 = new Queue<GameObject>();
    private Transform _池父物体;

    public void 初始化池()
    {
        _池父物体 = new GameObject($"Pool_{池化预制体.name}").transform;
        _池父物体.SetParent(transform);

        // 预生成对象
        for (int i = 0; i < 初始池大小; i++)
        {
            创建新对象();
        }
    }

    private GameObject 创建新对象()
    {
        GameObject 新对象 = Instantiate(池化预制体, _池父物体);
        新对象.SetActive(false);
        _对象池.Enqueue(新对象);
        return 新对象;
    }

    public GameObject 获取对象()
    {
        if (_对象池.Count == 0)
        {
            bool 可以扩展 = 允许动态扩展;
            if (可以扩展)
            {
                return 创建新对象();
            }
            return null;
        }

        GameObject 对象 = _对象池.Dequeue();
        return 对象;
    }

    public void 回收对象(GameObject 对象)
    {
        对象.SetActive(false);
        _对象池.Enqueue(对象);
    }
}

#region 原生子弹脚本 (替代原来的Projectile)
/// <summary>
/// 纯原生子弹脚本，替代原来的Projectile
/// </summary>
public class NativeProjectile : MonoBehaviour
{
    [Tooltip("子弹飞行速度")]
    public float 飞行速度 = 20f;

    [Tooltip("子弹伤害")]
    public int 伤害 = 10;

    [Tooltip("子弹存活时间")]
    public float 存活时间 = 2f;

    private Vector3 _方向;
    private float _出生时间;
    private NativeObjectPooler _所属池;

    public void 初始化(Vector3 方向, NativeObjectPooler 池 = null)
    {
        // 修复了这里的笔误，原来写错成了direction
        _方向 = 方向;
        _出生时间 = Time.time;
        _所属池 = 池;
    }

    private void Update()
    {
        // 移动
        transform.Translate(_方向 * 飞行速度 * Time.deltaTime, Space.World);

        // 超时回收
        bool 已超时 = Time.time - _出生时间 > 存活时间;
        if (已超时)
        {
            回收();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 处理碰撞
        回收();
    }

    private void 回收()
    {
        if (_所属池 != null)
        {
            _所属池.回收对象(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
#endregion
#endregion
