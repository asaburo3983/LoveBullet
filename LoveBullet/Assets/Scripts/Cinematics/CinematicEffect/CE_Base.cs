using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Cinematic Effect: Base Class
public class CE_Base : MonoBehaviour
{
    public enum Type
    {
        Oneshot,
        Stay
    }
    [SerializeField, Header("エフェクトの種類")]
    protected Type effectType;

    [SerializeField, Header("エフェクトの発生時間")]
    protected float effectDuration;

    // Tweenコンポーネント
    protected Tween tween;

    // エフェクト終了 (普通verとコルーチンver)
    protected void EndEffect()
    {
        tween.Kill();
        Destroy(gameObject);
    }
    protected IEnumerator EndEffectCrt(float _time)
    {
        yield return new WaitForSeconds(_time);
        EndEffect();
    }

    public Type GetEffectType()
    {
        return effectType;
    }

    virtual public void FadeOut() { }
}
