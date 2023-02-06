using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Cinematic Effect 001 : キャラクターが驚く
public class CE_001 : CE_Base
{
    [SerializeField, Header("ジャンプ力")]
    private float jumpValue;
    [SerializeField, Header("揺れの威力")]
    private float shakeValue;

    void OnEnable()
    {
        if(CinematicsManager.Instance != null &&
            CinematicsManager.Instance.GetEffectTargetObj() != null)
        {
            GameObject target = CinematicsManager.Instance.GetEffectTargetObj();

            tween = target.transform.DOShakePosition(effectDuration, shakeValue).
                OnComplete(EndEffect);
            //tween = target.transform.DOJump(target.transform.position, jumpValue, 1, effectDuration);
        }
    }
}
