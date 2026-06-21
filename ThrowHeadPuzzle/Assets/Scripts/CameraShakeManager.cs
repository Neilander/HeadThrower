using Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    #region 常量定义
    // 避免魔法数字，定义默认的震动速度参数
    private const float 默认震动速度 = 1.0f;
    #endregion

    #region 序列化字段
    // 按功能分组：核心组件引用
    [Section("核心组件引用")]
    [Tooltip("需要在此处赋值Cinemachine Impulse Source组件，若未赋值脚本将尝试自动获取")]
    [SerializeField]
    private CinemachineImpulseSource 震动源;

    // 按功能分组：震动参数配置
    [Section("震动参数配置")]
    [Tooltip("是否在游戏启动时进行一次测试震动")]
    [SerializeField]
    private bool 启动时测试震动 = false;
    #endregion

    #region 单例模式
    // 静态实例，方便全局访问
    public static CameraShakeManager 实例 { get; private set; }
    #endregion

    /// <summary>
    /// Unity生命周期：初始化
    /// </summary>
    private void Awake()
    {
        初始化单例();
        初始化组件引用();
    }

    /// <summary>
    /// Unity生命周期：开始
    /// </summary>
    private void Start()
    {
        处理启动测试();
    }

    /// <summary>
    /// 初始化单例模式，确保全局唯一
    /// </summary>
    private void 初始化单例()
    {
        bool 实例已存在 = 实例 != null && 实例 != this;
        if (实例已存在)
        {
            Destroy(gameObject);
            return;
        }

        实例 = this;
    }

    /// <summary>
    /// 初始化组件引用，若未手动赋值则自动获取
    /// </summary>
    private void 初始化组件引用()
    {
        bool 震动源未赋值 = 震动源 == null;
        if (震动源未赋值)
        {
            震动源 = GetComponent<CinemachineImpulseSource>();
        }

        // 再次检查，确保组件存在
        bool 依然为空 = 震动源 == null;
        if (依然为空)
        {
            Debug.LogError(
                "错误：物体上未找到 CinemachineImpulseSource 组件，震动功能将无法使用！",
                gameObject
            );
        }
    }

    /// <summary>
    /// 处理启动时的测试逻辑
    /// </summary>
    private void 处理启动测试()
    {
        bool 需要测试 = 启动时测试震动;
        if (需要测试)
        {
            触发默认震动();
        }
    }

    /// <summary>
    /// 公共方法：触发一次默认参数的摄像机震动
    /// </summary>
    public void 触发默认震动()
    {
        // 提取条件：检查震动源是否有效
        bool 震动源有效 = 震动源 != null;
        if (震动源有效)
        {
            // 使用默认速度生成震动
            震动源.GenerateImpulseWithVelocity(new Vector3(默认震动速度, 默认震动速度, 0));
        }
    }

    /// <summary>
    /// 公共方法：根据外部传入的力度触发摄像机震动
    /// </summary>
    /// <param name="震动力度">震动的强度系数</param>
    public void 触发震动(float 震动力度)
    {
        // 提取条件：检查震动源是否有效
        bool 震动源有效 = 震动源 != null;
        if (震动源有效)
        {
            // 根据力度计算最终速度向量
            Vector3 震动速度向量 = new Vector3(震动力度, 震动力度, 0);
            震动源.GenerateImpulseWithVelocity(震动速度向量);
        }
    }
}
