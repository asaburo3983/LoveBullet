using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Card
{
    public class ResultCard : Card
    {

        [SerializeField] Vector2 originSize = Vector2.one;
        [SerializeField] Vector2 bigSize;
        [SerializeField] float duration;
        Tween tw;

        DeckListManager dlMana;
        public void SetSize(Vector2 _origin, Vector2 _big)
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
            tw = transform.DOScale(originSize, duration).OnComplete(() => tw = null);
        }
        public void PointClick()
        {
            var resultMana = ResultManager.instance;
            if (resultMana.selectedCard) { return; }
            resultMana.selectedCard = true;

            //カードを追加する
            dlMana = DeckListManager.instance;
            DeckListManager.deckList.Add(this);
            //dlMana.deckList.Add(this);
            //リザルトキャンバスから削除する
            Destroy(this.gameObject);
        }
    }
}