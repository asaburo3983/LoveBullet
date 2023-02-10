using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.UI;
public class NovelBlackBackGround : MonoBehaviour
{
    NovelManager nm;
    SpriteRenderer sr;
    [SerializeField] float fadeAlpha;
    [SerializeField] float fadeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        nm = NovelManager.instance;
        nm.isNovel.Subscribe(x => { FadeImage(x); }).AddTo(this);
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FadeImage(bool enable)
    {
        if (enable)
        {
            DOTween.ToAlpha(() => sr.color, a => sr.color = a, fadeAlpha, fadeSpeed);

        }
        else
        {
            DOTween.ToAlpha(() => sr.color, a => sr.color = a, 0, fadeSpeed);

        }

    }
}
