using UnityEngine;
using UnityEngine.InputSystem;

public class LevelTeleporter : MonoBehaviour
{
    #region 公共变量
    [Tooltip("传送时的位置偏移量\n例如：(0, 0.5, 0) 会在目标位置上方0.5单位传送")]
    public Vector3 位置偏移 = Vector3.zero;

    [Header("传送设置")]
    [Tooltip("要传送到的关卡编号（1,2,3...）")]
    public int 目标关卡 = 1;

    [Tooltip("与传送门交互的按键")]
    public KeyCode 交互按键 = KeyCode.E;

    [Header("关卡数据")]
    [Tooltip("拖拽这里分配创建好的LevelDataSO文件")]
    public LevelDataSO 关卡数据;

    [Header("按键提示文件")]
    public DeliverBoolSO 提示图标布尔;
    #endregion

    #region 私有变量
    private bool 玩家在范围内 = false;
    private InputControl 输入控制;
    #endregion

    #region 生命周期

    private void Awake()
    {
        输入控制 = new InputControl();
    }

    private void OnEnable()
    {
        输入控制.Enable();
    }

    private void OnDisable()
    {
        输入控制.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool 是玩家 = other.CompareTag("Player");
        if (是玩家)
        {
            玩家在范围内 = true;
            提示图标布尔.RaiseEvent(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        bool 是玩家 = other.CompareTag("Player");
        if (是玩家)
        {
            玩家在范围内 = false;
            提示图标布尔.RaiseEvent(false);
        }
    }

    private void Update()
    {
        bool 按下交互键 = 输入控制.Player.Interact.triggered;
        bool 满足条件 = 玩家在范围内 && 按下交互键;
        
        if (满足条件)
        {
            执行传送();
        }
    }

    #endregion

    #region 传送逻辑

    public void 执行传送()
    {
        bool 有关卡数据 = 关卡数据 != null;
        if (!有关卡数据)
        {
            Debug.LogError("错误：关卡数据未分配！请在Inspector中拖拽分配关卡数据文件");
            return;
        }

        var 关卡信息 = 关卡数据.GetLevelInfo(目标关卡);
        bool 有关卡信息 = 关卡信息 != null;
        
        if (!有关卡信息)
        {
            Debug.LogError($"错误：未找到关卡 {目标关卡} 的数据");
            return;
        }

        传送玩家(关卡信息.spawnPosition);
    }

    protected virtual void 传送玩家(Vector2 位置)
    {
        GameObject 玩家 = GameObject.FindGameObjectWithTag("Player");
        bool 有玩家 = 玩家 != null;
        
        if (有玩家)
        {
            float 当前Z = 玩家.transform.position.z;
            玩家.transform.position = new Vector3(
                位置.x + 位置偏移.x,
                位置.y + 位置偏移.y,
                当前Z
            );
        }
        else
        {
            Debug.LogWarning("未找到玩家物体！请确保玩家设置了'Player'标签");
        }
    }

    #endregion

    #region 编辑器工具

    [ContextMenu("测试传送功能")]
    void 测试传送功能()
    {
        Debug.Log("测试传送功能...");
        执行传送();
    }

    #endregion
}
