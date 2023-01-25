using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

public class NovelManager : SingletonMonoBehaviour<NovelManager>
{

    public ReactiveProperty<bool> isNovel = new ReactiveProperty<bool>();
    int page;
    float textTime;

    string leftText;
    string rightText;


    private void Awake()
    {
        SingletonCheck(this);
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void NovelStart()
    {
        isNovel.Value = true;
        page = 0;
        textTime = 0;
        InsertText(page);
        //çïë—ÇèoåªÇ≥ÇπÇÈ
    }
    void NovelEnd()
    {

    }
    void InsertText(int _page)
    {

    }
    void NextPage()
    {
        if (isNovel.Value && InputSystem.instance.WasPressThisFlame("Player_Love", "Fire"))
        {
            //DBÇ©ÇÁï∂èÕÇéùÇ¡ÇƒÇ≠ÇÈ
            InsertText(page);
        }
    }
}
