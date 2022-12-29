using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUI : MonoBehaviour
{
    [SerializeField]List<Card.Card> cards = new List<Card.Card>();
    // Update is called once per frame
    void Update()
    {
        //TODOd‚¢ˆ—‚ğ‚µ‚Ä‚¢‚é‚Ì‚ÅŒã‚ÉC¬
        for(int i = 0; i < 6; i++) {

            cards[i].Initialize(Card.Fight.instance.GunInCards[i]);
        }
    }
}
