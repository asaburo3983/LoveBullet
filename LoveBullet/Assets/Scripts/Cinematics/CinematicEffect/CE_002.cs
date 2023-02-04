using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

// Cinematic Effect 002 : 画面3回点滅
public class CE_002 : CE_Base
{
    [SerializeField, Header("黒いパネル")]
    private Image blackPanel;
    [SerializeField, Header("点滅速度")]
    private float blackenSpeed;

    void OnEnable()
    {
        if (CinematicsManager.Instance != null)
        {
            tween = blackPanel.DOFade(1f, blackenSpeed).
                SetLoops(6, LoopType.Yoyo).
                OnComplete(EndEffect);
        }
    }
}
