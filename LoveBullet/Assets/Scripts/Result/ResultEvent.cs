using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultEvent : MonoBehaviour
{
    public void Dive()
    {
        Card.Fight.instance.StartFight();
    }
}
