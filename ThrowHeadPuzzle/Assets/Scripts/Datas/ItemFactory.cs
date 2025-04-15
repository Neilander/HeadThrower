using UnityEngine;
using System.Collections.Generic;

// 定义物品类，包含物品的基本信息
public class Item
{
    public string name;
    public int id;
    public string description;
    public Sprite image; // 新增图片字段

    public Item(string name, int id, string description, Sprite image)
    {
        this.name = name;
        this.id = id;
        this.description = description;
        this.image = image;
    }
}

// 物品工厂类，用于存储和提供物品信息
public class ItemFactory : MonoBehaviour
{
    // 存储所有物品的字典，键可以是物品名称或者编号
    private Dictionary<string, Item> itemDictionaryByName = new Dictionary<string, Item>();
    private Dictionary<int, Item> itemDictionaryById = new Dictionary<int, Item>();
    [Header("Pictures Setting")]
    public Sprite swordImage; // 剑的图片
    public Sprite shieldImage; // 盾牌的图片

    void Awake()
    {
        // 初始化一些物品数据
        InitializeItems();
    }

    // 初始化物品数据的方法
    private void InitializeItems()
    {
        Item sword = new Item("Sword", 1, "A sharp sword for combat.",swordImage);
        Item shield = new Item("Shield", 2, "A sturdy shield for defense.",shieldImage);
        Item storyIntro = new Item("StoryBook", 3, "我叫林哲，我的邻居好像搬家了，走前也没和我说一声，" +
            "新邻居家里有个小孩，老是用怪异的眼神看着我，还说着让我赶紧滚出这里的疯话，新邻居说这孩子有点心理问题我也就没放在心上。" +
            "我家的狗最近总是不见踪影，可能是去和隔壁小孩玩了吧。最近邻居好像都知道我得重感冒了，都离我家远远的，过了几天，我被捕了。为什么？", swordImage);

        // 将物品添加到字典中
        itemDictionaryByName[sword.name] = sword;
        itemDictionaryByName[shield.name] = shield;
        itemDictionaryByName[storyIntro.name] = storyIntro;

        itemDictionaryById[sword.id] = sword;
        itemDictionaryById[shield.id] = shield;
        itemDictionaryById[storyIntro.id] = storyIntro;
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