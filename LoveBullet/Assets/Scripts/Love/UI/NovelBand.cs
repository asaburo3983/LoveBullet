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
        rect = GetComponent<RectTransform>();
        nm = NovelManager.instance;
        nm.isNovel.Subscribe(x => { MoveUI(x); }).AddTo(this);
    }
    void Start()
    {
       
       
        
    }

    Tween bandTW;
    void MoveUI(bool isNovel)
    {
        if (isNovel)
        {
            if(bandTW!=null)bandTW.Kill();
            bandTW=rect.DOAnchorPos(endPos, needTime);
        }
        else
        {
            if (bandTW != null) bandTW.Kill();
            bandTW = rect.DOAnchorPos(startPos, needTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
