using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.UI;

public class NovelBand : MonoBehaviour
{
    NovelManager nm;
    RectTransform rect;
    [SerializeField] Vector2 startPos;
    [SerializeField] Vector2 endPos;
    [SerializeField] float needTime;

    // Start is called before the first frame update
    private void Awake()
    {
        nm = NovelManager.instance;
        nm.isNovel.Subscribe(x => { MoveUI(x); }).AddTo(this);
        rect = GetComponent<RectTransform>();

    }
    void Start()
    {
    }

    void MoveUI(bool isNovel)
    {
        if (isNovel)
        {
            rect.DOAnchorPos(endPos, needTime);
        }
        else
        {
            rect.DOAnchorPos(startPos, needTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
