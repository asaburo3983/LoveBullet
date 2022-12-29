using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : MonoBehaviour
{
    Card.Fight fight;

    // Start is called before the first frame update
    void Start()
    {
        fight = Card.Fight.instance;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 6; i++) {
            //transform.GetChild(i).gameObject.SetActive(fight.GunInCards[i].id != 0);
        }
    }
}
