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
            //�Q�[���J�n���͏����Ă���
            deckList.Clear();
            //�����f�b�L�̍쐬
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
        //�J�[�h�𐶐�����
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
        //�z�C�[������ňړ������鏈��
        var scroll = Input.mouseScrollDelta.y;
        slider.value -= scroll * ((1.0f/cardColumn)*sliderSpeed);

        //�X���C�_�[�ɉ����ňʒu���ړ�������
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
    /// �����f�b�L�쐬
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
        //�������͕���Ȃ��悤�ɂ��Ă���
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

        //�s�b�N�A�b�v����Ă���J�[�h������Ώ����Ă���
        CancelPickUpCard();

        //�J���o�X���t�F�[�h�ŕ\������
        deckListCanvas.SetActive(true);

        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 1.0f, canvasFadeSpeed);

        //�J�[�h�𐶐�����

        //�J�[�h�̃|�W�V�������Z�b�g
        for (int i = 0; i < cardNum; i++)
        {
            var pos = new Vector3(cardOriginPos.x + ((i % 6) * cardDistX), cardOriginPos.y - i / 6 * cardDistY, 0);
            cardsOBJ[i].transform.position = pos;
            // obj.transform.position = pos;
        }
    }
    public void DisableCanvas()
    {
        //�������͕���Ȃ��悤�ɂ��Ă���
        if (Mode.PowerUp == mode) { return; }

        //�J���o�X���t�F�[�h�Ŕ�\������
        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 0.0f, canvasFadeSpeed).OnComplete(() => deckListCanvas.SetActive(false));
    }
    public void EnableCanvas_PowerUp()
    {
        //�p���[�A�b�v���[�h�ŋN��
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

            //�L�����o�X�ɑ傫���J�[�h��\������
            pickupCard = Instantiate(cardPrefab, pickupCardPos, Quaternion.identity, deckListCanvas.transform);
            pickupCard.transform.localScale = pickupCardSize;
            var pCard = pickupCard.GetComponent<Card.CanvasCard>();
            pCard.Initialize(state);
            pCard.SetPowerUp(powerUp);

            //�C�x���g�g���K�[���폜���ĕ\���悤�ɂ���
            EventTrigger trg = pickupCard.GetComponent<EventTrigger>();
            trg.triggers.RemoveRange(0, trg.triggers.Count);
            //�t�F�[�h�ŕ\��
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
        //�J�[�h���w��̒l��������(�X�e�[�g��ύX����)
        foreach(var st in deckList)
        {
            var pCard = pickupCard.GetComponent<Card.CanvasCard>();
            //�I�������J�[�h�ƃf�b�L���X�gID�������ꍇ��������
            if (st.powerUp.deckListID== pCard.powerUp.deckListID)
            {
                //�f�b�L���X�g���̃J�[�h�ɒl��K��
                if (st.powerUp.AP > 0)
                {
                    st.powerUp.AP -= powerUpAP;
                }
                st.powerUp.AT += powerUpAP;
                st.powerUp.DF += powerUpDF;
                st.SetText();

                //�s�b�N�A�b�v����Ă���J�[�h�ɒl��K��
                pCard.powerUp = st.powerUp;
                pCard.SetText();

                //�`�悳��Ă���J�[�h���X�g�ɒl��K��
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

        //TODO�@���o���s��

        //���������
        mode = Mode.None;
        DisableCanvas();
       
    }
}