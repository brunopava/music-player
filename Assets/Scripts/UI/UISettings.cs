using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISettings : UIScreen
{
    public Button saveButton;
    public TMP_InputField inputfield;

    public void Refresh()
    {
        string url = PlayerPrefs.GetString("url");
    	inputfield.text = url;

        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(
            ()=>{
                PlayerPrefs.SetString("url", inputfield.text);
            }
        );
    }
}
