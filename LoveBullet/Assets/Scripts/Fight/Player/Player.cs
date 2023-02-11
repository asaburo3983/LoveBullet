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


    // TweenAnimation
    Tween fireTw = null;
    [SerializeField] float attackMovePosX;
    [SerializeField] float attackMoveSpeed;
    [SerializeField] float receiveMovePosX;
    [SerializeField] float receiveMoveSpeed;

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

    public void AttackAnim()
    {
        // プレイヤーアニメーション
        if (fireTw != null) fireTw.Kill(true);
        var pi = Player.instance;
        fireTw = Player.instance.transform.DOMoveX(attackMovePosX, attackMoveSpeed).SetLoops(2, LoopType.Yoyo).OnComplete(() => fireTw = null);
    }
    // 被ダメアニメーション
    Sequence seq;
    public void ReceiveAnim()
    {
        if (seq != null) { seq.Kill(true); }
        seq = DOTween.Sequence()
            .Append(transform.DOMoveX(receiveMovePosX, receiveMoveSpeed).SetLoops(2, LoopType.Yoyo))
            .Join(transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(new Color(1, 0, 0, 1), 0.1f).SetLoops(2, LoopType.Yoyo))
            .OnComplete(() => seq = null);
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
