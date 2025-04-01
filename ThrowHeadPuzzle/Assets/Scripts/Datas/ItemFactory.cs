using UnityEngine;
using System.Collections.Generic;

// 定义物品类，包含物品的基本信息
public class Item
{
    public string name;
    public int id;
    public string description;

    public Item(string name, int id, string description)
    {
        this.name = name;
        this.id = id;
        this.description = description;
    }
}

// 物品工厂类，用于存储和提供物品信息
public class ItemFactory : MonoBehaviour
{
    // 存储所有物品的字典，键可以是物品名称或者编号
    private Dictionary<string, Item> itemDictionaryByName = new Dictionary<string, Item>();
    private Dictionary<int, Item> itemDictionaryById = new Dictionary<int, Item>();

    void Awake()
    {
        // 初始化一些物品数据
        InitializeItems();
    }

    // 初始化物品数据的方法
    private void InitializeItems()
    {
        Item sword = new Item("Sword", 1, "A sharp sword for combat.");
        Item shield = new Item("Shield", 2, "A sturdy shield for defense.");

        // 将物品添加到字典中
        itemDictionaryByName[sword.name] = sword;
        itemDictionaryByName[shield.name] = shield;

        itemDictionaryById[sword.id] = sword;
        itemDictionaryById[shield.id] = shield;
    }

    // 根据物品名称获取物品信息的方法
    public Item GetItemByName(string itemName)
    {
        if (itemDictionaryByName.ContainsKey(itemName))
        {
            return itemDictionaryByName[itemName];
        }
        return null;
    }

    // 根据物品编号获取物品信息的方法
    public Item GetItemById(int itemId)
    {
        if (itemDictionaryById.ContainsKey(itemId))
        {
            return itemDictionaryById[itemId];
        }
        return null;
    }
}    