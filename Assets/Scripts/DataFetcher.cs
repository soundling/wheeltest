using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DataFetcher : MonoBehaviour
{
    public string BaseUrl = "https://8ghzo9aw5h.execute-api.eu-west-3.amazonaws.com/prod/";

    public void GetWheelInfos(Action<WheelInfosData> callback)
    {
        StartCoroutine(GetWheelInfoCoroutines(callback));
    }

    private IEnumerator GetWheelInfoCoroutines(Action<WheelInfosData> callback)
    {
        var w = CreateApiPostRequest("getData");
        yield return w.SendWebRequest();
        if (w.result == UnityWebRequest.Result.Success)
        {
            var rr = JsonUtility.FromJson<WheelInfosData>(w.downloadHandler.text);
            callback?.Invoke(rr);
        }

        if (w.result != UnityWebRequest.Result.Success) Debug.Log(w.error);
        yield return null;
    }

    public void GetWheelResult(Action<WheelResultData> callback, Key k)
    {
        StartCoroutine(GetWheelResultCoroutine(callback, k));
    }

    private IEnumerator GetWheelResultCoroutine(Action<WheelResultData> callback, Key k)
    {
        var w = CreateApiPostRequest("getResult", k);
        yield return w.SendWebRequest();
        if (w.result == UnityWebRequest.Result.Success)
        {
            var rr = JsonUtility.FromJson<WheelResultData>(w.downloadHandler.text);
            callback?.Invoke(rr);
        }

        if (w.result != UnityWebRequest.Result.Success) Debug.Log(w.error);
        yield return null;
    }
    public UnityWebRequest CreateApiPostRequest(string actionUrl, object body = null)
    {
        return CreateApiRequest(BaseUrl + actionUrl, UnityWebRequest.kHttpVerbPOST, body);
    }

    private UnityWebRequest CreateApiRequest(string url, string method, object body)
    {
        string bodyString = null;
        if (body is string)
        {
            bodyString = (string)body;
        }
        else if (body != null)
        {
            bodyString = JsonUtility.ToJson(body);
        }

        var request = new UnityWebRequest();
        request.url = url;
        request.method = method;
        request.downloadHandler = new DownloadHandlerBuffer();
        if (body != null)
        {
            var postData = Encoding.UTF8.GetBytes(bodyString);
            request.uploadHandler = new UploadHandlerRaw(postData);
        }

        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 60;
        return request;
    }
}