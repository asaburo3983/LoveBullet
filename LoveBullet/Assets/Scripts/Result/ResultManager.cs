using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ResultManager : SingletonMonoBehaviour<ResultManager>
{
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject cards;
    [SerializeField] ResultEvent ev;
    bool isResult;
    public bool IsResult =>isResult;

    private void Awake()
    {
        SingletonCheck(this);
    }

    private void Start()
    {
        canvas.gameObject.SetActive(false);
        canvas.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void StartResult()
    {
        canvas.gameObject.SetActive(true);
        var group = canvas.GetComponent<CanvasGroup>();
        DOTween.To(
            () => group.alpha,
            x => group.alpha = x,
            1.0f,
            1.0f
            );
        cards.gameObject.SetActive(true);
    }

    public void GetCard(Card.Card.State _state,GameObject _obj)
    {
        Debug.Log("hahaha");
        cards.gameObject.SetActive(false);
//        Card.Fight.instance.deckList.Add(_state);
    }
}
