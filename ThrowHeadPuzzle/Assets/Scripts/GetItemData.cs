using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GetItemData : MonoBehaviour
{
    public Text tmpText; // 引用 Text 组件
    public ItemFactory itemFactory;
    public DeliverStringSO _item;


    void Start()
    {
        // // 根据名称获取物品信息
        // Item sword = itemFactory.GetItemByName("Sword");
        // if (sword != null)
        // {
        //     Debug.Log($"Name: {sword.name}, ID: {sword.id}, Description: {sword.description}");
        // }

        // // 根据编号获取物品信息
        // Item shield = itemFactory.GetItemById(2);
        // if (shield != null)
        // {
        //     Debug.Log($"Name: {shield.name}, ID: {shield.id}, Description: {shield.description}");
        //     tmpText.text = $"Description: {shield.description}";
        // }
        GetData("StoryBook");
    }

    //TODO：未初始化  =>  需要在enable里获取一下DeliverStringSO _item;的数据，用于读取

    public void GetData(string item)
    {
        Item _tempItem = itemFactory.GetItemByName(item);
        if (_tempItem != null)
            tmpText.text = $"{_tempItem.name}: {_tempItem.description}";
        else
            tmpText.text = $"error";
    }
}
