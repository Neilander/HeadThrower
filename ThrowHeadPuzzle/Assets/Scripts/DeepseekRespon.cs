using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; // 引入Newtonsoft.Json库用于处理JSON

public class DeepseekRespon : MonoBehaviour
{
    public InputField inputText; // 第一个 InputField 用于输入文本
    public Text outputText; // 第二个 Text 用于显示回答
    public string apiUrl; // 硅基流动平台 API 的 URL
    public string apiKey; // API 密钥
    [TextArea(4, 6)]
    public string _initialPrompt;
    public UnityEvent<string> chatGPTResponse = new UnityEvent<string>();

    [SerializeField]
    string aiName = "AI Narrator";
    [SerializeField]
    string playerName = "Player";

    /// <summary>
    /// 发送prompt
    /// </summary>
    async public void InitialSendTextToAPI()
    {
        // 检查输入文本是否为空
        string input = _initialPrompt;
        if (string.IsNullOrEmpty(input))
        {
            Debug.Log("输入文本为空，请输入内容。");
            return;
        }

        // 准备请求体
        string requestBody = PrepareRequestBody(input);
        Debug.Log($"请求体: {requestBody}");

        // 调用 API 并获取响应
        string responseBody = await CallAPI(requestBody);

        // 处理响应结果
        ProcessResponse(responseBody);
    }
    /// <summary>
    /// 主方法，调用其他方法完成整个流程
    /// </summary>
    async public void SendTextToAPI()
    {
        // 检查输入文本是否为空
        string input = GetInputText();
        if (string.IsNullOrEmpty(input))
        {
            Debug.Log("输入文本为空，请输入内容。");
            return;
        }

        // 准备请求体
        string requestBody = PrepareRequestBody(input);
        Debug.Log($"请求体: {requestBody}");

        // 调用 API 并获取响应
        string responseBody = await CallAPI(requestBody);

        // 处理响应结果
        ProcessResponse(responseBody);
    }

    /// <summary>
    /// 获取输入文本
    /// </summary>
    /// <returns></returns>
    private string GetInputText()
    {
        return inputText.text;
    }

    /// <summary>
    /// 准备发送到 API 的请求体
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string PrepareRequestBody(string input)
    {
        // 根据API文档，构建请求体
        var request = new
        {
            model = "deepseek-3",
            messages = new[]
            {
                new { role = "user", content = input }
            }
        };
        // 使用Newtonsoft.Json将请求对象序列化为JSON字符串
        return JsonConvert.SerializeObject(request);
    }

    /// <summary>
    /// 调用 API 并返回响应内容
    /// </summary>
    /// <param name="requestBody"></param>
    /// <returns></returns>
    private async Task<string> CallAPI(string requestBody)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                // 设置超时时间，避免长时间等待
                client.Timeout = System.TimeSpan.FromSeconds(30); // 新加：设置超时时间
                // 添加 API 密钥到请求头
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
                // 创建请求内容
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // 发送 POST 请求并等待响应
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);


                // 打印请求URL和请求头用于调试
                Debug.Log($"请求URL: {apiUrl}"); // 新加：打印请求URL
                foreach (var header in client.DefaultRequestHeaders)
                {
                    Debug.Log($"请求头: {header.Key}: {string.Join(", ", header.Value)}"); // 新加：打印请求头
                }
                
                if (response.IsSuccessStatusCode)
                {
                    // 读取响应内容
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // 记录请求失败的状态码
                    Debug.Log($"API 请求失败，状态码: {response.StatusCode}");
                    return null;
                }
            }
        }
        catch (HttpRequestException e)
        {
            // 记录 HTTP 请求错误
            Debug.Log($"HTTP 请求错误: {e.Message}");
            return null;
        }
        catch (System.Exception e)
        {
            // 记录未知错误
            Debug.Log($"发生未知错误: {e.Message}");
            return null;
        }
    }

    // 处理 API 响应结果
    private void ProcessResponse(string responseBody)
    {
        if (!string.IsNullOrEmpty(responseBody))
        {
            try
            {
                // 解析JSON响应
                var response = JsonConvert.DeserializeObject<ResponseData>(responseBody);
                // 将响应内容显示在输出文本框中
                outputText.text = response.choices[0].message.content;
            }
            catch (JsonException e)
            {
                // 记录JSON解析错误
                Debug.Log($"JSON解析错误: {e.Message}");
            }
        }
    }
    // 定义响应数据的结构
    private class ResponseData
    {
        public Choice[] choices;
    }

    private class Choice
    {
        public Message message;
    }

    private class Message
    {
        public string content;
    }
}
