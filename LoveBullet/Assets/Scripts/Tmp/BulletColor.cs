using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletColor : MonoBehaviour
{
    SpriteRenderer sr;
    public int num;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var cards = Card.Fight.instance.GunInCards;
        if (cards.Count > 0 && cards[num].id != 0)
        {
            sr.color = Color.green;
        }
        else
        {
            sr.color = Color.grey;
        }
    }
}
