using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUI : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 6; i++) {
            var _card = transform.GetChild(i).GetComponent<Card.Card>();
            _card.ui.AP.GetComponent<TextMesh>().text = Card.Fight.instance.GunInCards[i].AP.ToString();
            _card.ui.AT.GetComponent<TextMesh>().text = Card.Fight.instance.GunInCards[i].AT.ToString();
            _card.ui.DF.GetComponent<TextMesh>().text = Card.Fight.instance.GunInCards[i].DF.ToString();

            _card.ui.name.GetComponent<TextMesh>().text = Card.Fight.instance.GunInCards[i].name;
            _card.ui.explanation.GetComponent<TextMesh>().text = Card.Fight.instance.GunInCards[i].explanation;

        }
    }
}
