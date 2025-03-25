using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public delegate void SuccessEvent (JSONObject json);
public delegate void ErrorEvent (JSONObject json);
public delegate void UpdateProgress (float progress);

public class WebRequestsManager : Singleton<WebRequestsManager>
{
    public void DownloadSong(JSONObject json, SuccessEvent successEvent, ErrorEvent errorEvent, UpdateProgress updateProgress)
    {
        Debug.Log(json);
        StartCoroutine(PostRequest(json, successEvent, errorEvent, updateProgress));
    }

    public void SearchSong(string query, SuccessEvent successEvent, ErrorEvent errorEvent, UpdateProgress updateProgress)
    {
        string[] splitArray =  query.Split(char.Parse(" "));
        string finalQuery = String.Join("%20", splitArray);
        string url = "http://127.0.0.1:8000/api/songs/search?query={0}";
        string apiCall = string.Format(url, finalQuery);

        StartCoroutine(GetRequest(apiCall, successEvent, errorEvent, updateProgress));
    }

    private IEnumerator PostRequest(JSONObject track, SuccessEvent successEvent, ErrorEvent errorEvent, UpdateProgress updateProgress)
    {
        string url = "http://127.0.0.1:8000/api/download/objects";
        WWWForm form = new WWWForm();
        byte[] encodedPayload = new System.Text.UTF8Encoding().GetBytes(track.ToString());
        // form.AddField("song", track.ToString());
        // form.AddBinaryData("song", encodedPayload);
        Debug.Log(url);
        Debug.Log(track.ToString());

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            Debug.Log("Posting...");
            webRequest.uploadHandler = (UploadHandler) new UploadHandlerRaw(encodedPayload);
            webRequest.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("cache-control", "no-cache");

            UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();

            yield return DownloadProgress(operation, updateProgress);

            JSONObject json = null;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    json = new JSONObject();
                    json.AddField("error", webRequest.error);
                    errorEvent(json);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    json = new JSONObject();
                    json.AddField("error", webRequest.error);
                    errorEvent(json);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    json = new JSONObject();
                    json.AddField("error", webRequest.error);
                    errorEvent(json);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("COMPLETE");
                    track.AddField("song_path", MusicPlayerManager.Instance.fullLibraryPath+'/'+PUtils.RemoveQuotes(track["name"].ToString()) + ".mp3");
                    successEvent(track);
                    System.IO.File.WriteAllBytes(@PUtils.RemoveQuotes(track["song_path"].ToString()), webRequest.downloadHandler.data);
                    Debug.Log(MusicPlayerManager.Instance.fullLibraryPath);
                    break;
            }
        }
    }

    IEnumerator DownloadProgress(UnityWebRequestAsyncOperation operation, UpdateProgress updateProgress)
    {
        while (!operation.isDone)
        {
            float downloadDataProgress = operation.progress * 100;
            updateProgress(downloadDataProgress);
            yield return null;
        }
    }

    private IEnumerator GetRequest(string url, SuccessEvent successEvent, ErrorEvent errorEvent, UpdateProgress updateProgress)
    {
        WWWForm form = new WWWForm();
        form.AddField("myField", "myData");

        JSONObject json = null;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            Debug.Log("Searching...");
            Debug.Log(url);
            UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();

            yield return DownloadProgress(operation, updateProgress);

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    // Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    json = new JSONObject();
                    json.AddField("error", webRequest.error);
                    errorEvent(json);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    // Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    json = new JSONObject();
                    json.AddField("error", webRequest.error);
                    errorEvent(json);
                    break;
                case UnityWebRequest.Result.Success:
                    json = new JSONObject(webRequest.downloadHandler.text);
                    if(successEvent != null)
                        successEvent(json);
                    break;
            }
        }
    }
}
