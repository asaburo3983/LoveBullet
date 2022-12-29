using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightButtonFunction : MonoBehaviour
{
    Card.Fight fight;
    void Start()
    {
        fight = Card.Fight.instance;
    }

    public void Fire()
    {
        fight.Fire();
    }
    public void Reload()
    {
        fight.Reload();
    }
    public void Cocking()
    {
        fight.Cocking();
    }
}
