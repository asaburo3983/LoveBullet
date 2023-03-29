using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class NovelManager : SingletonMonoBehaviour<NovelManager>
{

    CacheData_Novel cs;
    public ReactiveProperty<bool> isNovel = new ReactiveProperty<bool>();

    [System.Serializable]
    public enum NovelMode
    {
        Novel,
        Rest,
        Shop,
        MAX,
    }
    [SerializeField] public static NovelMode novelMode;
    [SerializeField] NovelMode debugStartNovelMode;
    [SerializeField] public static int chapterNum;

    public void SetNovelMode(NovelMode nm, int chapter = -1)
    {
        novelMode = nm;
        chapterNum = chapter;
    }

    [System.Serializable]
    public struct ModeState
    {
        public float playerStartPos;

        [Header("�x�e���Ɏg�p����l")]
        public float restHealPer;
        public float restMindHealPer;
        public int instanceDropObjectMin;
        public int instanceDropObjectMax;
        public List<GameObject> dropObject;
        public List<Transform> dropObjectPos;

        [Header("�V���b�v���Ɏg�p����l")]
        public List<GameObject> shopObject;
        public List<Transform> shopObjectPos;
    }
    [SerializeField] ModeState modeState;

    //�e�L�X�g�n
    [Header("�e�L�X�g�֘A")]
    bool stopInputForNextPage=false;
    int page =0;
    [SerializeField] TextMesh textL;
    [SerializeField] TextMesh textR;
    [SerializeField] TextMesh textC;
    [SerializeField] Color textColor;

    bool startText = false;
    float textAlpha = 0;
    [SerializeField] float fadeSpeed = 5;

    [SerializeField] float textStartPos;
    [SerializeField] float textEndPos;

    RectTransform textRectL;
    RectTransform textRectR;
    RectTransform textRectC;
    [SerializeField] float tatieFadeSpeed;
    [SerializeField] float tatieColorAnEmphasis = 0.75f;
    [SerializeField] Vector3 tatieLOrigineSize;
    [SerializeField] Vector3 tatieROrigineSize;
    [SerializeField] float tatieSizeEmphasis;

    [SerializeField] float textMoveTime=0.5f;

    public bool autoMode = false;
    [SerializeField] float autoWaitTime;
    ReactiveProperty<bool> autoNextPage = new ReactiveProperty<bool>();

    string[] cacheTextL = new string[2];
    string[] cacheTextR = new string[2];
    string[] cacheTextC = new string[2];

    int[] textLengthL = new int[2];
    int[] textLengthR = new int[2];
    int[] textLengthC = new int[2];

    public static float textSpeed = 5.0f;

    float canViewTextNum = 0;

    //�����G
    [Header("�����G�֘A")]
    Sprite tatieL;
    Sprite tatieR;
    [SerializeField]SpriteRenderer tatieLObj;
    [SerializeField]SpriteRenderer tatieRObj;

    [Header("��ʃG�t�F�N�g�p�I�u�W�F�N�g�o�^")]
    [SerializeField] GameObject goFight;
    [SerializeField] GameObject jessicaIntro;
    [SerializeField] GameObject hakuaiIntro;
    [Header("��ʃG�t�F�N�g�p�p�����[�^")]
    [SerializeField] float introFadeSpeed;
    [SerializeField] float introViewTime;
    [SerializeField] float goFightViewTime;

    private void Awake()
    {
        if (SingletonCheck(this))
        {
            //novelMode = debugStartNovelMode;

            //�I�[�g�y�[�W�߂����o�^
            autoNextPage.Subscribe(x =>
            {
                if(x==true)
                DOVirtual.DelayedCall(autoWaitTime, () => { NextPage(); autoNextPage.Value = false; });
            }
            ).AddTo(this);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        cs = CacheData_Novel.instance;
        textRectL = textL.gameObject.GetComponent<RectTransform>();
        textRectR = textR.gameObject.GetComponent<RectTransform>();
        textRectC = textC.gameObject.GetComponent<RectTransform>();
        switch (novelMode)
        {
            case NovelMode.Novel:
                //�v���C���[�̈ʒu��ݒ肵�ăm�x�����J�n����
                NovelStart();
                break;
            case NovelMode.Rest:
                Rest();
                break;
            case NovelMode.Shop:
                Shop();
                break;


        }
    }
    /// <summary>
    /// �v���C���[�̈ʒu��ݒ�ʒu�ɂ���
    /// </summary>
    void SetPlayerStartPos()
    {
        var pos = Love.PlayerLove.instance.transform.position;
        pos.x = modeState.playerStartPos;
        Love.PlayerLove.instance.transform.position = pos;
    }
    void Rest()
    {
        //HP��N%�񕜂�����
        //var pGameState = Player.instance.gameState;
        //var heal = pGameState.maxHP.Value * modeState.restHealPer / 100.0f;
        //Player.instance.gameState.hp.Value += (int)heal;
        //�h���b�v�I�u�W�F�N�g��z�u����

        //�s�����x��N���񕜂�����

        var objectNum= Random.Range(modeState.instanceDropObjectMin, modeState.instanceDropObjectMax+1);

        List<int> posNums = new List<int>();
        for(int i = 0; i<objectNum; i++)
        {
            //�m���v�Z���Đ�������
            var per = Random.Range(1, 101);
            var perPlus = 0;
            foreach(var dropObj in cs.dropObject)
            {

                perPlus += dropObj.percent;
                if (per < perPlus)
                {
                    //�ݒ肳��Ă���I�u�W�F�N�g�ƈʒu�ɐݒ肷�� 
                    //�����ꏊ�ɐ�������Ȃ��悤�ɂ���
                    var posNum = Random.Range(0, cs.dropObject.Count);
                    int loopBreak = 0;
                    if (posNums.Count > 0)
                    {
                        while (posNums.IndexOf(posNum) != -1)
                        {
                            posNum = Random.Range(0, cs.dropObject.Count);
                            loopBreak++;
                        }
                    }
                    posNums.Add(posNum);

                    Instantiate(modeState.dropObject[dropObj.number], modeState.dropObjectPos[posNum].position,Quaternion.identity);
                    break;
                }
            }
        }
        //�����ʒu�ɐݒ�
        SetPlayerStartPos();
    }
    void Shop()
    {
        //�V���b�v�p�I�u�W�F�N�g�𐶐�
        for (int i = 0; i < modeState.shopObject.Count; i++)
        {
            Instantiate(modeState.shopObject[i], modeState.shopObjectPos[i].position, Quaternion.identity);
        }
        //�����ʒu�ɐݒ�
        SetPlayerStartPos();
    }
    // Update is called once per frame
    void Update()
    {
        //�m�x�����ɓ��͂��s��ꂽ�ۂɃy�[�W������
        if (isNovel.Value && stopInputForNextPage == false && InputSystem.instance.WasPressThisFlame("Player", "Fire"))
        {
            NextPage();
        }
        //�e�L�X�g�̃t�F�[�h
        if (startText)
        {
            textAlpha += Time.deltaTime * fadeSpeed;
            if (textAlpha <= 1.0f)
            {   
                textL.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
                textR.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
                textC.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
            }

            CharacterFeeds();
        }
    }

    public void ChangeAutoMode()
    {
        autoMode= !autoMode;
    }
    /// <summary>
    /// ����������s��
    /// </summary>
    void CharacterFeeds()
    {
        //�e�L�X�g���菈��

        //�\������e�L�X�g�������Z
        canViewTextNum += (Time.deltaTime * textSpeed);

        CharacterFeed(textL, cacheTextL, textLengthL);
        CharacterFeed(textR, cacheTextR, textLengthR);
        CharacterFeed(textC, cacheTextC, textLengthC);

    }
    void CharacterFeed(TextMesh textMesh, string[] cacheText,int[] textLength)
    {
        //���������݂���Ƃ��̂ݏ������s��
        if (cacheText[0] != null)
        {
            //�\�����������`�悷�镶�����𒴂��Ă����ꍇ�����ɂ���
            var maxText = (int)canViewTextNum;
            if (maxText > textLength[0]) { maxText = textLength[0]; }

            //�������`��p�e�L�X�g�ɑ������
            var text = cacheText[0].Substring(0, maxText);
            textMesh.text = text;

            //�Q�s�ȏ�e�L�X�g������ꍇ�����������s��
            if (canViewTextNum > textLength[0] && cacheText[1] != null)
            {
                int maxText2 = (int)canViewTextNum - textLength[0];
                if (maxText2 > textLength[1]) { maxText2 = textLength[1]; }
                textMesh.text = text + cacheText[1].Substring(0, maxText2);

                //�e�L�X�g�I�[�g�������s��
                if (maxText2 >= textLength[1])
                {
                    AutoNextPage();
                }
            }
            //�e�L�X�g���P�s�̏ꍇ�ŃI�[�g���쓮���̏ꍇ�@�e�L�X�g�I�[�g�������s��
            else
            {
                if (maxText >= textLength[0])
                {
                    AutoNextPage();
                }
            }
        }
    }
    void AutoNextPage()
    {
        if (isNovel.Value && stopInputForNextPage == false&&autoMode)
        {
            autoNextPage.Value = true;
        }
    }
    public void NovelStart()
    {
        cs = CacheData_Novel.instance;
        isNovel.Value = true;
        
        Love.PlayerLove.instance.move = false;

        //TODO�@DB����v���C���[�ƃL�����̈ʒu��ݒ肷��
        //
        //
    }
    void NovelEnd()
    {
        Love.PlayerLove.instance.move = true;
    }
    void InsertText(int _page)
    {
        startText = true;
        textAlpha = 0;
        canViewTextNum = 0;

        textL.text = "";
        textR.text = "";
        textC.text = "";
        textL.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
        textR.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
        textC.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);

        textRectL.anchoredPosition = new Vector2(textRectL.anchoredPosition.x, textStartPos);
        textRectR.anchoredPosition = new Vector2(textRectR.anchoredPosition.x, textStartPos);
        textRectC.anchoredPosition = new Vector2(textRectC.anchoredPosition.x, textStartPos);

        textRectL.DOAnchorPosY(textEndPos, textMoveTime);
        textRectR.DOAnchorPosY(textEndPos, textMoveTime);
        textRectC.DOAnchorPosY(textEndPos, textMoveTime);
        cacheTextL[0] = null;
        cacheTextR[0] = null;
        cacheTextC[0] = null;
        cacheTextL[1] = null;
        cacheTextR[1] = null;
        cacheTextC[1] = null;

        var nowPageData = cs.chapter1[_page];
        var nextPageData = cs.chapter1[_page+1];
        string posString = nowPageData.position;
        //�O�̃|�W�V�������Q�Ƃ���
        var minusCount = 1;
        while (posString == "")
        {
            posString = cs.chapter1[_page - minusCount].position;
            minusCount++;
        }
        //��s�ڂ������鏈��
        if (posString == "L")
        {
            cacheTextL[0] = nowPageData.text;
            textLengthL[0]=cacheTextL[0].Length;
        }
        else if (posString == "R")
        {
            cacheTextR[0] = nowPageData.text;
            textLengthR[0] = cacheTextR[0].Length;
        }
        else if (posString == "C")
        {
            cacheTextC[0] = nowPageData.text;
            textLengthC[0] = cacheTextC[0].Length;
        }
        page++;
        //��s�ڂ����݂���Ȃ������鏈��
        if (nextPageData.newLine == 0)
        {
            if (posString == "L")
            {
                cacheTextL[1] += "\n"+ nextPageData.text;
                textLengthL[0] = cacheTextL[0].Length;
            }
            else if (posString == "R")
            {
                cacheTextR[1] += "\n" + nextPageData.text;
                textLengthR[1] = cacheTextR[1].Length;
            }
            else if (posString == "C")
            {
                cacheTextC[1] += "\n" + nextPageData.text;
                textLengthC[1] = cacheTextC[1].Length;

            }
            page++;
        }

        
    }


    void InsertImage(int _page)
    {
        var headStr = "Texture/Love/Tatie/";

        var textureNumL = cs.chapter1[_page].charaImageL;
        var textureNumR = cs.chapter1[_page].charaImageR;
        if (textureNumL != 0)
        {
            foreach(var taties in cs.tatie)
            {
                if (taties.number == textureNumL)
                {
                    string[] words = taties.textureName.Split('.');
                    tatieL = Resources.Load<Sprite>(headStr + words[0]);
                    break;
                }
            }
        }
        else
        {
            tatieL = null;
        }
        if (textureNumR != 0)
        {
            foreach (var taties in cs.tatie)
            {
                if (taties.number == textureNumR)
                {
                    string[] words = taties.textureName.Split('.');
                    tatieR = Resources.Load<Sprite>(headStr + words[0]);
                    break;
                }
            }
        }

        tatieLObj.sprite = tatieL;
        tatieRObj.sprite = tatieR;
    }
    enum NovelEffectNum
    {
        GoFight=3,
        Intro_Jessica=4,
        Intro_Hakuai=5

    }
    void InsertEffect(int _page)
    {
        // �G�t�F�N�g���� �킩��ɂ����̂Œ��ڏ������ނ��Ƃɂ���

        var effectNumL = cs.chapter1[_page].effectL;
        var effectNumR = cs.chapter1[_page].effectR;

        switch (effectNumL)
        {
            case (int)NovelEffectNum.GoFight:
                //�퓬�J�n�X�`���\������V�[���ړ��܂�
                goFight.SetActive(true);
                stopInputForNextPage = true;
                Sequence sequenceG = DOTween.Sequence().OnStart(() => { })
                .Append(goFight.GetComponent<CanvasGroup>().DOFade(1, introFadeSpeed))
                .Append(DOVirtual.DelayedCall(goFightViewTime, () => { }))
                .Append(DOVirtual.DelayedCall(0.01f, () => { /*�V�[���ړ�*/SceneManager.LoadScene("Fight"); }))
                ;
                break;
            case (int)NovelEffectNum.Intro_Jessica:
                //�C���g���\�������\���A�y�[�W�X�V�܂�
                jessicaIntro.SetActive(true);
                stopInputForNextPage = true;
                Sequence sequenceJ = DOTween.Sequence().OnStart(() => { })
                .Append(jessicaIntro.GetComponent<CanvasGroup>().DOFade(1, introFadeSpeed))
                .Append(DOVirtual.DelayedCall(introViewTime, () => { }))
                .Append(jessicaIntro.GetComponent<CanvasGroup>().DOFade(0, introFadeSpeed))
                .Append(DOVirtual.DelayedCall(0.01f, () => { jessicaIntro.SetActive(false); }))
                .Append(DOVirtual.DelayedCall(0.01f, () => { stopInputForNextPage = false; NextPage(); }))
                ;
                break;
            case (int)NovelEffectNum.Intro_Hakuai:
                //�C���g���\�������\���A�y�[�W�X�V�܂�
                hakuaiIntro.SetActive(true);
                stopInputForNextPage = true;
                Sequence sequenceH = DOTween.Sequence().OnStart(() => { })
                .Append(hakuaiIntro.GetComponent<CanvasGroup>().DOFade(1, introFadeSpeed))
                .Append(DOVirtual.DelayedCall(introViewTime, () => { }))
                .Append(hakuaiIntro.GetComponent<CanvasGroup>().DOFade(0, introFadeSpeed))
                .Append(DOVirtual.DelayedCall(0.01f, () => { hakuaiIntro.SetActive(false); }))
                .Append(DOVirtual.DelayedCall(0.01f, () => { stopInputForNextPage = false; NextPage(); }))
                ;
                break;
        }
        //if (effectNumL <= 0) { effectNumL = -1; }
        //if (effectNumR <= 0) { effectNumR = -1; }

        //CinematicsManager.Instance.PlayCinematicEffect(effectNumL, tatieLObj, effectNumR, tatieRObj);
    }

    /// <summary>
    /// �摜�̋������s���i���邭�����違�X�P�[���𒲐��j
    /// </summary>
    /// <param name="_page"></param>
    void InsertImageEmphasis(int _page)
    {

        var nowPageData = cs.chapter1[_page];
        string posString = nowPageData.position;
        //�ʒu��񂪂Ȃ��ꍇ�O�̃|�W�V�������Q�Ƃ���
        var minusCount = 1;
        while (posString == "")
        {
            posString = cs.chapter1[_page - minusCount].position;
            minusCount++;
            if (minusCount > 50) { posString = ""; break; }
        }

        Color LColor = Color.white;
        Color RColor = Color.white;
        Vector3 LSize = tatieLOrigineSize;
        Vector3 RSize = tatieROrigineSize;

        //�����̕\�L�ꏊ�ɂ��J���[�̐���
        switch (posString)
        {
            case "L":
                //�摜�������傫���\������A�l���P�ɐݒ�
                RColor = new Color(tatieColorAnEmphasis, tatieColorAnEmphasis, tatieColorAnEmphasis, 1);
                LColor = Color.white;

                RSize = tatieROrigineSize;
                LSize = tatieSizeEmphasis * tatieLOrigineSize;
                break;
            case "R":
                RColor = Color.white;
                LColor = new Color(tatieColorAnEmphasis, tatieColorAnEmphasis, tatieColorAnEmphasis, 1);

                RSize = tatieSizeEmphasis * tatieROrigineSize;
                LSize = tatieLOrigineSize;
                break;
        }
        //�摜���Z�b�g����Ă��Ȃ��ꍇA�l���O�k�ɂ��ăt�F�[�h������
        if (tatieLObj.sprite == null)
        {
            var col = Color.white;
            col.a = 0;
            tatieLObj.color = col;
        }
        if (tatieRObj.sprite == null)
        {
            var col = Color.white;
            col.a = 0;
            tatieRObj.color = col;
        }
        var textureNumL = cs.chapter1[_page].charaImageL;
        var textureNumR = cs.chapter1[_page].charaImageR;
        //���ۂɃJ���[��ψق�����@�^�񒆂ɕ������\�������ꍇ�͉ߋ��̃J���[�����̂܂܂ɂ���
        //�e�N�X�`�����ݒ肳��Ă��Ȃ���Ԃ���\������ꍇ�ɖ�肪�N����̂�IF�̐퓬�Ƀe�N�X�`���m�F���V�Ă���
        if (textureNumL != 0 || posString != "C")
        {
            tatieLObj.transform.DOScale(LSize, tatieFadeSpeed);
            tatieLObj.DOColor(LColor, tatieFadeSpeed);
        }
        if (textureNumR != 0 || posString != "C")
        {
            tatieRObj.transform.DOScale(RSize, tatieFadeSpeed);
            tatieRObj.DOColor(RColor, tatieFadeSpeed);
        }
    }
    void NextPage()
    {

            InsertImageEmphasis(page);
            InsertImage(page);
            

            InsertEffect(page);
            InsertText(page);

    }
    void InputEvent()
    {
       
    }
}
