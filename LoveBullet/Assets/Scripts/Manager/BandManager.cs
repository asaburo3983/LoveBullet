using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandManager : MonoBehaviour
{
    [SerializeField] DeckListManager deckList;

    public void InstantateDeckList()
    {
        deckList.OnActive();
    }
}
