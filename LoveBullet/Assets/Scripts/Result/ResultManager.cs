using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ResultManager : SingletonMonoBehaviour<ResultManager>
{
    [SerializeField, Header("デバッグ用")]
    ReactiveProperty<bool> startResult_Debug = new ReactiveProperty<bool>();

    public ReactiveProperty<bool> result = new ReactiveProperty<bool>();
    public bool isResult => result.Value;
    [SerializeField, Header("キャンバス設定")]
    public GameObject resultCanvas;
    [SerializeField] float canvasFadeTime;
    [SerializeField] GameObject cardSelectCanvas;
    [SerializeField] GameObject lovePointCanvas;
    [SerializeField] float canvasMoveSpeed;

    [SerializeField,Header("ランクごとの出る割合")]
    List<int> cardRankPercent = new List<int>();

    [SerializeField, Header("ターンボーナス")]
    float turnBounus = 1.5f;

    [SerializeField, Header("取得できるカード数")]
    int getCardQuantity = 1;
    [SerializeField]
    List<GameObject> canvasCardObjects = new List<GameObject>();

    [SerializeField, ReadOnly]
    int getCardValueQuantity = 3;

    [SerializeField] public ReactiveProperty<int> getLovePoint=new ReactiveProperty<int>();
    [SerializeField] public ReactiveProperty<int> getLovePointMulti = new ReactiveProperty<int>();

    ReactiveProperty<bool> mode=new ReactiveProperty<bool>();
    public bool Mode => mode.Value;
    
    List<Card.Card.State> getCards = new List<Card.Card.State>();

    [SerializeField]
    string moveScene;
    private void Awake()
    {
        if (SingletonCheck(this, true))
        {


            Debug.LogWarning("現在、ターンボーナス、取得できるカード種類数、取得できるカード数は固定されています");
            turnBounus = 1.5f;
            getCardQuantity = 1;
            getCardValueQuantity = 3;

            mode.Value = false;

            mode.Where(x => !x).Subscribe(x => CardMode()).AddTo(this);
            mode.Where(x => x).Subscribe(x => LoveMode()).AddTo(this);



        }
    }

    // Start is called before the first frame update
    void Start()
    {
        resultCanvas.SetActive(false);
        resultCanvas.GetComponent<CanvasGroup>().alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void StartResult_Effect()
    {
        resultCanvas.SetActive(true);
        //リザルト画面をフェードさせて表示する
        resultCanvas.GetComponent<CanvasGroup>().DOFade(1, canvasFadeTime);
    }
    void EndResult_Effect()
    {
        //画面を特定の画像でフェードさせる フェード処理は後ほど
        //シーンを移動する　TODO プロトタイプでは戦闘シーンに戻るようにしておく
        SceneManager.LoadScene(moveScene);

       
    }

    /// <summary>
    /// 戦闘終了時に呼び出してリザルト処理を行う
    /// </summary>
    /// <param name="_lovePoint"></param>
    /// <param name="_turnBounus"></param>
    public void StartResult(int _lovePoint, bool _turnBounus = false)
    {
        result.Value = true;

        mode.Value = false;

        //取得できるカードを追加
        SetCards();

        //取得LovePointを設定する
        SetLovePoint(_lovePoint,_turnBounus);

        StartResult_Effect();
    }

    /// <summary>
    /// ラブポイント振り分け終了時に呼ぶリザルト終了処理
    /// </summary>
    public void EndResult()
    {
        result.Value = false;
        EndResult_Effect();
    }
    /// <summary>
    /// モードを切り替える
    /// </summary>
    /// <param name="_mode"></param>
    public void ChangeMode()
    {
        mode.Value = !mode.Value;
    }
    void CardMode()
    {
        cardSelectCanvas.transform.DOMoveX(0, canvasMoveSpeed);
        lovePointCanvas.transform.DOMoveX(1920, canvasMoveSpeed);
    }
    void LoveMode()
    {
        cardSelectCanvas.transform.DOMoveX(-1920, canvasMoveSpeed);
        lovePointCanvas.transform.DOMoveX(0, canvasMoveSpeed);
    }

    /// <summary>
    /// 取得できるカードを設定する
    /// </summary>
    void SetCards()
    {
        for (int i = 0; i < getCardValueQuantity; i++)
        {
            int cardRank = 0;
            //確率でランク分布を指せる
            var percent = Random.RandomRange(1, 101);
            for(int h=0;h< cardRankPercent.Count; h++)
            {
                if (percent <= cardRankPercent[h])
                {
                    cardRank = h + 1;
                }
            }
            //指定されたランクと同じカードを取得するリストに追加
            var rankTmp = 0;
            while (rankTmp != cardRank)
            {
                var cardRange = CacheData.instance.cardStates.Count;
                var num = Random.RandomRange(1, cardRange);
                rankTmp = CacheData.instance.cardStates[num].rank;
                //ランクが同じ場合カードに追加して終了
                if (rankTmp == cardRank)
                {
                    getCards.Add(CacheData.instance.cardStates[num]);
                   
                }
            }

            //todoとりあえずステートを入れる
            canvasCardObjects[i].GetComponent<Card.CanvasCard>().Initialize(getCards[i]);
        }
    }

    /// <summary>
    /// 取得できるラブポイントを設定する
    /// </summary>
    /// <param name="_lovePoint"></param>
    /// <param name="_turnBounus"></param>
    void SetLovePoint(int _lovePoint,bool _turnBounus)
    {
        //加算することであえて振り分けない選択肢を与える
        getLovePoint.Value = _lovePoint;
        getLovePointMulti.Value += getLovePoint.Value;
        getLovePointMulti.Value = _turnBounus ? (int)((float)getLovePointMulti.Value * turnBounus) :getLovePointMulti.Value;
    }
}
