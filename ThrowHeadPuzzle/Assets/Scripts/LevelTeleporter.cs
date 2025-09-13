using UnityEngine;

public class LevelTeleporter : MonoBehaviour
{
    // ========== 在Unity编辑器中可设置的参数 ==========
    [Header("传送设置")]
    [Tooltip("要传送到的关卡编号（1,2,3...）")]
    public int 目标关卡 = 1; // 这个传送门会传送到哪个关卡

    [Tooltip("与传送门交互的按键")]
    public KeyCode interactKey = KeyCode.E; // 按哪个键进行传送

    [Header("关卡数据")]
    [Tooltip("拖拽这里分配创建好的LevelDataSO文件")]
    public LevelDataSO levelData; // 引用上面创建的关卡数据文件

    // ========== 私有变量（不在编辑器中显示） ==========
    private bool playerInRange = false; // 记录玩家是否在传送门范围内

    // ========== 碰撞检测相关方法 ==========

    // 当有其他物体进入这个碰撞器时调用（需要碰撞器设置为Trigger）
    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的物体是否是玩家（玩家需要设置Tag为"Player"）
        if (other.CompareTag("Player"))
        {
            playerInRange = true; // 标记玩家在范围内
            Debug.Log($"按 {interactKey} 键进入关卡 {目标关卡}");
        }
    }

    // 当有其他物体离开这个碰撞器时调用
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // 标记玩家离开范围
        }
    }

    // ========== 每帧更新的逻辑 ==========
    void Update()
    {
        // 如果玩家在范围内且按下了交互键
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            TeleportToLevel(); // 执行传送
        }
    }

    // ========== 传送功能的核心方法 ==========
    public void TeleportToLevel()
    {
        // 安全检查：确保关卡数据已分配
        if (levelData == null)
        {
            Debug.LogError("错误：LevelDataSO 未分配！请在Inspector中拖拽分配关卡数据文件");
            return; // 退出方法，不执行后面的代码
        }

        // 获取目标关卡的信息
        var levelInfo = levelData.GetLevelInfo(目标关卡);

        // 检查是否找到了关卡数据
        if (levelInfo == null)
        {
            Debug.LogError($"错误：未找到关卡 {目标关卡} 的数据");
            return;
        }

        // 执行传送玩家到指定位置
        TeleportPlayer(levelInfo.startPosition);

        // 显示成功信息
        Debug.Log($"已传送到关卡 {目标关卡} - {levelInfo.levelName}");
    }

    // ========== 实际的传送逻辑 ==========
    protected virtual void TeleportPlayer(Vector2 position)
    {
        // 在场景中查找带有"Player"标签的物体（玩家角色）
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // 确保找到了玩家物体
        if (player != null)
        {
            // 将玩家传送到目标位置
            // 获取玩家当前的z坐标（保持原有深度）
            float currentZ = player.transform.position.z;

            // 设置新位置，保持z坐标不变
            player.transform.position = new Vector3(position.x, position.y, currentZ);
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
