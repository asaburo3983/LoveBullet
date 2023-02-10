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
    [SerializeField, Header("�G�t�F�N�g�̎��")]
    protected Type effectType;

    [SerializeField, Header("�G�t�F�N�g�̔�������")]
    protected float effectDuration;

    // Tween�R���|�[�l���g
    protected Tween tween;

    // �G�t�F�N�g�I�� (����ver�ƃR���[�`��ver)
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
