using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

// Cinematic Effect 003 : スチル表示(1)
public class CE_003 : CE_Base
{
    [SerializeField, Header("左側に表示する場合の画像座標")]
    private Vector2 effectLPos;
    [SerializeField, Header("右側に表示する場合の画像座標")]
    private Vector2 effectRPos;
    [SerializeField, Header("スチル画像オブジェクト")]
    private Image stillImage;

    private void OnEnable()
    {
        if (CinematicsManager.Instance != null)
        {
            if(CinematicsManager.Instance.GetEffectSide() == CinematicsManager.EffectSide.effL)
            {
                tween = stillImage.transform.DOLocalMove(effectLPos, 0f);
            }
            else
            {
                tween = stillImage.transform.DOLocalMove(effectRPos, 0f);
            }

            // 演出はここ。今後変更かも
            tween = stillImage.DOFade(1f, 0.5f);
        }
    }

    public override void FadeOut()
    {
        tween = stillImage.DOFade(0f, 0.5f).
            OnComplete(EndEffect);
    }
}
