using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
public class NovelManager : SingletonMonoBehaviour<NovelManager>
{

    CacheScenario cs;
    public ReactiveProperty<bool> isNovel = new ReactiveProperty<bool>();
    int page=0;
    float textTime;

    string leftText;
    string rightText;

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
    Sprite tatieL;
    Sprite tatieR;
    [SerializeField]SpriteRenderer tatieLObj;
    [SerializeField]SpriteRenderer tatieRObj;
    
    private void Awake()
    {
        SingletonCheck(this);
    }


    // Start is called before the first frame update
    void Start()
    {
        cs = CacheScenario.instance;
        textRectL = textL.gameObject.GetComponent<RectTransform>();
        textRectR = textR.gameObject.GetComponent<RectTransform>();
        textRectC = textC.gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        NextPage();
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

        //// エフェクトデバッグ用
        //if(Input.GetKeyDown(KeyCode.Alpha1)) // 左画像の揺れ
        //{
        //    CinematicsManager.Instance.PlayCinematicEffect(1, tatieLObj, -1, tatieRObj);
        //}
        //if(Input.GetKeyDown(KeyCode.Alpha2)) // 点滅
        //{
        //    CinematicsManager.Instance.PlayCinematicEffect(2, tatieLObj, -1, tatieRObj);
        //}
        //if(Input.GetKeyDown(KeyCode.Alpha3)) // 右画像の揺れ
        //{
        //    CinematicsManager.Instance.PlayCinematicEffect(-1, tatieLObj, 1, tatieRObj);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha4)) // 点滅（右を使って変更があるかどうか確認)
        //{
        //    CinematicsManager.Instance.PlayCinematicEffect(-1, tatieLObj, 2, tatieRObj);
        //}
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
        cs = CacheScenario.instance;
        isNovel.Value = true;
        //page = 0;
        textTime = 0;

        Love.PlayerLove.instance.move = false;
        //InsertText(page);
        //黒帯を出現させる
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
                }
            }
        }

        tatieLObj.sprite = tatieL;
        tatieRObj.sprite = tatieR;



    }

    void InsertEffect(int _page)
    {
        // エフェクト発生
        var effectNumL = cs.chapter1[_page].effectL;
        var effectNumR = cs.chapter1[_page].effectR;

        if (effectNumL <= 0) { effectNumL = -1; }
        if (effectNumR <= 0) { effectNumR = -1; }

        CinematicsManager.Instance.PlayCinematicEffect(effectNumL, tatieLObj, effectNumR, tatieRObj);
    }
    void NextPage()
    {
        if (isNovel.Value && InputSystem.instance.WasPressThisFlame("Player", "Fire"))
        {

            InsertImage(page);
            InsertEffect(page);
            InsertText(page);
            
        }
    }
    void InputEvent()
    {
       
    }
}
