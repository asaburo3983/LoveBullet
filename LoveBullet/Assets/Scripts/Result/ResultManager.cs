using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;
public class ResultManager : SingletonMonoBehaviour<ResultManager>
{

    [SerializeField] GameObject resultCanvas;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float canvasFadeSpeed;

    [SerializeField] GameObject card;
    [SerializeField] List<Transform> cardPos;

    List<GameObject> cards = new List<GameObject>();

    public bool selectedCard = false;
    private void Awake()
    {
        SingletonCheck(this);
    }
    private void OnEnable()
    {

        
    }
    Card.Card.State GetRandomCard()
    {
        return Card.Search.GetCard(1);
    }
    private void Start()
    {


    }
    private void Update()
    {
        
    }
    public void EnableCanvas()
    {
        //カンバスをフェードで表示する
        resultCanvas.SetActive(true);

        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 1.0f, canvasFadeSpeed);

        selectedCard = false;
        //カードを生成する
        foreach (var pos in cardPos)
        {
            var obj = Instantiate(card, pos.position, Quaternion.identity, resultCanvas.transform);
            obj.transform.GetComponent<Card.Card>().Initialize(GetRandomCard());
            cards.Add(obj);
        }
    }
    public void DisableCanvas()
    {
        //カンバスをフェードで非表示する
        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 0.0f, canvasFadeSpeed).OnComplete(() => resultCanvas.SetActive(false));
    }

    /// <summary>
    /// ボタンでシーン移動
    /// </summary>
    /// <param name="mode"></param>
    public void MoveNovel_Novel()
    {
        //とりあえず移動
        NovelManager.instance.novelMode = NovelManager.NovelMode.Novel;
        SceneManager.LoadScene("Novel");
    }
    public void MoveNovel_Rest()
    {
        //とりあえず移動
        NovelManager.instance.novelMode = NovelManager.NovelMode.Rest;
        SceneManager.LoadScene("Novel");
    }
    public void MoveNovel_Shop()
    {
        //とりあえず移動
        NovelManager.instance.novelMode = NovelManager.NovelMode.Shop;
        SceneManager.LoadScene("Novel");
    }

    public void MoveFight()
    {
        FightManager.floor++;
        SceneManager.LoadScene("Fight");
    }
}
