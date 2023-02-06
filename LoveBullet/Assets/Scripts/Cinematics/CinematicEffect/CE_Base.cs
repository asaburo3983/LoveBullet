using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Cinematic Effect: Base Class
public class CE_Base : MonoBehaviour
{
    [SerializeField, Header("エフェクトの発生時間")]
    protected float effectDuration;

    // Tweenコンポーネント
    protected Tween tween;

    // エフェクト終了
    protected void EndEffect()
    {
        tween.Kill();
        Destroy(gameObject);
    }
}
