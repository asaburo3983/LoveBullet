using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CommandMove : MonoBehaviour
{
    [SerializeField]float moveX;
    [SerializeField]float speed;

    float originePosX;

    Card.Fight fight;
    void Start()
    {
        originePosX = transform.position.x;
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

    Tween tw;
    public void OnEnterMove()
    {
        tw=transform.DOMoveX(moveX, speed);
    }
    public void OnExitMove()
    {
        transform.DOMoveX(originePosX, speed);
    }
}
