using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
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

        [Header("休憩時に使用する値")]
        public float restHealPer;
        public float restMindHealPer;
        public int instanceDropObjectMin;
        public int instanceDropObjectMax;
        public List<GameObject> dropObject;
        public List<Transform> dropObjectPos;

        [Header("ショップ時に使用する値")]
        public List<GameObject> shopObject;
        public List<Transform> shopObjectPos;
    }
    [SerializeField] ModeState modeState;

    //テキスト系
    [Header("テキスト関連")]
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

    string[] cacheTextL = new string[2];
    string[] cacheTextR = new string[2];
    string[] cacheTextC = new string[2];

    int[] textLengthL = new int[2];
    int[] textLengthR = new int[2];
    int[] textLengthC = new int[2];

    public float textSpeed = 5.0f;
    float canViewTextNum = 0;

    //立ち絵
    [Header("立ち絵関連")]
    Sprite tatieL;
    Sprite tatieR;
    [SerializeField]SpriteRenderer tatieLObj;
    [SerializeField]SpriteRenderer tatieRObj;

    [Header("画面エフェクト用オブジェクト登録")]
    [SerializeField] GameObject goFight;
    [SerializeField] GameObject jessicaIntro;
    [SerializeField] GameObject hakuaiIntro;
    [Header("画面エフェクト用パラメータ")]
    [SerializeField] float introFadeSpeed;
    [SerializeField] float introViewTime;

    private void Awake()
    {
        if (SingletonCheck(this))
        {
            //novelMode = debugStartNovelMode;
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
                //プレイヤーの位置を設定してノベルを開始する
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
    /// プレイヤーの位置を設定位置にする
    /// </summary>
    void SetPlayerStartPos()
    {
        var pos = Love.PlayerLove.instance.transform.position;
        pos.x = modeState.playerStartPos;
        Love.PlayerLove.instance.transform.position = pos;
    }
    void Rest()
    {
        //HPをN%回復させる
        //var pGameState = Player.instance.gameState;
        //var heal = pGameState.maxHP.Value * modeState.restHealPer / 100.0f;
        //Player.instance.gameState.hp.Value += (int)heal;
        //ドロップオブジェクトを配置する

        //不安程度をN％回復させる

        var objectNum= Random.Range(modeState.instanceDropObjectMin, modeState.instanceDropObjectMax+1);

        List<int> posNums = new List<int>();
        for(int i = 0; i<objectNum; i++)
        {
            //確率計算して生成する
            var per = Random.Range(1, 101);
            var perPlus = 0;
            foreach(var dropObj in cs.dropObject)
            {

                perPlus += dropObj.percent;
                if (per < perPlus)
                {
                    //設定されているオブジェクトと位置に設定する 
                    //同じ場所に生成されないようにする
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
        //初期位置に設定
        SetPlayerStartPos();
    }
    void Shop()
    {
        //ショップ用オブジェクトを生成
        for (int i = 0; i < modeState.shopObject.Count; i++)
        {
            Instantiate(modeState.shopObject[i], modeState.shopObjectPos[i].position, Quaternion.identity);
        }
        //初期位置に設定
        SetPlayerStartPos();
    }
    // Update is called once per frame
    void Update()
    {
        //ノベル時に入力が行われた際にページを捲る
        if (isNovel.Value && stopInputForNextPage == false && InputSystem.instance.WasPressThisFlame("Player", "Fire"))
        {
            NextPage();
        }
        //テキストのフェード
        if (startText)
        {
            textAlpha += Time.deltaTime * fadeSpeed;
            if (textAlpha <= 1.0f)
            {   
                textL.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
                textR.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
                textC.color = new Color(textColor.r, textColor.g, textColor.b, textAlpha);
            }

            CharacterFeed();
        }
    }

    void CharacterFeed()
    {
        //テキスト送り処理
        canViewTextNum += (Time.deltaTime * textSpeed);

        if (cacheTextL[0] != null)
        {
            //canViewTextNumは文字列の個数以下にしないといけない
            var maxText = (int)canViewTextNum;
            if (maxText > textLengthL[0]) { maxText = textLengthL[0]; }

            var text = cacheTextL[0].Substring(0, maxText);
            textL.text = text;

            if (canViewTextNum > textLengthL[0] && cacheTextL[1] != null)
            {
                int maxText2 = (int)canViewTextNum - textLengthL[0];
                if (maxText2 > textLengthL[1]) { maxText2 = textLengthL[1]; }
                textL.text = text + cacheTextL[1].Substring(0, maxText2);
            }
        }
        if (cacheTextR[0] != null)
        {
            //canViewTextNumは文字列の個数以下にしないといけない
            var maxText = (int)canViewTextNum;
            if (maxText > textLengthR[0]) { maxText = textLengthR[0]; }

            var text = cacheTextR[0].Substring(0, maxText);
            textR.text = text;

            if (canViewTextNum > textLengthR[0] && cacheTextR[1] != null)
            {
                int maxText2 = (int)canViewTextNum - textLengthR[0];
                if (maxText2 > textLengthR[1]) { maxText2 = textLengthR[1]; }
                textR.text = text + cacheTextR[1].Substring(0, maxText2);
            }
        }
        if (cacheTextC[0] != null)
        {
            //canViewTextNumは文字列の個数以下にしないといけない
            var maxText = (int)canViewTextNum;
            if (maxText > textLengthC[0]) { maxText = textLengthC[0]; }

            var text = cacheTextC[0].Substring(0, maxText);
            textC.text = text;

            if (canViewTextNum > textLengthC[0] && cacheTextC[1] != null)
            {
                int maxText2 = (int)canViewTextNum - textLengthC[0];
                if (maxText2 > textLengthC[1]) { maxText2 = textLengthC[1]; }
                textC.text = text + cacheTextC[1].Substring(0, maxText2);
            }
        }
    }
    public void NovelStart()
    {
        cs = CacheData_Novel.instance;
        isNovel.Value = true;
        
        Love.PlayerLove.instance.move = false;

        //TODO　DBからプレイヤーとキャラの位置を設定する
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
        //前のポジションを参照する
        var minusCount = 1;
        while (posString == "")
        {
            posString = cs.chapter1[_page - minusCount].position;
            minusCount++;
        }
        //一行目を代入する処理
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
        //二行目が存在するなら代入する処理
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
        // エフェクト発生 わかりにくいので直接書き込むことにする

        var effectNumL = cs.chapter1[_page].effectL;
        var effectNumR = cs.chapter1[_page].effectR;

        switch (effectNumL)
        {
            case (int)NovelEffectNum.GoFight:
                break;
            case (int)NovelEffectNum.Intro_Jessica:
                //イントロ表示から非表示、ページ更新まで
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
                //イントロ表示から非表示、ページ更新まで
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
    /// 画像の強調を行う（明るく見せる＆スケールを調整）
    /// </summary>
    /// <param name="_page"></param>
    void InsertImageEmphasis(int _page)
    {

        var nowPageData = cs.chapter1[_page];
        string posString = nowPageData.position;
        //位置情報がない場合前のポジションを参照する
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

        //文字の表記場所によるカラーの制御
        switch (posString)
        {
            case "L":
                //画像を少し大きく表示してA値を１に設定
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
        //画像がセットされていない場合A値を０殻にしてフェードさせる
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
        //実際にカラーを変異させる　真ん中に文字が表示される場合は過去のカラーをそのままにする
        //テクスチャが設定されていない状態から表示する場合に問題が起きるのでIFの戦闘にテクスチャ確認をシている
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
