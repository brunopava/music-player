using System.Text.RegularExpressions;
using UnityEngine;
using DG.Tweening;

public class PUtils
{
    public static string RemoveQuotes(string target)
    {
        return Regex.Replace(target, "\"", string.Empty);
    }

    public static Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
    }

    public static void AnimateButtonClick(RectTransform button)
    {
        button.localScale = new Vector3(1f,1f,1f);
        button.DOScale(new Vector3(0.9f,0.9f,0.9f), 0.1f).SetLoops(2, LoopType.Yoyo);
    }

    public static string FormatTime (float time)
    {
        System.TimeSpan ts = System.TimeSpan.FromMilliseconds(time);
        string result = ts.ToString("m\\:ss");
        return result;
    }

    public static float GetProgress(int current, int duration)
    {
        float equation = (current*100)/duration;
        float percent = equation*0.01f;
        return percent;
    }
}
