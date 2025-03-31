using System;
using System.Collections.Generic;

namespace ChatGPTWrapper
{

    [Serializable]
    // [Serializable] 特性表明这个类的实例可以被序列化，
    // 意味着可以将该类的对象转换为字节流，以便存储到文件或通过网络传输。

    //定义一个公共类 Message，用于表示与 ChatGPT 交互过程中的消息。
    public class Message
    {
        public string role;

        public string content;

        /// <summary>
        /// 接收两个字符串参数 r 和 c，分别用于初始化 role 角色和 content 具体内容 字段
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        public Message(string r, string c)
        {
            role = r;
            content = c;
        }
    }
}