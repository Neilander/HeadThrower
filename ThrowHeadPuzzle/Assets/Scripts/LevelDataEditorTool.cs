using UnityEditor;
using UnityEngine;

public class LevelDataEditorTool : EditorWindow
{
    private LevelDataSO levelData;
    private int selectedLevelIndex = 0;

    // 添加菜单项
    [MenuItem("Tools/关卡数据编辑器")]
    public static void ShowWindow()
    {
        GetWindow<LevelDataEditorTool>("关卡数据编辑器");
    }

    void OnGUI()
    {
        GUILayout.Label("🚀 关卡出生点坐标设置工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 选择LevelDataSO文件
        levelData = (LevelDataSO)
            EditorGUILayout.ObjectField("关卡数据文件", levelData, typeof(LevelDataSO), false);

        if (levelData == null)
        {
            EditorGUILayout.HelpBox("请先选择一个LevelDataSO文件", MessageType.Info);
            return;
        }

        if (levelData.关卡 == null || levelData.关卡.Length == 0)
        {
            EditorGUILayout.HelpBox("请先在LevelDataSO中设置关卡数组", MessageType.Warning);
            return;
        }

        // 选择要设置的关卡
        string[] levelOptions = new string[levelData.关卡.Length];
        for (int i = 0; i < levelData.关卡.Length; i++)
        {
            levelOptions[i] =
                $"关卡 {levelData.关卡[i].levelNumber}: {levelData.关卡[i].levelName}";
        }

        selectedLevelIndex = EditorGUILayout.Popup("选择关卡", selectedLevelIndex, levelOptions);
        EditorGUILayout.Space();

        // 显示当前坐标
        Vector3 currentPosition = levelData.关卡[selectedLevelIndex].spawnPosition;
        EditorGUILayout.LabelField("当前坐标:", $"{currentPosition}");
        EditorGUILayout.Space();

        // 从选中物体获取坐标的按钮
        if (Selection.activeGameObject != null)
        {
            if (
                GUILayout.Button(
                    $"📌 从选中物体获取坐标: {Selection.activeGameObject.name}",
                    GUILayout.Height(40)
                )
            )
            {
                SetPositionFromSelection();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("请在场景中选择一个物体作为出生点", MessageType.Info);
        }

        EditorGUILayout.Space();

        // 手动输入坐标
        EditorGUILayout.LabelField("或者手动输入坐标:");
        levelData.关卡[selectedLevelIndex].spawnPosition = EditorGUILayout.Vector3Field(
            "坐标",
            levelData.关卡[selectedLevelIndex].spawnPosition
        );

        EditorGUILayout.Space();

        // 保存按钮
        if (GUILayout.Button("💾 保存数据", GUILayout.Height(30)))
        {
            SaveData();
        }
    }

    /// <summary>
    /// 从选中物体获取坐标
    /// </summary>
    private void SetPositionFromSelection()
    {
        if (Selection.activeGameObject == null)
            return;

        Vector3 worldPosition = Selection.activeGameObject.transform.position;
        levelData.关卡[selectedLevelIndex].spawnPosition = worldPosition;

        Debug.Log(
            $"已设置关卡 {levelData.关卡[selectedLevelIndex].levelNumber} 的出生点坐标: {worldPosition}"
        );
        Repaint(); // 刷新窗口显示
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    private void SaveData()
    {
        EditorUtility.SetDirty(levelData);
        AssetDatabase.SaveAssets();
        Debug.Log("关卡数据已保存");
    }
}
