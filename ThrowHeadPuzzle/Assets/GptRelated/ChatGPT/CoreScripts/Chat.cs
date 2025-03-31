using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;

namespace ChatGPTWrapper
{
    // Due to OpenAI's new chat completions api, this replaces the old "Prompt" class, but the prompt class is still used for the older models.
    // 由于OpenAI推出了新的聊天API，此类替代了旧的 "Prompt" 类，但 "Prompt" 类仍用于旧模型
    public class Chat
    {
        private string _initialPrompt;
        // 存储当前对话的所有消息
        private List<Message> _currentChat = new List<Message>();


        /// <summary>
        /// Chat(prompt) prompt添加到当前对话列表中
        /// </summary>
        /// <param name="initialPrompt"></param>
        public Chat(string initialPrompt)
        {
            _initialPrompt = initialPrompt;
            Message systemMessage = new Message("system", initialPrompt);
            _currentChat.Add(systemMessage);
        }
        public List<Message> CurrentChat { get { return _currentChat; } }

        public enum Speaker
        {
            User,
            ChatGPT
        }

        /// <summary>
        /// 参数(第一个参数speaker.User or speaker.ChatGPT, 第二个参数 text)
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="text"></param>
        public void AppendMessage(Speaker speaker, string text)
        {

            switch (speaker)
            {
                case Speaker.User:
                    Message userMessage = new Message("user", text);
                    _currentChat.Add(userMessage);
                    break;
                case Speaker.ChatGPT:
                    Message chatGPTMessage = new Message("assistant", text);
                    _currentChat.Add(chatGPTMessage);
                    break;
            }
        }

        /// <summary>
        ///  移除最旧的一轮对话消息（用户和ChatGPT的一轮对话），并返回移除消息的总长度
        /// </summary>
        /// <returns></returns>
        public int RemoveOldestMessage()
        {
            int _removeLength = _currentChat[1].content.Length + _currentChat[2].content.Length;
            _currentChat.RemoveAt(1);
            _currentChat.RemoveAt(1);
            return _removeLength;
        }
    }
}
