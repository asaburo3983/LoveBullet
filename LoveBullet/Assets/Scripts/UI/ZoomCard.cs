using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCard : MonoBehaviour
{
    [SerializeField] Card.Card card;
    public void OnClick()
    {
        Destroy(transform.parent.gameObject);
    }

    public void Initialize(Card.Card.State _state)
    {
        card.Initialize(_state);
    }
}
