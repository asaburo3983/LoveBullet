using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Card
{
    public class CanvasCard : Card
    {

        Vector2 originSize;
        [SerializeField]Vector2 bigSize;
        [SerializeField] float duration;
        Tween tw;

        // Start is called before the first frame update
        void Start()
        {
            originSize = Vector2.one;

        }

        // Update is called once per frame
        void Update()
        {

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
        public void Push()
        {
            var result = ResultManager.instance;
            if (result.Mode == false) {
                return;
            }
            Fight.instance.deckList.Add(state);
            ResultManager.instance.ChangeMode();
        }
    }
}