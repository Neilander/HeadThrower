using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemDataEditorTool : EditorWindow
{
    private DeliverStringSO itemData;
    private Vector2 scrollPosition;
    private DeliverStringSO.ObjectData newItem = new DeliverStringSO.ObjectData();
    private string searchKeyword = "";
    private int selectedItemIndex = -1;

    // 添加菜单项
    [MenuItem("Tools/物品数据编辑器")]
    public static void ShowWindow()
    {
        GetWindow<ItemDataEditorTool>("物品数据编辑器");
    }

    void OnGUI()
    {
        GUILayout.Label("📦 物品数据管理工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 选择数据文件
        itemData = (DeliverStringSO)
            EditorGUILayout.ObjectField("物品数据文件", itemData, typeof(DeliverStringSO), false);

        if (itemData == null)
        {
            EditorGUILayout.HelpBox("请先选择一个DeliverStringSO文件", MessageType.Info);

            if (GUILayout.Button("创建新物品数据文件"))
            {
                CreateNewDataFile();
            }
            return;
        }

        // 数据列表为空时的提示
        if (itemData.objects == null || itemData.objects.Count == 0)
        {
            EditorGUILayout.HelpBox("当前数据文件为空，可添加新物品", MessageType.Warning);
        }
        else
        {
            // 搜索功能
            searchKeyword = EditorGUILayout.TextField("搜索物品", searchKeyword);
            EditorGUILayout.Space();

            // 物品列表显示
            GUILayout.Label($"物品列表 ({itemData.objects.Count})", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));

            for (int i = 0; i < itemData.objects.Count; i++)
            {
                var item = itemData.objects[i];
                // 搜索过滤
                if (
                    !string.IsNullOrEmpty(searchKeyword)
                    && !item.objectName.ToLower().Contains(searchKeyword.ToLower())
                    && !item.objectID.ToString().Contains(searchKeyword)
                )
                {
                    continue;
                }

                // 选中状态高亮
                bool isSelected = (selectedItemIndex == i);
                var bgColor = isSelected ? new Color(0.6f, 0.8f, 1f) : GUI.backgroundColor;
                GUI.backgroundColor = bgColor;

                using (new EditorGUILayout.VerticalScope("box"))
                {
                    // 点击行选中物品
                    if (
                        GUILayout.Button(
                            $"{item.objectName} (ID: {item.objectID})",
                            EditorStyles.label
                        )
                    )
                    {
                        selectedItemIndex = i;
                    }

                    // 显示选中物品的详细信息
                    if (isSelected)
                    {
                        EditorGUILayout.Space();
                        EditorGUI.indentLevel++;

                        item.objectName = EditorGUILayout.TextField("物品名称", item.objectName);
                        item.objectID = EditorGUILayout.IntField("物品ID", item.objectID);
                        item.objectType = (ObjectType)
                            EditorGUILayout.EnumPopup("物品类型", item.objectType);

                        EditorGUILayout.Space();
                        item.description = EditorGUILayout.TextArea(
                            item.description,
                            GUILayout.Height(60),
                            GUILayout.Width(300)
                        );

                        EditorGUILayout.Space();
                        item.objectSprite = (Sprite)
                            EditorGUILayout.ObjectField(
                                "物品图标",
                                item.objectSprite,
                                typeof(Sprite),
                                false
                            );

                        EditorGUILayout.Space();
                        item.value = EditorGUILayout.IntField("价值", item.value);
                        item.isStackable = EditorGUILayout.Toggle("可堆叠", item.isStackable);
                        item.weight = EditorGUILayout.FloatField("重量", item.weight);

                        EditorGUILayout.Space();
                        if (GUILayout.Button("删除此物品"))
                        {
                            if (
                                EditorUtility.DisplayDialog(
                                    "确认删除",
                                    $"确定要删除 {item.objectName} 吗？",
                                    "删除",
                                    "取消"
                                )
                            )
                            {
                                itemData.objects.RemoveAt(i);
                                selectedItemIndex = -1;
                                MarkAsDirty();
                                break;
                            }
                        }

                        EditorGUI.indentLevel--;
                    }
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();
        GUILayout.Label("➕ 添加新物品", EditorStyles.boldLabel);
        using (new EditorGUILayout.VerticalScope("box"))
        {
            newItem.objectName = EditorGUILayout.TextField("物品名称", newItem.objectName);
            newItem.objectID = EditorGUILayout.IntField("物品ID", newItem.objectID);
            newItem.objectType = (ObjectType)
                EditorGUILayout.EnumPopup("物品类型", newItem.objectType);

            if (GUILayout.Button("添加到列表"))
            {
                AddNewItem();
            }
        }

        EditorGUILayout.Space();
        // 底部操作按钮
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("清空所有数据"))
            {
                if (
                    EditorUtility.DisplayDialog(
                        "警告",
                        "确定要清空所有物品数据吗？",
                        "确认",
                        "取消"
                    )
                )
                {
                    itemData.ClearAllData();
                    selectedItemIndex = -1;
                    MarkAsDirty();
                }
            }

            if (GUILayout.Button("💾 保存数据"))
            {
                SaveData();
            }
        }
    }

    /// <summary>
    /// 创建新的数据文件
    /// </summary>
    private void CreateNewDataFile()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "创建新物品数据",
            "New ItemData",
            "asset",
            "请输入文件名"
        );

        if (!string.IsNullOrEmpty(path))
        {
            DeliverStringSO newData = CreateInstance<DeliverStringSO>();
            AssetDatabase.CreateAsset(newData, path);
            AssetDatabase.SaveAssets();
            itemData = newData;
            Debug.Log("已创建新物品数据文件");
        }
    }

    /// <summary>
    /// 添加新物品
    /// </summary>
    private void AddNewItem()
    {
        if (string.IsNullOrEmpty(newItem.objectName))
        {
            EditorUtility.DisplayDialog("错误", "物品名称不能为空", "确定");
            return;
        }

        if (itemData.ContainsObject(newItem.objectName))
        {
            EditorUtility.DisplayDialog("错误", "已存在同名物品", "确定");
            return;
        }

        if (itemData.ContainsObject(newItem.objectID))
        {
            EditorUtility.DisplayDialog("错误", "已存在相同ID的物品", "确定");
            return;
        }

        itemData.AddObject(
            new DeliverStringSO.ObjectData()
            {
                objectName = newItem.objectName,
                objectID = newItem.objectID,
                objectType = newItem.objectType,
                description = newItem.description,
                objectSprite = newItem.objectSprite,
                value = newItem.value,
                isStackable = newItem.isStackable,
                weight = newItem.weight,
            }
        );

        // 重置输入框
        newItem = new DeliverStringSO.ObjectData();
        MarkAsDirty();
    }

    /// <summary>
    /// 标记数据为已修改
    /// </summary>
    private void MarkAsDirty()
    {
        EditorUtility.SetDirty(itemData);
        Repaint();
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    private void SaveData()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("提示", "物品数据已保存", "确定");
    }
}
