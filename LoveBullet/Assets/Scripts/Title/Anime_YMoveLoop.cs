using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Anime_YMoveLoop : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] float plusDistY;
    [SerializeField] float speed;

    float localTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        localTime += Time.deltaTime;
        if (localTime > time)
        {
            localTime = 0;
            transform.DOMoveY(plusDistY,speed).SetLoops(2, LoopType.Yoyo);
        }

    }
}
