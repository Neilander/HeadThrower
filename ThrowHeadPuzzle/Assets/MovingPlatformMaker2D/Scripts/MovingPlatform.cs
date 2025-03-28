using UnityEngine;
using System.Collections;

namespace MovingPlatformMaker2D
{

    // 将该组件添加到Unity的组件菜单中，路径为 "Moving Platform Maker 2D/Moving Platform"，
    // 这样在Unity编辑器中可以方便地通过菜单添加该组件到游戏对象上
    [AddComponentMenu("Moving Platform Maker 2D/Moving Platform")]
    // 定义一个名为 MovingPlatform 的公共类，继承自 MonoBehaviour，
    // 意味着它可以作为一个脚本组件挂载到Unity的游戏对象上
    public class MovingPlatform : MonoBehaviour
    {
        // 定义一个 LayerMask 类型的公共变量 layerMask，
        // 用于指定要检测碰撞的游戏对象所在的层
        public LayerMask layerMask;

        // 当脚本在Unity编辑器中被重置（例如，首次添加到游戏对象或点击脚本组件上的重置按钮）时调用的方法
        void Reset()
        {
            // 将 layerMask 设置为只包含名为 "Player" 的层，
            // 即只对 "Player" 层的游戏对象进行碰撞检测
            layerMask = LayerMask.GetMask(new string[] { "Player" });
        }

        // 当有其他碰撞体进入该碰撞体时调用的方法
        void OnCollisionEnter2D(Collision2D other)
        {
            // 调用 checkCollision 方法处理碰撞事件
            checkCollision(other);
        }

        // 当有其他碰撞体持续与该碰撞体接触时调用的方法
        void OnCollisionStay2D(Collision2D other)
        {
            // 调用 checkCollision 方法处理持续碰撞事件
            checkCollision(other);
        }

        // 当有其他碰撞体离开该碰撞体时调用的方法
        void OnCollisionExit2D(Collision2D other)
        {
            // 检查 layerMask 是否未设置（值为 0）或者碰撞的游戏对象所在的层在 layerMask 中
            if (layerMask.value == 0 || Utils.IsInLayerMask(other.gameObject.layer, layerMask))
            {
                // 检查碰撞的游戏对象是否有父对象，并且父对象是否为当前移动平台对象
                if (other.transform.parent != null && transform == other.transform.parent.transform)
                {
                    // 如果满足条件，将碰撞的游戏对象的父对象设置为 null，使其脱离移动平台
                    other.transform.parent = null;
                }
            }
        }

        // 私有方法，用于检查碰撞并处理相关逻辑
        private void checkCollision(Collision2D other)
        {
            // 检查 layerMask 是否未设置（值为 0）或者碰撞的游戏对象所在的层在 layerMask 中
            if (layerMask.value == 0 || Utils.IsInLayerMask(other.gameObject.layer, layerMask))
            {
                // 遍历所有的碰撞接触点
                foreach (ContactPoint2D contact in other.contacts)
                {
                    // 在场景视图中绘制一条从碰撞点出发，沿着碰撞法线方向的红色射线，用于调试查看碰撞情况
                    Debug.DrawRay(contact.point, contact.normal, Color.red);
                    // 检查碰撞法线的 y 分量是否小于 0，即碰撞是从上方发生的
                    if (contact.normal.y < 0)
                    {
                        // 检查碰撞的刚体的 y 方向速度是否小于等于 0，即物体是向下运动或静止的
                        if (other.rigidbody.velocity.y <= 0)
                        {
                            // 如果满足条件，将碰撞的游戏对象的父对象设置为当前移动平台对象，
                            // 这样碰撞的游戏对象会跟随移动平台一起移动
                            other.transform.parent = transform;
                        }
                        // 一旦找到符合条件的碰撞点，就跳出方法，不再继续检查其他碰撞点
                        return;
                    }
                }
            }
        }
    }
}