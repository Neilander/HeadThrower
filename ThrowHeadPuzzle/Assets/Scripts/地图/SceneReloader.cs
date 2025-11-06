using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    // 目标场景名称（需与Build Settings中的名称一致）
    public string targetSceneName = "Level";

    private void Update()
    {
        // 按下R键触发重新加载
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadTargetScene());
        }
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
    // private InputAction _reloadAction;
    // 定义一个存储要重新加载的场景索引的数组
    // private int[] _sceneIndicesToReload = { 1, 2, 3 }; // 这里假设要重新加载索引为0、1、2的场景，你可以根据实际情况修改


    // private void Awake()
    // {
    //     DontDestroyOnLoad(gameObject);
    //     _reloadAction = new InputAction("Reload", binding: "<Keyboard>/r");
    //     _reloadAction.performed += ctx => StartCoroutine(ReloadScenes());
    // }

    // private IEnumerator ReloadScenes()
    // {
    //     int sceneCount = SceneManager.sceneCountInBuildSettings;
    //     foreach (int sceneIndex in _sceneIndicesToReload)
    //     {
    //         if (sceneIndex < 0 || sceneIndex >= sceneCount)
    //         {
    //             Debug.LogError($"无效的场景索引: {sceneIndex}，总场景数量: {sceneCount}");
    //             continue;
    //         }

    //         // 检查场景是否已经加载
    //         Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
    //         if (scene.isLoaded)
    //         {
    //             Debug.Log($"开始卸载场景，场景索引: {sceneIndex}");
    //             // 异步卸载场景
    //             AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneIndex);
    //             // 等待卸载完成
    //             while (!unloadOperation.isDone)
    //             {
    //                 yield return null;
    //             }
    //             Debug.Log($"场景卸载完成，场景索引: {sceneIndex}");
    //         }

    //         Debug.Log($"开始加载场景，场景索引: {sceneIndex}");
    //         // 异步加载场景，使用 Additive 模式
    //         AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
    //         // 等待加载完成
    //         while (!loadOperation.isDone)
    //         {
    //             yield return null;
    //         }
    //         Debug.Log($"场景加载完成，场景索引: {sceneIndex}");
    //     }
    // }

    // private void OnEnable() => _reloadAction.Enable();
    // private void OnDisable() => _reloadAction.Disable();
}
