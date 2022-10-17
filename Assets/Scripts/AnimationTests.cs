using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnimationTests : MonoBehaviour
{
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Animate()
    {
        rectTransform.DOMove(new Vector2(100,100), 0.2f).SetLoops(2,LoopType.Yoyo);
    }
}
