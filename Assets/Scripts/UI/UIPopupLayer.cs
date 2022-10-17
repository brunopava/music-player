using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPopupLayer : Singleton<UIPopupLayer>
{
    public UIPopup[] _popups;

    private void Awake()
    {   
    	DisablePopups();
    }

    public void DisablePopups()
    {
        _popups = gameObject.GetComponentsInChildren<UIPopup>(true);

        foreach(UIPopup current in _popups)
        {
            current.gameObject.SetActive(false);
        }
    }

    public T GetPopupByType<T>() where T : class
    {
        _popups = gameObject.GetComponentsInChildren<UIPopup>(true);

        foreach(UIPopup current in _popups)
        {
            if(current.gameObject.GetComponent<T>() != null)
            {
                return current.gameObject.GetComponent<T>();
            }
        }
        return null;
    }

    public void ShowPopupCreatePlaylist()
    {
        UIPopupCreatePlaylist popup = GetPopupByType<UIPopupCreatePlaylist>();
        popup.gameObject.SetActive(true);
        popup.Initialize(
            ()=>{
                
            },
            ()=>{
                JSONObject json = new JSONObject();
                json.AddField("name", popup.playListName.text);
                json.AddField("songs", new JSONObject());
                MusicPlayerManager.Instance.CreatePlaylist(json);
            }
        );
    }
}
