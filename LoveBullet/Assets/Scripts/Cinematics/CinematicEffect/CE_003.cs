using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

// Cinematic Effect 003 : �X�`���\��(1)
public class CE_003 : CE_Base
{
    [SerializeField, Header("�����ɕ\������ꍇ�̉摜���W")]
    private Vector2 effectLPos;
    [SerializeField, Header("�E���ɕ\������ꍇ�̉摜���W")]
    private Vector2 effectRPos;
    [SerializeField, Header("�X�`���摜�I�u�W�F�N�g")]
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

            // ���o�͂����B����ύX����
            tween = stillImage.DOFade(1f, 0.5f);
        }
    }

    public override void FadeOut()
    {
        tween = stillImage.DOFade(0f, 0.5f).
            OnComplete(EndEffect);
    }
}
