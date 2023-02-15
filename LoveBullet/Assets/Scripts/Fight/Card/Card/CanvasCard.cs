using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Card
{
    public class CanvasCard : Card
    {

        [SerializeField]Vector2 originSize = Vector2.one;
        [SerializeField]Vector2 bigSize;
        [SerializeField] float duration;
        Tween tw;

        DeckListManager dlMana;
        public void SetSize(Vector2 _origin,Vector2 _big)
        {
            originSize = _origin;
            bigSize = _big;
        }

        public void PointEnter()
        {
            if (tw != null)
            {
                tw.Kill();
            }
            tw = transform.DOScale(bigSize, duration).OnComplete(() => tw = null);
        }
        public void PointExit()
        {
            if (tw != null)
            {
                tw.Kill();
            }
            tw = transform.DOScale(originSize, duration).OnComplete( () => tw = null);
        }
        public void PointClick()
        {
            dlMana = DeckListManager.instance;
            dlMana.PickUpCard(true, state,powerUp);
        }
        public void Push()
        {
            ResultManager.instance.GetCard(state,this.gameObject);
        }
    }
}