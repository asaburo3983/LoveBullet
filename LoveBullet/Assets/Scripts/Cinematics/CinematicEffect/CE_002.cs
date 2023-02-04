using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

// Cinematic Effect 002 : ���3��_��
public class CE_002 : CE_Base
{
    [SerializeField, Header("�����p�l��")]
    private Image blackPanel;
    [SerializeField, Header("�_�ő��x")]
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
