using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using DG.Tweening;

public class Fight : SingletonMonoBehaviour<Fight>
{
    [SerializeField] List<Transform> enemyAddventPos;

    [Header("カード管理系")]
    [SerializeField] List<Card.Card> deckInCards = new List<Card.Card>();

    ReactiveCollection<Card.Card> gunInCards = new ReactiveCollection<Card.Card>();
    public List<Card.Card> GunInCards { get { return gunInCards.ToList(); } }
    public ReactiveCollection<Card.Card> gunInCardsReactive => gunInCards;

    // デバッグ用の銃内のカード表示処理
#if UNITY_EDITOR
    [SerializeField] List<Card.Card> GunInCard;
    private void FixedUpdate()
    {
        GunInCard = gunInCards.ToList();
    }
#endif
    [SerializeField] List<Card.Card> trashInCards = new List<Card.Card>();

    Player player;
    Player.InGameState plState;

    [Header("その他ルール系")]
    [SerializeField, ReadOnly] int floor;
    [SerializeField] int reloadCost = 1;
    [SerializeField] int cockingCost = 1;
    [SerializeField, ReadOnly] bool playerTurn = true;
    public bool PlayerTurn => playerTurn;
    bool cocking = false;
    public bool CockingFlg { get { return cocking; } set { cocking = value; } }

    [Header("敵処理系")]
    public GameObject enemyBase;
    List<Enemy.Enemy.State> enemysState = new List<Enemy.Enemy.State>();
    public List<Enemy.Enemy> enemyObjects = new List<Enemy.Enemy>();
    [SerializeField, ReadOnly] int targetId = 0;
    public int TargetId => targetId;

    public ReactiveCollection<Enemy.Enemy> actEnemy = new ReactiveCollection<Enemy.Enemy>();

    #region InitFunction

    private void Awake()
    {
        if (SingletonCheck(this, true))
        {
        }
    }
    void Start()
    {
        player = Player.instance;
        plState = player.gameState;
        playerTurn = true;

        // 行動する敵を順番にアニメーションし、全てが終わると行動できるようにする
        actEnemy.ObserveRemove().Subscribe(x =>
        {
            if (actEnemy.Count == 0)
            {
                playerTurn = true;
                plState.Def.Value = 0;
                return;
            }
            //敵の行動処理
            actEnemy[0].AttackAnimation();
        }).AddTo(this);

    }
    private void Update()
    {


        if (enemyObjects.Count == 0 && ResultManager.instance.IsResult == false)
        {
            //TODO仮置き
            ResultManager.instance.StartResult();
        }
    }
    //戦闘を開始する
    public void StartFight()
    {
        StartFight_ShuffleCard();

        AdventEnemys();
    }
    void StartTurn_Player()
    {
        Player.ResetDF();
    }

    void StartTurn_Enemy()
    {
        //エネミー側で削除されていた場合リストから削除しておく
        for (int i = 0; i < enemyObjects.Count; i++)
        {
            if (enemyObjects[i] == null)
            {
                enemyObjects.RemoveAt(i);
            }
        }
        foreach (var enemy in enemyObjects)
        {
            enemy.Action();//敵に順番に行動させる
            enemy.ResetDF();//DFをリセットする
        }
    }

    #endregion

    #region Action

    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <param name="_enemy"></param>
    public void Fire(Enemy.Enemy _enemy)
    {
        // ターン処理が実行可能かチェック
        if (!playerTurn) return;

        // コッキング中は再度処理しない
        if (cocking) return;

        var _card = gunInCards[0].state;
        int _damage = _card.Damage+ gunInCards[0].powerUp.AT + plState.Atk.Value + plState.Atk_Never.Value;

        // 攻撃デバフが存在する場合、値の補正を行う
        if (plState.ATWeaken.Value > 0)
        {
            _damage *= player.Rate.AT;
            _damage /= 100;
        }

        // SE再生
        AudioSystem.AudioControl.Instance.SE.CardSePlayOneShot(gunInCards[0].SE);

        Debug.LogError("ランダム攻撃処理　未作成");
        Debug.LogError("与ダメージ吸収処理　　未作成");

        // 攻撃処理

        // 攻撃回数分ループする
        var attackNum = _card.MultiAttack;
        if (_card.Damage > 0 && attackNum == 0)
        {
            attackNum++;
        }
        for (int i = 0; i < attackNum; i++)
        {
            if (_card.Whole)
            {

                // 全体攻撃
                foreach (var _eObj in enemyObjects)
                {
                    _eObj.ReceiveDamage(_damage);
                    _eObj.ReceiveStan(_card.buff[(int)BuffEnum.Bf_Stan]);
                    _eObj.ReceiveATWeaken(_card.buff[(int)BuffEnum.Bf_Weaken_Attack]);
                    _eObj.ReceiveDFWeaken(_card.buff[(int)BuffEnum.Bf_Weaken_Diffence]);
                }
            }
            else
            {
                // 単体攻撃
                _enemy.ReceiveDamage(_damage);
                _enemy.ReceiveStan(_card.buff[(int)BuffEnum.Bf_Stan]);
                _enemy.ReceiveATWeaken(_card.buff[(int)BuffEnum.Bf_Weaken_Attack]);
                _enemy.ReceiveDFWeaken(_card.buff[(int)BuffEnum.Bf_Weaken_Diffence]);
            }
        }

        // プレイヤーステータス加減
        int _progress = _card.AP;
        plState.Def.Value = Mathf.Clamp(_card.buff[(int)BuffEnum.Bf_Diffence] > 0 ? _card.buff[(int)BuffEnum.Bf_Diffence] + plState.Def_Never.Value : 0, 0, 999);
        plState.Atk.Value = _card.buff[(int)BuffEnum.Bf_Attack];
        plState.Atk_Never.Value += _card.buff[(int)BuffEnum.Bf_Attack_Never];
        plState.Def_Never.Value += _card.buff[(int)BuffEnum.Bf_Diffence_Never];


        plState.hp.Value += _card.buff[(int)BuffEnum.Bf_Heal];

        plState.ATWeaken.Value = Mathf.Clamp(plState.ATWeaken.Value - 1, 0, 9999);
        plState.DFWeaken.Value = Mathf.Clamp(plState.DFWeaken.Value - 1, 0, 9999);

        // 強制リロードするか
        if (_card.Reload > 0)
        {
            for (int i = _card.Reload; i > 0; i--)
            {
                Reload();
            }
        }
        else
        {
            //捨て札にカードを入れてリボルバーから抜く
            if (_card.id != 0) trashInCards.Add(gunInCards[0]);//からの弾丸の場合捨て札に置かない

            gunInCards[0].state = Card.Search.GetCard(0);
            gunInCards.Move(0, 5);
            cocking = true;
        }

        // エネミーのターン経過処理
        ProgressTurn(_progress);

        //プレイヤーの攻撃アニメーション
        Player.instance.AttackAnim();
    }

    /// <summary>
    /// ターンを進行する
    /// </summary>
    /// <param name="_progressTurn">経過するターン</param>
    public void ProgressTurn(int _progressTurn)
    {
        bool _flg = true;
        foreach (var _enemy in enemyObjects)
        {
            if (_enemy.ProgressTurn(_progressTurn))
            {
                _flg = playerTurn = false;
                actEnemy.Add(_enemy);
            }
        }

        // 敵を行動させる
        if (!_flg) actEnemy[0].AttackAnimation();
    }

    /// <summary>
    /// プレイヤーの被ダメージ処理
    /// </summary>
    /// <param name="_damage"></param>
    public void ReceiveDamage(int _damage)
    {
        int dmg = _damage;
        // 防御デバフが存在する場合、値の補正を行う
        if (plState.DFWeaken.Value > 0)
        {
            dmg *= player.Rate.DF;
            dmg /= 100;
        }

        // 防御バフがある場合、ダメージ減算
        if (dmg <= plState.Def.Value)
        {
            plState.Def.Value -= dmg;
        }
        else
        {
            plState.hp.Value -= dmg - plState.Def.Value;
        }
    }

    /// <summary>
    /// プレイヤーの被デバフ処理
    /// </summary>
    /// <param name="_atk"></param>
    /// <param name="_def"></param>
    public void ReceiveWeaken(int _atk, int _def)
    {
        plState.ATWeaken.Value += _atk;
        plState.DFWeaken.Value += _def;
    }


    // ターゲット再指定
    public void SetTarget(int _id)
    {
        targetId = _id;
    }

    public void SetTarget(Enemy.Enemy _enemy)
    {
        targetId = enemyObjects.IndexOf(_enemy);
    }


    /// <summary>
    /// 現在ターゲットIDへの攻撃処理
    /// </summary>
    public void Fire()
    {
        Fire(enemyObjects[targetId]);
    }

    #endregion

    #region Card

    /// <summary>
    /// デッキリストからカードをシャッフルして山札を構築する
    /// </summary>
    void StartFight_ShuffleCard()
    {
        deckInCards.Clear();
        gunInCards.Clear();
        trashInCards.Clear();

        //デッキリストをシャッフルしてデッキに入れる
        deckInCards = DeckListManager.instance.deckList.OrderBy(a => Guid.NewGuid()).ToList();

        Reload();
    }


    /// <summary>
    /// リボルバーをリロードする
    /// </summary>
    /// <param name="bulletNum"></param>
    public void Reload(int bulletNum = 6)
    {
        //リボルバー内を空に 中身があるならTrushに移動させる
        foreach (var card in gunInCards)
        {
            if (card.state.id != 0) trashInCards.Add(card);
        }
        gunInCards.Clear();

        // とりあえず音
        AudioSystem.AudioControl.Instance.SE.PlaySeOneShot(SEList.Reload);


        //デッキ内カードが込める弾の個数より少ない時
        if (deckInCards.Count < bulletNum)
        {
            //山札がないので捨て札をシャッフルして山札に加える
            ResetTrush();
        }
        //デッキからカード追加
        for (int i = 0; i < bulletNum; i++)
        {
            gunInCards.Add(deckInCards[0]);
            deckInCards.RemoveAt(0);
        }

        ProgressTurn(reloadCost);
    }

    /// <summary>
    /// コッキングしてリボルバー内の弾丸を１つずらす
    /// </summary>
    public void Cocking()
    {
        // ターン処理が実行可能かチェック
        if (!playerTurn) return;

        // コッキング中は再度処理しない
        if (cocking) return;

        AudioSystem.AudioControl.Instance.SE.PlaySeOneShot(SEList.Cocking);

        gunInCards.Move(0, 5);
        ProgressTurn(cockingCost);
        cocking = true;
    }
    /// <summary>
    /// 捨て札にあるスキルを再度山に戻す
    /// </summary>
    public void ResetTrush()
    {
        // トラッシュリストをシャッフルしてデッキに入れる
        trashInCards = trashInCards.OrderBy(a => Guid.NewGuid()).ToList();

        foreach (var _cards in trashInCards)
        {
            deckInCards.Add(_cards);
        }

        //トラッシュのカードをなくす
        trashInCards.Clear();
    }

    /// <summary>
    /// 弾丸の能力を使用する
    /// </summary>
    /// <param name="target"></param>
    /// <param name="allTarget"></param>
    void CardSkill(int target = 0, bool allTarget = false)
    {
        if (enemyObjects.Count <= 0) { Debug.LogError("敵が存在しない状態でスキルを発動しています"); return; }
        //敵にダメージなり与える処理
        enemyObjects[target].ReceiveDamage(gunInCards[0].state.Damage);//敵へ攻撃処理
        plState.Def.Value += gunInCards[0].state.buff[(int)BuffEnum.Bf_Diffence];//プレイヤーへ防御追加
    }
    #endregion

    #region Enemy
    /// <summary>
    /// 階層に応じた敵を出現させる
    /// </summary>
    void AdventEnemys()
    {
        //敵をリセットする
        enemysState.Clear();

        //敵の出現条件について
        //とりあえず指定パターンで出限させる（後にDB化して指定パターンの中でもランダムで出現させるなどを行う）
        var rand = UnityEngine.Random.Range(0, Enemy.AddventPattern.GetGroupMax(floor));
        var enemyGroup = Enemy.AddventPattern.GetGroup(floor, rand);

        int enemyCount = 0;
        foreach (var enemyId in enemyGroup)
        {

            if (enemyId == -1) continue;
            enemysState.Add(Enemy.Enemy.GetEnemyState(enemyId));
            //敵生成
            var enemy = Instantiate(enemyBase, enemyAddventPos[enemyCount].position, Quaternion.identity);
            var script = enemy.GetComponent<Enemy.Enemy>();
            script.Initialize(enemysState[enemyCount]);

            enemyObjects.Add(script);

            enemyCount++;
        }

    }

    #endregion
}