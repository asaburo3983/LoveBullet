using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightInput : MonoBehaviour
{
    Card.Fight fight;
    private void Start()
    {
        fight = Card.Fight.instance;
    }
    // Start is called before the first frame update
    public void Shot()
    {
        Card.Fight.instance.shotInput = true;
    }
    public void Reload()
    {
        Card.Fight.instance.reloadInput = true;
    }
    public void Cocking()
    {
        Card.Fight.instance.cockingInput = true;
    }
    public void TargetPlus()
    {
        fight.shotTarget++;
        if (fight.enemyObjects.Count <= fight.shotTarget)
        {
            fight.shotTarget = 0;
        }
    }
    public void TargetMinus()
    {
        fight.shotTarget--;
        if (fight.shotTarget < 0)
        {
            fight.shotTarget = fight.enemyObjects.Count - 1;
        }
    }


}
