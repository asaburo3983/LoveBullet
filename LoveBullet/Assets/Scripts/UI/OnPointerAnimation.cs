using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OnPointerAnimation : MonoBehaviour
{
    Sequence onPointer = null;
    [SerializeField] float animationTime = 0.2f;

    public void OnPointer()
    {
        if (onPointer != null) onPointer.Kill(true);
        onPointer = DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), animationTime).SetLoops(2, LoopType.Yoyo))
            .Join(transform.DORotate(new Vector3(0, 0, 20), animationTime).SetLoops(2, LoopType.Yoyo))
            .OnComplete(() => onPointer = null);
    }
}
