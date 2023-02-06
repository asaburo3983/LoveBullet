using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OnPointerAnimation : MonoBehaviour
{
    Sequence onPointer = null;
    [SerializeField] float animationTime = 0.2f;
    [SerializeField] float coolTime = 0.5f;
    bool enter = false;

    public void OnPointerEnter()
    {
        enter = true;        
        onPointer = DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), animationTime).SetLoops(2, LoopType.Yoyo))
            .Join(transform.DORotate(new Vector3(0, 0, 20), animationTime).SetLoops(2, LoopType.Yoyo))
            .OnComplete(() => onPointer = null);
    }

    public void OnPointerExit()
    {
        enter = false;
        if (onPointer != null) onPointer.Kill(true);
    }

    private void Update()
    {
        if (!enter) return;
        if (onPointer != null) return;
        onPointer = DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), animationTime).SetLoops(2, LoopType.Yoyo))
            .Join(transform.DORotate(new Vector3(0, 0, 20), animationTime).SetLoops(2, LoopType.Yoyo))
            .SetDelay(coolTime)
            .OnComplete(() => onPointer = null);
    }

}
