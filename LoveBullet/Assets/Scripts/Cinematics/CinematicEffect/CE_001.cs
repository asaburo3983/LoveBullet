using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Cinematic Effect 001 : �L�����N�^�[������
public class CE_001 : CE_Base
{
    [SerializeField, Header("�W�����v��")]
    private float jumpValue;
    [SerializeField, Header("�h��̈З�")]
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
