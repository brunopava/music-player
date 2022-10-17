using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityManager : Singleton<ActivityManager>
{
    AndroidJavaClass unityClass;
    AndroidJavaObject unityActivity;
    AndroidJavaObject pluginInstance;

#if UNITY_ANDROID && !UNITY_EDITOR
    private void Start()
    {
        InitializePlugin("com.zerofucks.unityactivity.PluginActivity");
    }

    private void InitializePlugin(string pluginName)
    {
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");  

        pluginInstance = new AndroidJavaObject(pluginName); 

        if(pluginInstance == null)
        {
            Debug.Log("De Ruim");
            return;
        }

        pluginInstance.CallStatic("receiveUnityActivity", unityActivity);
    }
#endif

    public void PrepareSong(string path)
    {
        if(pluginInstance != null)
        {
            pluginInstance.Call("PrepareSong", path);
        }
    }

    public void SongPrepared(string callbackResult)
    {
        Debug.Log("SongPrepared: "+callbackResult);
        MusicPlayerManager.Instance.PlayCurrentSong();
    }

    public void SongComplete(string callbackResult)
    {
        Debug.Log("SongComplete: "+callbackResult);
        MusicPlayerManager.Instance.NextSong();
    }

    public void SetNextSong(string path)
    {
        if(pluginInstance != null)
        {
            pluginInstance.Call("SetNextSong", path);
        }
    }

    public void SetPrevSong(string path)
    {
        if(pluginInstance != null)
        {
            pluginInstance.Call("SetPrevSong", path);
        }
    }

    public bool IsPlaying()
    {
        if(pluginInstance != null)
        {
            return pluginInstance.Call<bool>("IsPlaying");
        }
        return false;
    }

    public void Play()
    {
        if(pluginInstance != null)
        {
            pluginInstance.Call("Play");
        }
    }

    public void Pause()
    {
        if(pluginInstance != null)
        {
            pluginInstance.Call("Pause");
        }
    }

    public void PlayNextSong()
    {
        if(pluginInstance != null)
        {
            pluginInstance.Call("PlayNextSong");
        }
    }

    public void PlayPrevSong()
    {
        if(pluginInstance != null)
        {
            pluginInstance.Call("PlayPrevSong");
        }
    }

    public void SetVolume(float volume)
    {
        if(pluginInstance != null)
        {
            pluginInstance.Call("SetVolume", volume);
        }
    }

    public int GetCurrentTime()
    {
        if(pluginInstance != null)
        {
            int currentTime = pluginInstance.Call<int>("GetCurrentTime");
            return currentTime;
        }
        return 0;
    }

    public int GetDuration()
    {
        if(pluginInstance != null)
        {
            int duration = pluginInstance.Call<int>("GetDuration");
            return duration;
        }
        return 0;
    }

    public int GetTotalSpace()
    {
        if(pluginInstance != null)
        {
            int space = pluginInstance.Call<int>("GetTotalSpace", false);
            return space;
        }
        return 0;
    }


    public int GetFreeSpace()
    {
        if(pluginInstance != null)
        {
            int space = pluginInstance.Call<int>("GetFreeSpace", false);
            return space;
        }
        return 0;
    }

    public int GetUsedSpace()
    {
        if(pluginInstance != null)
        {
            int space = pluginInstance.Call<int>("GetUsedSpace", false);
            return space;
        }
        return 0;
    }
}
