using System.Collections.Generic;
using UnityEngine;

// 定义物品类型枚举
public enum ObjectType
{
    Item,
    NPC,
    Environment,
    Weapon,
    Consumable,
}

// 创建菜单选项
[CreateAssetMenu(fileName = "New DeliverStringSO", menuName = "数据/物品文字描述数据SO")]
public class DeliverStringSO : ScriptableObject
{
    [System.Serializable]
    public class ObjectData
    {
        [Header("物体基本信息")]
        public string objectName; // 物体名称
        public int objectID; // 物体ID
        public ObjectType objectType; // 物体类型

        [Header("描述信息")]
        [TextArea(3, 8)]
        public string description; // 详细描述

        [Header("视觉资源")]
        public Sprite objectSprite; // 物体精灵图

        [Header("其他元数据")]
        public int value; // 价值
        public bool isStackable = true; // 是否可堆叠
        public float weight; // 重量
    }

    [Header("物体数据列表")]
    public List<ObjectData> objects = new List<ObjectData>();

    // 用于快速查找的字典
    private Dictionary<string, ObjectData> objectByNameDictionary;
    private Dictionary<int, ObjectData> objectByIdDictionary;
    private bool isInitialized = false;

    /// <summary>
    /// 初始化字典（第一次使用时自动初始化）
    /// </summary>
    private void InitializeDictionaries()
    {
        if (isInitialized)
            return;

        objectByNameDictionary = new Dictionary<string, ObjectData>();
        objectByIdDictionary = new Dictionary<int, ObjectData>();

        foreach (var obj in objects)
        {
            if (!string.IsNullOrEmpty(obj.objectName))
            {
                // 使用小写键名以便不区分大小写查找
                string key = obj.objectName.ToLower();
                if (!objectByNameDictionary.ContainsKey(key))
                {
                    objectByNameDictionary[key] = obj;
                }
                else
                {
                    Debug.LogWarning($"重复的物体名称: {obj.objectName}");
                }
            }

            if (!objectByIdDictionary.ContainsKey(obj.objectID))
            {
                objectByIdDictionary[obj.objectID] = obj;
            }
            else
            {
                Debug.LogWarning($"重复的物体ID: {obj.objectID} - {obj.objectName}");
            }
        }

        isInitialized = true;
    }

    /// <summary>
    /// 根据物体名称获取描述文本（主要功能）
    /// </summary>
    public string GetDescriptionByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            Debug.LogWarning("物体名称不能为空");
            return "未知物体";
        }

        InitializeDictionaries();

        string key = objectName.ToLower();
        if (objectByNameDictionary.ContainsKey(key))
        {
            return objectByNameDictionary[key].description;
        }

        Debug.LogWarning($"未找到物体: {objectName}");
        return $"未找到物体: {objectName}的描述";
    }

    /// <summary>
    /// 根据物体ID获取描述文本
    /// </summary>
    public string GetDescriptionByID(int objectID)
    {
        InitializeDictionaries();

        if (objectByIdDictionary.ContainsKey(objectID))
        {
            return objectByIdDictionary[objectID].description;
        }

        Debug.LogWarning($"未找到物体ID: {objectID}");
        return $"未找到ID为 {objectID} 的物体的描述";
    }

    /// <summary>
    /// 根据物体ID获取描述文本
    /// </summary>
    public string GetNameByID(int objectID)
    {
        InitializeDictionaries();

        if (objectByIdDictionary.ContainsKey(objectID))
        {
            return objectByIdDictionary[objectID].objectName;
        }

        Debug.LogWarning($"未找到物体ID: {objectID}");
        return $"未找到ID为 {objectID} 的物体的描述";
    }

    /// <summary>
    /// 根据物体名称获取完整数据
    /// </summary>
    public ObjectData GetObjectDataByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
            return null;

        InitializeDictionaries();

        string key = objectName.ToLower();
        if (objectByNameDictionary.ContainsKey(key))
        {
            return objectByNameDictionary[key];
        }

        return null;
    }

    /// <summary>
    /// 根据物体ID获取完整数据
    /// </summary>
    public ObjectData GetObjectDataByID(int objectID)
    {
        InitializeDictionaries();

        if (objectByIdDictionary.ContainsKey(objectID))
        {
            return objectByIdDictionary[objectID];
        }

        return null;
    }

    /// <summary>
    /// 获取物体精灵图
    /// </summary>
    public Sprite GetSpriteByName(string objectName)
    {
        ObjectData data = GetObjectDataByName(objectName);
        return data?.objectSprite;
    }

    /// <summary>
    /// 获取物体精灵图（通过ID）
    /// </summary>
    public Sprite GetSpriteByID(int objectID)
    {
        ObjectData data = GetObjectDataByID(objectID);
        return data?.objectSprite;
    }

    /// <summary>
    /// 检查物体是否存在
    /// </summary>
    public bool ContainsObject(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
            return false;

        InitializeDictionaries();
        return objectByNameDictionary.ContainsKey(objectName.ToLower());
    }

    /// <summary>
    /// 检查物体ID是否存在
    /// </summary>
    public bool ContainsObject(int objectID)
    {
        InitializeDictionaries();
        return objectByIdDictionary.ContainsKey(objectID);
    }

    /// <summary>
    /// 获取所有物体名称列表
    /// </summary>
    public List<string> GetAllObjectNames()
    {
        List<string> names = new List<string>();
        foreach (var obj in objects)
        {
            if (!string.IsNullOrEmpty(obj.objectName))
            {
                names.Add(obj.objectName);
            }
        }
        return names;
    }

    /// <summary>
    /// 根据类型获取物体名称列表
    /// </summary>
    public List<string> GetObjectNamesByType(ObjectType type)
    {
        List<string> names = new List<string>();
        foreach (var obj in objects)
        {
            if (obj.objectType == type && !string.IsNullOrEmpty(obj.objectName))
            {
                names.Add(obj.objectName);
            }
        }
        return names;
    }

    /// <summary>
    /// 添加新物体数据
    /// </summary>
    public void AddObject(ObjectData newObject)
    {
        if (newObject != null && !string.IsNullOrEmpty(newObject.objectName))
        {
            objects.Add(newObject);
            isInitialized = false; // 标记需要重新初始化
            Debug.Log($"添加新物体: {newObject.objectName}");
        }
    }

    /// <summary>
    /// 移除物体数据
    /// </summary>
    public void RemoveObject(string objectName)
    {
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            if (objects[i].objectName == objectName)
            {
                objects.RemoveAt(i);
                isInitialized = false;
                Debug.Log($"移除物体: {objectName}");
                return;
            }
        }
    }

    /// <summary>
    /// 清空所有数据
    /// </summary>
    public void ClearAllData()
    {
        objects.Clear();
        isInitialized = false;
        Debug.Log("已清空所有物体数据");
    }
}
