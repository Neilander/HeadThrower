using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ChatGPTWrapper
{

    public class Requests
    {
        /// <summary>
        /// 为 UnityWebRequest 设置请求头
        /// </summary>
        /// <param name="req">UnityWebRequest 对象</param>
        /// <param name="headers">包含请求头键值对的列表</param>
        private void SetHeaders(ref UnityWebRequest req, List<(string, string)> headers)
        {
            // 遍历头信息列表，为请求设置每个头信息
            for (int i = 0; i < headers.Count; i++)
            {
                req.SetRequestHeader(headers[i].Item1, headers[i].Item2);
            }
        }

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        /// <typeparam name="T">响应数据的类型</typeparam>
        /// <param name="uri">请求的 URI</param>
        /// <param name="callback">请求成功时的回调函数，用于处理响应数据</param>
        /// <param name="headers">可选的请求头列表</param>
        /// <returns>用于协程的 IEnumerator 对象</returns>
        public IEnumerator GetReq<T>(string uri, System.Action<T> callback, List<(string, string)> headers = null)
        {
            UnityWebRequest webRequest = new UnityWebRequest(uri, "GET");
            if (headers != null) SetHeaders(ref webRequest, headers);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.disposeUploadHandlerOnDispose = true;
            webRequest.disposeDownloadHandlerOnDispose = true;
            // 发送请求并等待响应
            yield return webRequest.SendWebRequest();

#if UNITY_2020_3_OR_NEWER
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    // 请求成功时，将响应的 JSON 数据转换为指定类型的对象，并调用回调函数
                    var responseJson = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    callback(responseJson);
                    break;
            }
#else
            if(!string.IsNullOrWhiteSpace(webRequest.error))
            {
                Debug.LogError($"Error {webRequest.responseCode} - {webRequest.error}");
                yield break;
            }
            else
            {
                var responseJson = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    callback(responseJson);
            }
#endif
            webRequest.Dispose();

        }

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <typeparam name="T">响应数据的类型</typeparam>
        /// <param name="uri">请求的 URI</param>
        /// <param name="json">要发送的 JSON 数据</param>
        /// <param name="callback">请求成功时的回调函数，用于处理响应数据</param>
        /// <param name="headers">可选的请求头列表</param>
        /// <returns>用于协程的 IEnumerator 对象</returns>
        public IEnumerator PostReq<T>(string uri, string json, System.Action<T> callback, List<(string, string)> headers = null)
        {
            UnityWebRequest webRequest = new UnityWebRequest(uri, "POST");
            if (headers != null) SetHeaders(ref webRequest, headers);

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.disposeDownloadHandlerOnDispose = true;
            webRequest.disposeUploadHandlerOnDispose = true;

            Debug.Log("Sending ");
            yield return webRequest.SendWebRequest();

#if UNITY_2020_3_OR_NEWER
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    if (uri.EndsWith("/completions"))
                    {
                        var errJson = JsonUtility.FromJson<ChatGPTResError>(webRequest.downloadHandler.text);
                        Debug.LogError(errJson.error.message);
                    }
                    if (webRequest.error == "HTTP/1.1 429 Too Many Requests")
                    {
                        Debug.Log("retrying...");
                        yield return PostReq<T>(uri, json, callback, headers);
                    }
                    break;
                case UnityWebRequest.Result.Success:
                    var responseJson = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    callback(responseJson);
                    break;
            }
#else
            if(!string.IsNullOrWhiteSpace(webRequest.error))
            {
                Debug.LogError($"Error {webRequest.responseCode} - {webRequest.error}");
                yield break;
            }
            else
            {
                var responseJson = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
                    callback(responseJson);
            }
#endif

            webRequest.Dispose();
        }
    }
}
