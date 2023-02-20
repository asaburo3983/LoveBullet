using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

public class BandManager : SingletonMonoBehaviour<BandManager>
{
    [SerializeField] Canvas optionCanvas;
    [SerializeField] Canvas deckListCanvas;

    public ReactiveProperty<int> playerHP = new ReactiveProperty<int>();
    public ReactiveProperty<int> playerMaxHP = new ReactiveProperty<int>();

    public ReactiveProperty<int> unstable = new ReactiveProperty<int>();
    public ReactiveProperty<int> feelRed = new ReactiveProperty<int>();
    public ReactiveProperty<int> feelBlue = new ReactiveProperty<int>();
    public ReactiveProperty<int> feelGreen = new ReactiveProperty<int>();

    [SerializeField] Transform hpObj;
    [SerializeField] Transform unstabeObj;
    [SerializeField] Transform feelRedObj;
    [SerializeField] Transform feelBlueObj;
    [SerializeField] Transform feelGreenObj;
    [SerializeField] Vector3 beatBigSize;
    [SerializeField] float beatSpeed;

    //[SerializeField] static NovelManager.NovelMode novelMode;
    private void Awake()
    {
        SingletonCheck(this, true);

        //UI‚Ì‘Œ¸‚É‚æ‚éŒÛ“®ƒAƒjƒ
        playerHP.Zip(playerHP.Skip(1), (x, y) => new { OldValue = x, NewValue = y }).Subscribe(t => {
            HeartBeatAnim(t.OldValue, t.NewValue, hpObj);
        }).AddTo(this);
        unstable.Zip(unstable.Skip(1), (x, y) => new { OldValue = x, NewValue = y }).Subscribe(t => {
            HeartBeatAnim(t.OldValue, t.NewValue, unstabeObj);
        }).AddTo(this);
        feelRed.Zip(feelRed.Skip(1), (x, y) => new { OldValue = x, NewValue = y }).Subscribe(t => {
            HeartBeatAnim(t.OldValue, t.NewValue, feelRedObj);
        }).AddTo(this);
        feelBlue.Zip(feelBlue.Skip(1), (x, y) => new { OldValue = x, NewValue = y }).Subscribe(t => {
            HeartBeatAnim(t.OldValue, t.NewValue, feelBlueObj);
        }).AddTo(this);
        feelGreen.Zip(feelGreen.Skip(1), (x, y) => new { OldValue = x, NewValue = y }).Subscribe(t => {
            HeartBeatAnim(t.OldValue, t.NewValue, feelGreenObj);
        }).AddTo(this);
    }
private void Start()
    {
        
    }
    private void Update()
    {
        
    }
    Tween heartBeatTW;
    void HeartBeatAnim(int oldNum,int nowNum,Transform objT)
    {
        if (heartBeatTW != null) { heartBeatTW.Kill(true); }
        var beatNum = Mathf.Abs((oldNum - nowNum)*2);
        heartBeatTW = objT.DOScale(beatBigSize, beatSpeed).SetLoops(beatNum, LoopType.Yoyo).OnComplete(() => heartBeatTW = null);
    }

    public void HealHP(int heal)
    {
        playerHP.Value += heal;
        if (playerHP.Value > playerMaxHP.Value)
        {
            playerHP.Value = playerMaxHP.Value;
        }

    }
    public void HealMind(int heal)
    {
        unstable.Value -= heal;
        if (unstable.Value < 0)
        {
            unstable.Value = 0;
        }
    }

}
