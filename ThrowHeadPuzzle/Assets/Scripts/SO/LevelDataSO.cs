using UnityEngine;

// 创建菜单选项：在Unity编辑器中右键 -> Create -> Game -> Level Data
[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelDataSO : ScriptableObject
{
    // 定义一个可序列化的类来存储每个关卡的信息
    [System.Serializable]
    public class LevelInfo
    {
        [Tooltip("关卡编号（1,2,3...）")]
        public int levelNumber; // 关卡的数字标识

        [Tooltip("关卡名称（如：森林关卡、沙漠关卡）")]
        public string levelName; // 关卡的显示名称

        [Tooltip("该关卡的玩家出生点坐标")]
        public Vector2 startPosition; // 玩家传送到这个关卡时出现的位置

        [Tooltip("如果需要跨场景传送，填写场景名称")]
        public string sceneName; // 如果关卡在另一个场景，这里填场景名字
    }

    // 存储所有关卡信息的数组
    public LevelInfo[] 关卡;

    // 根据关卡编号获取关卡信息的方法
    public LevelInfo GetLevelInfo(int levelNumber)
    {
        // 遍历所有关卡
        foreach (var level in 关卡)
        {
            // 如果找到匹配的关卡编号
            if (level.levelNumber == levelNumber)
            {
                return level; // 返回这个关卡的信息
            }
        }

        // 如果没有找到，返回空并显示警告
        Debug.LogWarning($"未找到编号为 {levelNumber} 的关卡数据");
        return null;
    }
}
