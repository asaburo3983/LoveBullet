using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ResultManager : SingletonMonoBehaviour<ResultManager>
{
    [SerializeField, Header("�f�o�b�O�p")]
    ReactiveProperty<bool> startResult_Debug = new ReactiveProperty<bool>();

    public ReactiveProperty<bool> result = new ReactiveProperty<bool>();
    public bool isResult => result.Value;
    [SerializeField, Header("�L�����o�X�ݒ�")]
    public GameObject resultCanvas;
    [SerializeField] float canvasFadeTime;
    [SerializeField] GameObject cardSelectCanvas;
    [SerializeField] GameObject lovePointCanvas;
    [SerializeField] float canvasMoveSpeed;

    [SerializeField,Header("�����N���Ƃ̏o�銄��")]
    List<int> cardRankPercent = new List<int>();

    [SerializeField, Header("�^�[���{�[�i�X")]
    float turnBounus = 1.5f;

    [SerializeField, Header("�擾�ł���J�[�h��")]
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


            Debug.LogWarning("���݁A�^�[���{�[�i�X�A�擾�ł���J�[�h��ސ��A�擾�ł���J�[�h���͌Œ肳��Ă��܂�");
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
        //���U���g��ʂ��t�F�[�h�����ĕ\������
        resultCanvas.GetComponent<CanvasGroup>().DOFade(1, canvasFadeTime);
    }
    void EndResult_Effect()
    {
        //��ʂ����̉摜�Ńt�F�[�h������ �t�F�[�h�����͌�ق�
        //�V�[�����ړ�����@TODO �v���g�^�C�v�ł͐퓬�V�[���ɖ߂�悤�ɂ��Ă���
        SceneManager.LoadScene(moveScene);

       
    }

    /// <summary>
    /// �퓬�I�����ɌĂяo���ă��U���g�������s��
    /// </summary>
    /// <param name="_lovePoint"></param>
    /// <param name="_turnBounus"></param>
    public void StartResult(int _lovePoint, bool _turnBounus = false)
    {
        result.Value = true;

        mode.Value = false;

        //�擾�ł���J�[�h��ǉ�
        SetCards();

        //�擾LovePoint��ݒ肷��
        SetLovePoint(_lovePoint,_turnBounus);

        StartResult_Effect();
    }

    /// <summary>
    /// ���u�|�C���g�U�蕪���I�����ɌĂԃ��U���g�I������
    /// </summary>
    public void EndResult()
    {
        result.Value = false;
        EndResult_Effect();
    }
    /// <summary>
    /// ���[�h��؂�ւ���
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
    /// �擾�ł���J�[�h��ݒ肷��
    /// </summary>
    void SetCards()
    {
        for (int i = 0; i < getCardValueQuantity; i++)
        {
            int cardRank = 0;
            //�m���Ń����N���z���w����
            var percent = Random.RandomRange(1, 101);
            for(int h=0;h< cardRankPercent.Count; h++)
            {
                if (percent <= cardRankPercent[h])
                {
                    cardRank = h + 1;
                }
            }
            //�w�肳�ꂽ�����N�Ɠ����J�[�h���擾���郊�X�g�ɒǉ�
            var rankTmp = 0;
            while (rankTmp != cardRank)
            {
                var cardRange = CacheData.instance.cardStates.Count;
                var num = Random.RandomRange(1, cardRange);
                rankTmp = CacheData.instance.cardStates[num].rank;
                //�����N�������ꍇ�J�[�h�ɒǉ����ďI��
                if (rankTmp == cardRank)
                {
                    getCards.Add(CacheData.instance.cardStates[num]);
                   
                }
            }

            //todo�Ƃ肠�����X�e�[�g������
            canvasCardObjects[i].GetComponent<Card.CanvasCard>().Initialize(getCards[i]);
        }
    }

    /// <summary>
    /// �擾�ł��郉�u�|�C���g��ݒ肷��
    /// </summary>
    /// <param name="_lovePoint"></param>
    /// <param name="_turnBounus"></param>
    void SetLovePoint(int _lovePoint,bool _turnBounus)
    {
        //���Z���邱�Ƃł����ĐU�蕪���Ȃ��I������^����
        getLovePoint.Value = _lovePoint;
        getLovePointMulti.Value += getLovePoint.Value;
        getLovePointMulti.Value = _turnBounus ? (int)((float)getLovePointMulti.Value * turnBounus) :getLovePointMulti.Value;
    }
}
