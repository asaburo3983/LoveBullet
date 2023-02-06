using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandManager : MonoBehaviour
{
    [SerializeField] DeckListManager deckList;
    [SerializeField] OptionManager option;


    public void InstantateDeckList()
    {
        deckList.OnActive();
        option.OffActive();
    }

    public void InstantateOption()
    {
        option.OnActive();
        deckList.OffActive();
    }

    public void OffActive()
    {
        option.OffActive();
        deckList.OffActive();
    }
}
