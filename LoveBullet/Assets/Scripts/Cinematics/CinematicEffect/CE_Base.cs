using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Cinematic Effect: Base Class
public class CE_Base : MonoBehaviour
{
    [SerializeField, Header("�G�t�F�N�g�̔�������")]
    protected float effectDuration;

    // Tween�R���|�[�l���g
    protected Tween tween;

    // �G�t�F�N�g�I��
    protected void EndEffect()
    {
        tween.Kill();
        Destroy(gameObject);
    }
}
