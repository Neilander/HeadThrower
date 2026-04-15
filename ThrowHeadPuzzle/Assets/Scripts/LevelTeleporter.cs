using UnityEngine;

public class LevelTeleporter : MonoBehaviour
{
    // 重构说明：
    // 1) 将魔法数字提取为语义化常量。
    // 2) 将所有 if 控制流条件提取为上一行 bool 变量。
    // 3) 优先将可安全重命名的英文变量改为中文语义命名。

    private const int 默认关卡编号 = 1;

    // ========== 在Unity编辑器中可设置的参数 ==========

    [Tooltip("传送时的位置偏移量\n" + "例如：(0, 0.5, 0) 会在目标位置上方0.5单位传送")]
    public Vector3 positionOffset = Vector3.zero;

    [Header("传送设置")]
    [Tooltip("要传送到的关卡编号（1,2,3...）")]
    public int 目标关卡 = 默认关卡编号; // 这个传送门会传送到哪个关卡

    [Tooltip("与传送门交互的按键")]
    public KeyCode interactKey = KeyCode.T; // 按哪个键进行传送

    [Header("关卡数据")]
    [Tooltip("拖拽这里分配创建好的LevelDataSO文件")]
    public LevelDataSO levelData; // 引用上面创建的关卡数据文件

    [Header("按键提示文件")]
    public DeliverBoolSO TSignBoolSO;

    // ========== 私有变量（不在编辑器中显示） ==========
    private bool 玩家在传送范围内 = false; // 记录玩家是否在传送门范围内

    // ========== 碰撞检测相关方法 ==========

    // 当有其他物体进入这个碰撞器时调用（需要碰撞器设置为Trigger）
    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的物体是否是玩家（玩家需要设置Tag为"Player"）
        bool 进入的是玩家 = other.CompareTag("Player");
        if (进入的是玩家)
        {
            玩家在传送范围内 = true; // 标记玩家在范围内
            Debug.Log($"按 {interactKey} 键进入关卡 {目标关卡}");
            TSignBoolSO.RaiseEvent(true); //显示按键提示
        }
    }

    // 当有其他物体离开这个碰撞器时调用
    void OnTriggerExit2D(Collider2D other)
    {
        bool 离开的是玩家 = other.CompareTag("Player");
        if (离开的是玩家)
        {
            玩家在传送范围内 = false; // 标记玩家离开范围
            TSignBoolSO.RaiseEvent(false); //隐藏按键提示
        }
    }

    // ========== 每帧更新的逻辑 ==========
    void Update()
    {
        // 如果玩家在范围内且按下了交互键
        bool 玩家在范围内且按下交互键 = 玩家在传送范围内 && Input.GetKeyDown(interactKey);
        if (玩家在范围内且按下交互键)
        {
            TeleportToLevel(); // 执行传送
        }
    }

    // ========== 传送功能的核心方法 ==========
    public void TeleportToLevel()
    {
        // 安全检查：确保关卡数据已分配
        bool 关卡数据已分配 = levelData != null;
        if (!关卡数据已分配)
        {
            Debug.LogError("错误：LevelDataSO 未分配！请在Inspector中拖拽分配关卡数据文件");
            return; // 退出方法，不执行后面的代码
        }

        // 获取目标关卡的信息
        var 目标关卡信息 = levelData.GetLevelInfo(目标关卡);

        // 检查是否找到了关卡数据
        bool 找到目标关卡数据 = 目标关卡信息 != null;
        if (!找到目标关卡数据)
        {
            Debug.LogError($"错误：未找到关卡 {目标关卡} 的数据");
            return;
        }

        // 执行传送玩家到指定位置
        TeleportPlayer(目标关卡信息.spawnPosition);

        // 显示成功信息
        //Debug.Log($"已传送到关卡 {目标关卡} - {levelInfo.levelName}");
    }

    // ========== 实际的传送逻辑 ==========
    protected virtual void TeleportPlayer(Vector2 position)
    {
        // 在场景中查找带有"Player"标签的物体（玩家角色）
        GameObject 玩家物体 = GameObject.FindGameObjectWithTag("Player");

        // 确保找到了玩家物体
        bool 找到玩家物体 = 玩家物体 != null;
        if (找到玩家物体)
        {
            //先将玩家的头组合起来再传送
            PlayerController 玩家控制器 = 玩家物体.GetComponent<PlayerController>();
            bool 找到玩家控制器 = 玩家控制器 != null;
            if (找到玩家控制器)
            {
                GameObject 默认头部物体 = 玩家控制器.默认头部;
                bool 已配置默认头部物体 = 默认头部物体 != null;
                if (已配置默认头部物体)
                {
                    PickUpHead 默认头部拾取组件 = 默认头部物体.GetComponent<PickUpHead>();
                    bool 找到默认头部拾取组件 = 默认头部拾取组件 != null;
                    if (找到默认头部拾取组件)
                    {
                        // 复用捡头逻辑：移除刚体 -> 设为玩家子对象 -> 还原局部位姿。
                        InteractionSignal 头部拼接信号 = new InteractionSignal(玩家物体, InteractionType.KeyPress);
                        默认头部拾取组件.OnInteract(头部拼接信号);

                        // 利用状态机同步到“头在身上”状态，避免状态与层级不一致。
                        bool 当前不是头在身上状态 = 玩家控制器.curstate != PlayerController.ThrowState.HeadOnBody;
                        if (当前不是头在身上状态)
                        {
                            玩家控制器.StartState(PlayerController.ThrowState.HeadOnBody);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("默认头部物体上缺少 PickUpHead 组件，已跳过头部拼接");
                    }
                }
                else
                {
                    Debug.LogWarning("PlayerController.默认头部 未配置，已跳过头部拼接");
                }
            }


            // 将玩家传送到目标位置
            // 获取玩家当前的z坐标（保持原有深度）
            float 当前Z坐标 = 玩家物体.transform.position.z;

            // 设置新位置，保持z坐标不变
            玩家物体.transform.position = new Vector3(
                position.x + positionOffset.x,
                position.y + positionOffset.y,
                当前Z坐标
            );
            // 可选：添加传送特效或声音
            // Instantiate(teleportEffect, position, Quaternion.identity);
            // AudioManager.PlaySound("TeleportSound");
        }
        else
        {
            Debug.LogWarning("未找到玩家物体！请确保玩家设置了'Player'标签");
        }
    }

    // ========== 在Unity编辑器中右键菜单的实用功能 ==========
    [ContextMenu("测试传送功能")]
    void TestTeleport()
    {
        // 在编辑模式下测试传送功能
        Debug.Log("测试传送功能...");
        TeleportToLevel();
    }
}
