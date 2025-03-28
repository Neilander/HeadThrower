using System.Collections;
using UnityEngine;

namespace MovingPlatformMaker2D
{
    // 将该组件添加到Unity的组件菜单中，方便在编辑器中使用
    [AddComponentMenu("Moving Platform Maker 2D/Path Follower Trigger")]
    // 确保该脚本所在的游戏对象上有一个BoxCollider2D组件，如果没有则会自动添加
    [RequireComponent(typeof(BoxCollider2D))]
    // 定义一个名为PathFollowerTrigger的公共类，继承自MonoBehaviour
    public class PathFollowerTrigger : MonoBehaviour
    {
        // 定义一个LayerMask类型的公共变量，用于指定触发的层
        public LayerMask layerMask;

        // 定义一个PathFollower类型的数组，用于存储需要控制的路径跟随者
        public PathFollower[] followers = new PathFollower[0];

        // 定义一个布尔类型的公共变量，用于指定是否只有在进入触发器内部时才激活路径跟随者
        public bool activeOnlyWhenInside = false;

        // 定义一个布尔类型的公共变量，用于指定激活时是否改变路径跟随者的移动方向
        public bool changeDirectionOnActivate = false;

        // 当脚本在编辑器中被重置时调用的方法
        void Reset()
        {
            // 获取当前游戏对象上的Collider2D组件
            Collider2D col = GetComponent<Collider2D>();
            // 如果没有找到Collider2D组件，则添加一个BoxCollider2D组件
            if (col == null)
                col = gameObject.AddComponent<BoxCollider2D>();
            // 将该Collider2D组件设置为触发器
            col.isTrigger = true;

            // 将layerMask设置为只包含名为"Player"的层
            layerMask = LayerMask.GetMask(new string[] { "Player" });
        }

        // 当脚本实例被启用时调用的方法，通常用于初始化操作
        void Start()
        {
            // 遍历所有的路径跟随者
            foreach (PathFollower follower in followers)
                // 将所有路径跟随者的激活状态设置为false
                follower.active = false;
        }

        // 当有其他Collider2D进入该触发器时调用的方法
        void OnTriggerEnter2D(Collider2D other)
        {
            // 检查进入触发器的游戏对象的层是否不在指定的layerMask中，如果是则直接返回
            if (!Utils.IsInLayerMask(other.gameObject.layer, layerMask))
                return;

            // 遍历所有的路径跟随者
            foreach (PathFollower follower in followers)
            {
                // 将所有路径跟随者的激活状态设置为true
                follower.active = true;
            }
        }

        // 当有其他Collider2D离开该触发器时调用的方法
        void OnTriggerExit2D(Collider2D other)
        {
            // 检查离开触发器的游戏对象的层是否不在指定的layerMask中，或者activeOnlyWhenInside为false，如果是则直接返回
            if (!Utils.IsInLayerMask(other.gameObject.layer, layerMask) || !activeOnlyWhenInside)
                return;

            // 遍历所有的路径跟随者
            foreach (PathFollower follower in followers)
            {
                // 将所有路径跟随者的激活状态设置为false
                follower.active = false;
                // 如果changeDirectionOnActivate为true，则调用路径跟随者的Reverse方法改变其移动方向
                if (changeDirectionOnActivate)
                    follower.Reverse();
            }
        }
    }
}
