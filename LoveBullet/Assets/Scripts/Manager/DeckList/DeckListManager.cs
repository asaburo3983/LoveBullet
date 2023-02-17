using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class DeckListManager : SingletonMonoBehaviour<DeckListManager>
{
    public enum Mode
    {
        None,
        PowerUp
    }
    public Mode mode;

    [SerializeField] List<int> startDecksId = new List<int>();
    [SerializeField] GameObject deckListCanvas;
    [SerializeField] GameObject cardPrefab;

    [SerializeField] Slider slider;
    [SerializeField] float sliderSpeed;
    [SerializeField] float cardsMoveSpeed;

    [SerializeField] Transform cardsParent;

    public static List<Card.Card> deckList=new List<Card.Card>();

    [SerializeField] float canvasFadeSpeed;
    [SerializeField] Vector2 cardOriginPos;
    [SerializeField] float cardDistX;
    [SerializeField] float cardDistY;

    int cardNum;
    int cardColumn;
    CanvasGroup canvasGroup;

    ReactiveProperty<Vector3> cardsPos=new ReactiveProperty<Vector3>();

    List<Card.CanvasCard> viewCard=new List<Card.CanvasCard>();
    private void Awake()
    {
        if(SingletonCheck(this, true))
        {
            //ゲーム開始時は消しておく
            deckList.Clear();
            //初期デッキの作成
            InitializeStartDeck();
        }
        canvasGroup = deckListCanvas.GetComponent<CanvasGroup>();

        DisableCanvas();
        Debug.Log(deckList.Count);

    }
    List<GameObject> cardsOBJ=new List<GameObject>();
    void Start()
    {

        cardsPos.Subscribe(x => MoveCards()).AddTo(this);
        CardCreate();
    }
    void CardCreate()
    {
        //カードを生成する
        cardNum = deckList.Count;
        cardColumn = cardNum / 6;

        for (int i = 0; i < cardNum; i++)
        {
            var pos = new Vector3(cardOriginPos.x + ((i % 6) * cardDistX), cardOriginPos.y - i / 6 * cardDistY, 0);
            var obj = Instantiate(cardPrefab, pos, Quaternion.identity, cardsParent);

            cardsOBJ.Add(obj);
            viewCard.Add(obj.GetComponent<Card.CanvasCard>());
            viewCard[i].powerUp.deckListID = i;
            viewCard[i].Initialize(deckList[i].state);
        }

    }
    private void Update()
    {
        MoveSlider();


    }
    void MoveSlider()
    {
        //ホイール操作で移動させる処理
        var scroll = Input.mouseScrollDelta.y;
        slider.value -= scroll * ((1.0f/cardColumn)*sliderSpeed);

        //スライダーに応じで位置を移動させる
        cardsPos.Value = new Vector3(
            cardOriginPos.x,
            cardOriginPos.y + (cardColumn * cardDistY * slider.value),
            0);
        
    }
    Tween tw;
    void MoveCards()
    {
        if (tw != null)
        {
            tw.Kill(true);
        }
        tw = cardsParent.transform.DOMoveY(cardsPos.Value.y, cardsMoveSpeed).OnComplete(() => tw = null);
    }
    /// <summary>
    /// 初期デッキ作成
    /// </summary>
    void InitializeStartDeck()
    {
        int i = 0;
        foreach (var cardId in startDecksId)
        {
            Card.Card.State state = Card.Search.GetCard(cardId);
            Card.Card card = new Card.Card();
            card.SetState(state);
            deckList.Add(card);
            deckList[i].powerUp.deckListID = i;
            i++;
        }
    }


    public void ActiveCanvas()
    {
        //強化時は閉じれないようにしておく
        if (Mode.PowerUp == mode) { return; }

        if (deckListCanvas.active)
        {
            DisableCanvas();
        }
        else
        {
            EnableCanvas();
        }
    }
    public void EnableCanvas()
    {

        //ピックアップされているカードがあれば消しておく
        CancelPickUpCard();

        //カンバスをフェードで表示する
        deckListCanvas.SetActive(true);

        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 1.0f, canvasFadeSpeed);

        //カードを生成する

        //カードのポジションをセット
        for (int i = 0; i < cardNum; i++)
        {
            var pos = new Vector3(cardOriginPos.x + ((i % 6) * cardDistX), cardOriginPos.y - i / 6 * cardDistY, 0);
            cardsOBJ[i].transform.position = pos;
            // obj.transform.position = pos;
        }
    }
    public void DisableCanvas()
    {
        //強化時は閉じれないようにしておく
        if (Mode.PowerUp == mode) { return; }

        //カンバスをフェードで非表示する
        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 0.0f, canvasFadeSpeed).OnComplete(() => deckListCanvas.SetActive(false));
    }
    public void EnableCanvas_PowerUp()
    {
        //パワーアップモードで起動
        EnableCanvas();
        mode = Mode.PowerUp;
    }
    GameObject pickupCard;
    [SerializeField]Vector3 pickupCardPos;
    [SerializeField]Vector3 pickupCardSize;
    CanvasGroup pickupCardCanvasGroup;
    [SerializeField] float pickupCardFadeSpeed;
    Tween pickupTW;

    [SerializeField] GameObject pickupBackGround;
    public void PickUpCard(bool active,Card.Card.State state, Card.Card.PowerUp powerUp)
    {
        if (deckListCanvas.active == false) { return; }

        CancelPickUpCard();

        if (active)
        {
            if (pickupTW != null) { pickupTW.Kill(true); }

            //キャンバスに大きくカードを表示する
            pickupCard = Instantiate(cardPrefab, pickupCardPos, Quaternion.identity, deckListCanvas.transform);
            pickupCard.transform.localScale = pickupCardSize;
            var pCard = pickupCard.GetComponent<Card.CanvasCard>();
            pCard.Initialize(state);
            pCard.SetPowerUp(powerUp);

            //イベントトリガーを削除して表示ようにする
            EventTrigger trg = pickupCard.GetComponent<EventTrigger>();
            trg.triggers.RemoveRange(0, trg.triggers.Count);
            //フェードで表示
            pickupCardCanvasGroup = pickupCard.GetComponent<CanvasGroup>();
            pickupCardCanvasGroup.alpha = 0.0f;
            pickupTW = DOTween.To(() => pickupCardCanvasGroup.alpha, (x) => pickupCardCanvasGroup.alpha = x, 1.0f, pickupCardFadeSpeed);

            pickupBackGround.SetActive(true);
            pickupBackGround.GetComponent<Image>().DOFade(0.7f, pickupCardFadeSpeed);
            Debug.Log("ID" + pickupCard.GetComponent<Card.CanvasCard>().powerUp.deckListID);
        }
        else
        {
            CancelPickUpCard();
        }
    }
    public void CancelPickUpCard()
    {
        if (pickupCard != null)
        {
            if (pickupTW != null) { pickupTW.Kill(true); }

            pickupTW = DOTween.To(() => pickupCardCanvasGroup.alpha, (x) => pickupCardCanvasGroup.alpha = x, 0.0f, pickupCardFadeSpeed).OnComplete(() => Destroy(pickupCard));
            pickupBackGround.SetActive(false);
            pickupBackGround.GetComponent<Image>().DOFade(0.0f, pickupCardFadeSpeed);
        }
    }
    [SerializeField]int powerUpAT;
    [SerializeField]int powerUpDF;
    [SerializeField]int powerUpAP;

    public void PowerUp()
    {
        if (Mode.PowerUp != mode) { return; }
        //カードを指定の値強化する(ステートを変更する)
        foreach(var st in deckList)
        {
            var pCard = pickupCard.GetComponent<Card.CanvasCard>();
            //選択したカードとデッキリストIDが同じ場合強化する
            if (st.powerUp.deckListID== pCard.powerUp.deckListID)
            {
                //デッキリスト内のカードに値を適応
                if (st.powerUp.AP > 0)
                {
                    st.powerUp.AP -= powerUpAP;
                }
                st.powerUp.AT += powerUpAP;
                st.powerUp.DF += powerUpDF;
                st.SetText();

                //ピックアップされているカードに値を適応
                pCard.powerUp = st.powerUp;
                pCard.SetText();

                //描画されているカードリストに値を適応
                foreach (var vc in viewCard)
                {
                    if(vc.powerUp.deckListID== pCard.powerUp.deckListID)
                    {
                        vc.powerUp = st.powerUp;
                        vc.SetText();
                        break;
                    }
                }
                break;
            }
        }

        //TODO　演出を行う

        //強化を閉じる
        mode = Mode.None;
        DisableCanvas();
       
    }
}