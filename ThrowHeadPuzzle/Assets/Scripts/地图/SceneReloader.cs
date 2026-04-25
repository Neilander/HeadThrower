using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    // 目标场景名称（需与Build Settings中的名称一致）
    [Section("重置参数配置")]
    public string targetSceneName = "Level";
    public KeyCode 重置按键 = KeyCode.R;
    public Vector2 传送点坐标 = new Vector2(0, 0);

    [Tooltip("传送时的位置偏移量\n" + "例如：(0, 0.5, 0) 会在目标位置上方0.5单位传送")]
    public Vector3 positionOffset = Vector3.zero;

    [SerializeField]
    private DeliverintSO 当前关卡值SO;

    [SerializeField]
    private int 当前关卡ID = 0;
    public LevelDataSO levelData; // 引用上面创建的关卡数据文件

    /// <summary>
    /// 初始化时订阅
    /// </summary>
    private void Awake()
    {
        当前关卡值SO.订阅事件(更新当前关卡); // 订阅事件
    }

    /// <summary>
    /// 销毁时取消订阅
    /// </summary>
    private void OnDestroy()
    {
        当前关卡值SO.取消订阅事件(更新当前关卡); // 取消订阅
    }

    private void Update()
    {
        // 按下R键触发重新加载
        if (Input.GetKeyDown(重置按键))
        {
            StartCoroutine(ReloadTargetScene());
            TeleportToLevel();
        }
    }

    private void 更新当前关卡(int 关卡ID)
    {
        当前关卡ID = 关卡ID;
    }

    // 协程：先卸载再重新加载场景（确保异步操作顺序执行）
    private IEnumerator ReloadTargetScene()
    {
        // 1. 查找当前已加载的"Level"场景
        Scene targetScene = SceneManager.GetSceneByName(targetSceneName);

        if (!targetScene.isLoaded)
        {
            Debug.LogWarning($"场景 {targetSceneName} 未加载，无需重新加载");
            yield break;
        }

        // 2. 异步卸载"Level"场景（不影响其他场景）
        yield return SceneManager.UnloadSceneAsync(targetScene);
        Debug.Log($"场景 {targetSceneName} 已卸载");

        // 3. 异步重新加载"Level"场景（additive模式，共存于当前场景集合）
        yield return SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        Debug.Log($"场景 {targetSceneName} 已重新加载");
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
        var 目标关卡信息 = levelData.GetLevelInfo(当前关卡ID);

        // 检查是否找到了关卡数据
        bool 找到目标关卡数据 = 目标关卡信息 != null;
        if (!找到目标关卡数据)
        {
            //Debug.LogError($"错误：未找到关卡 {目标关卡} 的数据");
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
                        InteractionSignal 头部拼接信号 = new InteractionSignal(
                            玩家物体,
                            InteractionType.KeyPress
                        );
                        默认头部拾取组件.OnInteract(头部拼接信号);

                        // 利用状态机同步到“头在身上”状态，避免状态与层级不一致。
                        bool 当前不是头在身上状态 =
                            玩家控制器.curstate != PlayerController.ThrowState.HeadOnBody;
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
}
