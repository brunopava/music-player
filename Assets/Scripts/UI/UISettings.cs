using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : UIScreen
{
    public Text totalSpace;
    public Text usedSpace;
    public Text freeSpace;

    public void Refresh()
    {
    	totalSpace.text = ActivityManager.Instance.GetTotalSpace().ToString();
    	usedSpace.text = ActivityManager.Instance.GetUsedSpace().ToString();
    	freeSpace.text = ActivityManager.Instance.GetFreeSpace().ToString();
    }
}
