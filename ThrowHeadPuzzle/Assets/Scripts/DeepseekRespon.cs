using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System; // 引入Newtonsoft.Json库用于处理JSON

public class DeepseekRespon : MonoBehaviour
{
    public InputField inputText; // 第一个 InputField 用于输入文本
    public Text outputText; // 第二个 Text 用于显示回答
    public string apiUrl; // 硅基流动平台 API 的 URL
    public string apiKey; // API 密钥
    [TextArea(4, 6)]
    public string initialRequirement;
    [TextArea(4, 6)]
    public string _initialPrompt;
    public UnityEvent<string> chatGPTResponse = new UnityEvent<string>();

    [SerializeField]
    string aiName = "AI Narrator";
    [SerializeField]
    string playerName = "Player";
    bool isdone = false;

    [Header("传入的场景")]
    public AIDialogueScenario gameScenario;

    // 定义一个枚举来表示不同的模型选项
    public enum AvailableModels
    {
        DeepSeekR1_1_5B,
        DeepSeekR1_7B,
        DeepSeekR1_14B,
        DeepSeekR1_32B,
        DeepSeekR1_671B,
        DeepSeekV3,
        通义千问2_5_7B
        // 你可以在这里添加更多的模型选项
    }

    public AvailableModels selectedModel = AvailableModels.DeepSeekR1_1_5B; // 新增：可在视图窗口选择的模型枚举变量

    void Awake()
    {
        if (!isdone)
        {
            if (gameScenario != null)
            {
                _initialPrompt = gameScenario.returnFullPrompt();
                initialRequirement = gameScenario.returnFullRequest();
            }
               
            InitialSendTextToAPI();
            isdone = !isdone;
            Debug.Log("已经完成初始化");
        }
    }

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
        string requestBody = InitialPrepareRequestBody(input, "system");
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
        string requestBody = PrepareRequestBody(input, "user");
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
    /// 初始化发送到 API 的请求体
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string InitialPrepareRequestBody(string input, string _role)
    {

        // 根据选择的枚举值获取对应的模型名称
        string modelName = GetModelNameFromEnum(selectedModel);
        // 根据API文档，构建请求体
        var request = new
        {
            model = modelName,
            stream = false,
            max_tokens = 512,
            temperature = 0.7,
            top_p = 0.7,
            top_k = 50,
            frequency_penalty = 0.5,
            n = 1,
            stop = new string[] { },
            messages = new[]
            {
                new { role = _role, content = input }
            }
        };
        // 使用Newtonsoft.Json将请求对象序列化为JSON字符串
        return JsonConvert.SerializeObject(request);
    }


    /// <summary>
    /// 准备发送到 API 的请求体
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string PrepareRequestBody(string input, string _role)
    {

        // 根据选择的枚举值获取对应的模型名称
        string modelName = GetModelNameFromEnum(selectedModel);
        // 根据API文档，构建请求体
        var request = new
        {
            model = modelName,
            stream = false,
            max_tokens = 512,
            temperature = 0.7,
            top_p = 0.7,
            top_k = 50,
            frequency_penalty = 0.5,
            n = 1,
            stop = new string[] { },
            messages = new[]
            {
                //new { role = "system", content = initialRequirement + "\n" + knowledgeBaseContent }, // 添加初始要求和知识库信息
                new { role = "system", content = initialRequirement + "\n"}, // 添加初始要求和知识库信息
                new { role = _role, content = input }
            }
        };
        // 使用Newtonsoft.Json将请求对象序列化为JSON字符串
        return JsonConvert.SerializeObject(request);
    }

    /// <summary>
    /// 根据枚举值获取对应的模型名称
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    private string GetModelNameFromEnum(AvailableModels model)
    {
        switch (model)
        {
            case AvailableModels.DeepSeekR1_14B:
                return "deepseek-ai/DeepSeek-R1-Distill-Qwen-14B";
            case AvailableModels.DeepSeekR1_1_5B:
                return "deepseek-ai/DeepSeek-R1-Distill-Qwen-1.5B";
            case AvailableModels.DeepSeekR1_32B:
                return "deepseek-ai/DeepSeek-R1-Distill-Qwen-32B";
            case AvailableModels.DeepSeekR1_671B:
                return "deepseek-ai/DeepSeek-R1";
            case AvailableModels.DeepSeekR1_7B:
                return "deepseek-ai/DeepSeek-R1-Distill-Qwen-7B";
            case AvailableModels.DeepSeekV3:
                return "deepseek-ai/DeepSeek-V3";
            case AvailableModels.通义千问2_5_7B:
                return "Qwen/Qwen2.5-7B-Instruct";
            // 你可以在这里添加更多的case分支来处理更多的模型选项
            default:
                return "deepseek-ai/DeepSeek-R1-Distill-Qwen-1.5B"; // 默认模型
        }
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

                // 打印请求URL和请求头用于调试
                //Debug.Log($"请求URL: {apiUrl}");
                foreach (var header in client.DefaultRequestHeaders)
                {
                    //Debug.Log($"请求头: {header.Key}: {string.Join(", ", header.Value)}");
                }

                // 发送 POST 请求并等待响应
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);


                // 打印请求URL和请求头用于调试
                //Debug.Log($"请求URL: {apiUrl}"); // 新加：打印请求URL
                // foreach (var header in client.DefaultRequestHeaders)
                // {
                //     //Debug.Log($"请求头: {header.Key}: {string.Join(", ", header.Value)}"); // 新加：打印请求头
                // }

                if (response.IsSuccessStatusCode)
                {
                    // 读取响应内容
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // 打印响应内容用于调试
                    Debug.Log($"响应内容: {responseContent}"); // 新加：打印响应内容
                    // 读取响应内容
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    // 记录请求失败的状态码和响应内容
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.Log($"API请求失败，状态码: {response.StatusCode}, 响应内容: {errorContent}"); // 修改：记录更详细的失败信息
                    return null;
                }
            }
        }
        catch (HttpRequestException e)
        {
            // 记录HTTP请求错误和详细信息
            Debug.Log($"HTTP请求错误: {e.Message}, 详细信息: {e.InnerException?.Message}"); // 修改：记录详细错误信息
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
        public string id;
        public Choice[] choices;
        public Usage usage;
        public int created;
        public string model;
        public string @object;
    }

    private class Choice
    {
        public Message message;
        public string finish_reason;
    }

    private class Message
    {
        public string role;
        public string content;
        public string reasoning_content;
        public ToolCall[] tool_calls;
    }

    private class ToolCall
    {
        public string id;
        public string type;
        public Function function;
    }

    private class Function
    {
        public string name;
        public string arguments;
    }

    private class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}
