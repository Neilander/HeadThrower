using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GetItemData : MonoBehaviour
{
    public Text tmpText_name; // 引用 Text 组件
    public Text tmpText_discription; // 引用 Text 组件

    [SerializeField]
    private int itemID = 0;

    [Header("数据引用")]
    public DeliverStringSO deliverStringSO;
    public DeliverBoolSO isShowed;
    private string itemDiscription;

    [Header("物品ID传递")]
    [Tooltip("跨场景传送物品ID数据的文件")]
    public DeliverintSO _UIintSO;

    void Start()
    {
        //GetData("Sword");

        //DisplayObjectInfo();
    }

    private void OnEnable()
    {
        _UIintSO._intvalue += DeliverItemID;
        isShowed._boolvalue += DisplayObjectInfo;
    }

    private void OnDisable()
    {
        _UIintSO._intvalue -= DeliverItemID;
        isShowed._boolvalue -= DisplayObjectInfo;
    }

    private void DeliverItemID(int _int)
    {
        itemID = _int;
        //Debug.Log(itemID);
    }

    void DisplayObjectInfo(bool _bool)
    {
        //Debug.Log(2);

        if (deliverStringSO == null)
        {
            Debug.LogError("DeliverStringSO 未分配！");
            return;
        }

        // 方法1：通过名称获取描述
        // string description = deliverStringSO.GetDescriptionByName(objectName);
        // Debug.Log($"{objectName} 的描述: {description}");

        // 方法2：通过ID获取描述

        string description = deliverStringSO.GetDescriptionByID(itemID);
        tmpText_discription.text = description;
        string _name = deliverStringSO.GetNameByID(itemID);
        tmpText_name.text = _name;
        //Debug.Log($"ID {itemID} 的描述: {description}");

        // 方法3：获取完整数据
        // DeliverStringSO.ObjectData data = deliverStringSO.GetObjectDataByName(objectName);
        // if (data != null)
        // {
        //     Debug.Log($"名称: {data.objectName}");
        //     Debug.Log($"描述: {data.description}");
        //     Debug.Log($"类型: {data.objectType}");
        //     Debug.Log($"价值: {data.value}");

        //     // 可以使用 data.objectSprite 来设置UI图像等
        // }
    }

    // void Update()
    // {
    //     if (1)
    //         return;
    // }

    // public void GetData(string item)
    // {
    //     Item _tempItem = itemFactory.GetItemByName(item);
    //     if (_tempItem != null)
    //         tmpText.text = $"{_tempItem.name}: {_tempItem.description}";
    //     else
    //         tmpText.text = $"error";
    // }
}
