using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class DeckListManager : MonoBehaviour {

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject cardBase;
    [SerializeField] GameObject list;

    [SerializeField, Header("�����ʒu�ݒ�")] Vector2 cardLeftUpPos;
    [SerializeField] Vector2 cardPosDist;
    
    [SerializeField, Header("�J�[�h�ݒ�")] float cardSize = 0.6f;
    [SerializeField] float bigSize = 1.0f;

    [SerializeField, Header("�X�N���[���ݒ�")] float scrollSensi;
    [SerializeField] float space;

    FloatReactiveProperty scrollRatio = new FloatReactiveProperty();
    public FloatReactiveProperty ScrollRatio => scrollRatio;

    InputSystem input;

    BoolReactiveProperty openCanvas = new BoolReactiveProperty(false);

    [SerializeField] GameObject zoomCard;


    enum Sort
    {
        Rarity = 0,
        Damage,
        Def,
        Id,
        Obtain
    }
    // Start is called before the first frame update

    Card.Fight fight;

    ReactiveProperty<int> deckCount = new ReactiveProperty<int>();
    [SerializeField]List<GameObject> cardListObject = new List<GameObject>();

    int select = 0;

    private void Awake()
    {
        fight = Card.Fight.instance;

        deckCount.Subscribe(x => {
            //�����f�[�^���N���A����
            //�f�b�L���X�g���ɂ���J�[�h�𐶐�
            for (int i = 0; i < cardListObject.Count; i++) {
                Destroy(cardListObject[i]);
            }
            cardListObject.Clear();

            foreach (var card in fight.deckList) {
                var obj = Instantiate(cardBase, list.transform);
                var sc = obj.GetComponent<Card.CanvasCard>();
                sc.Initialize(card);
                cardListObject.Add(obj);

                obj.transform.localScale = new Vector3(cardSize, cardSize, 1);
                obj.GetComponent<Card.CanvasCard>().SetSize(new Vector2(cardSize, cardSize), new Vector2(bigSize, bigSize));

            }
            //�I�u�W�F�N�g�̈ʒu���\�[�g����
            SortPos(Sort.Obtain);
        }).AddTo(this);

        openCanvas.Subscribe(x => canvas.SetActive(x)).AddTo(this);
    }

    void Start()
    {
        input = InputSystem.instance;

    }


    // Update is called once per frame
    void Update()
    {
        // �J���Ă���Ƃ��ȊO�͏������Ȃ�
        if (!openCanvas.Value) return;

        // �f�b�L�T�C�Y�̍X�V
        deckCount.Value = fight.deckList.Count;

        // �X�N���[������
        float _scroll = input.GetValue("Player", "Scroll").y;
        var _pos = list.transform.localPosition;
        float max = Mathf.Abs(space + cardPosDist.y * ((cardListObject.Count / 6) - 1));

        _pos.y = Mathf.Clamp(_pos.y + (_scroll * scrollSensi), 0, max);

        scrollRatio.Value = _pos.y / max;

        list.transform.localPosition = _pos;
    }


    //�f�b�L���̃J�[�h��\������
    void SortPos(Sort sort)
    {
        bool reverce = false;

        if (sort == Sort.Obtain) {
            // �������Ȃ��@�f�b�L�̏��Ԓʂ�
            for (int i = 0; i < fight.deckList.Count; i++) {
                cardListObject[i].GetComponent<Card.CanvasCard>().Initialize(fight.deckList[i]);
            }

            if (select == -1) {
                cardListObject.Reverse();
                select = 0;
            }
            else {
                select = -1;
            }
        }
        else if (sort == Sort.Rarity) {

            if (select == 1) {
                reverce = true;
                select = 0;
            }
            else {
                select = 1;
            }

            var query = cardListObject
                .OrderBy(value => value.GetComponent<Card.CanvasCard>().STATE.rank * (reverce ? -1 : 1))
                .ThenBy(value => value.GetComponent<Card.CanvasCard>().STATE.id);

            cardListObject = query.ToList();
        }
        else if (sort == Sort.Damage) {

            if (select == 2) {
                reverce = true;
                select = 0;
            }
            else {
                select = 2;
            }

            var query = cardListObject
                .OrderBy(value => value.GetComponent<Card.CanvasCard>().STATE.Damage * (reverce ? -1 : 1))
                .ThenBy(value => value.GetComponent<Card.CanvasCard>().STATE.id);

            cardListObject = query.ToList();
        }
        else if (sort == Sort.Def) {

            if (select == 3) {
                reverce = true;
                select = 0;
            }
            else {
                select = 3;
            }

            var query = cardListObject
                .OrderBy(value => value.GetComponent<Card.CanvasCard>().STATE.buff[(int)BuffEnum.Bf_Diffence] * (reverce ? -1 : 1))
                .ThenBy(value => value.GetComponent<Card.CanvasCard>().STATE.id);

            cardListObject = query.ToList();
        }
        else if (sort == Sort.Id) {
            if (select == 4) {
                reverce = true;
                select = 0;
            }
            else {
                select = 4;
            }
            var query = cardListObject.OrderBy(value => value.GetComponent<Card.CanvasCard>().STATE.id * (reverce ? -1 : 1));
            cardListObject = query.ToList();
        }

        //�J�[�h��z��ɓ����Ă��鏇�ԂŔz�u����        
        SortCard();
    }

    void SortCard()
    {
        for (int i = 0; i < cardListObject.Count; i++) {
            var pos = new Vector2(
                cardLeftUpPos.x + (i % 6) * cardPosDist.x,
                cardLeftUpPos.y + (i / 6) * cardPosDist.y
                );

            cardListObject[i].transform.localPosition = pos;
        }
    }

    public void OnActive()
    {
        openCanvas.Value = true;
    }
    public void OffActive()
    {
        openCanvas.Value = false;
    }

    public void SortRarity()
    {
        SortPos(Sort.Rarity);
    }
    public void SortDamage()
    {
        SortPos(Sort.Damage);
    }
    public void SortDef()
    {
        SortPos(Sort.Def);
    }
    public void SortID()
    {
        SortPos(Sort.Id);
    }

    public void SortObtain()
    {
        SortPos(Sort.Obtain);
    }

}