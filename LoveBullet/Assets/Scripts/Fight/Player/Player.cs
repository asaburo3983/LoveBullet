using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UniRx;

[System.Serializable]
public struct ReductionRate
{
    [Header("0% ~ 100%"), Header("デバフ時の倍率")]
    [Range(0, 100)] public int AT;
    [Header("100% ~ 200%"), Range(100, 200)] public int DF;
}

public enum RecieveDamageReaction
{
    EnemyAttack,
    Guard,
    Poison,
    Self,
}

public class Player : SingletonMonoBehaviour<Player>
{
    [SerializeField] ReductionRate rate;
    public ReductionRate Rate => rate;

    [System.Serializable]
    public struct InGameState
    {
        public IntReactiveProperty maxHP;
        public IntReactiveProperty hp;
        public IntReactiveProperty Atk;
        public IntReactiveProperty Atk_Never;
        public IntReactiveProperty Def;
        public IntReactiveProperty Def_Never;
        public IntReactiveProperty ATWeaken;
        public IntReactiveProperty DFWeaken;

        public int reloadAP;
        public int cockingAP;

        public int freeCocking;
    }
    public InGameState gameState = new InGameState();


    [System.Serializable]
    public struct UI
    {
        public GameObject hp;
        public GameObject AP;
        public GameObject DF;
        public GameObject ATWeaken;
        public GameObject DFWeaken;
    }
    [SerializeField] UI ui;

    /// <summary>
    /// DF管理
    /// </summary>
    public static void ResetDF()
    {
        instance.gameState.Def.Value = 0;
    }
    public static void PlusDF(int _value)
    {
        instance.gameState.Def.Value += _value;
    }
    public static void MinusDF(int _value)
    {
        instance.gameState.Def.Value -= _value;
    }
    /// <summary>
    /// プレイヤーにダメージを与える処理
    /// </summary>
    /// <param name="_damage"></param>
    public static void ReceiveDamage(int _damage)
    {
        var state = instance.gameState;
        var dmg = _damage - state.Def.Value;
        if (dmg > 0) {
            state.hp.Value -= dmg;
        }
        state.Def.Value = Mathf.Clamp(state.Def.Value - _damage, 0, 9999);
    }

    // 被ダメアニメーション
    public void ReceiveAnim()
    {
        Sequence seq = DOTween.Sequence()
            .Append(transform.DOLocalMoveX(transform.localPosition.x - 50, 0.1f).SetLoops(2, LoopType.Yoyo))
            .Join(transform.GetChild(0).GetComponent<Image>().DOColor(new Color(1, 0, 0, 1), 0.1f).SetLoops(2, LoopType.Yoyo));
    }

    private void Start()
    {
        instance.gameState.hp.Pairwise().Subscribe(x => {
            // 0以上から0以下になった場合死亡処理
            if (x.Current <= 0 && x.Previous > 0) {
                Debug.Log("死亡処理　未作成");
            }
        }).AddTo(this);
    }


    private void Awake()
    {
        SingletonCheck(this);
        gameState.maxHP.Value = gameState.hp.Value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
