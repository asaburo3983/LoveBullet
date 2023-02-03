using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CardListScrollBar : MonoBehaviour
{
    [SerializeField] DeckListManager deckList;
    [SerializeField] float dist;

    // Start is called before the first frame update
    void Start()
    {
        deckList.ScrollRatio.Subscribe(x => {
            var _pos = transform.localPosition;
            _pos.y = dist * x - dist * 0.5f;
            transform.localPosition = _pos;
        }).AddTo(this);
    }

}
