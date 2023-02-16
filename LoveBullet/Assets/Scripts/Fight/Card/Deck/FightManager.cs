using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using DG.Tweening;

public class FightManager : SingletonMonoBehaviour<FightManager>
{

    //マネージャー系
    BandManager bandMana;


    [Header("カード管理系")]
    [SerializeField] List<Card.Card> deckInCards = new List<Card.Card>();
    public List<Card.Card> gunInCards = new List<Card.Card>();
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
    public GameObject enemyPrefab;
    [SerializeField] List<Transform> enemyStartPos;//エネミー初期位置
    [SerializeField] List<Transform> enemyAddventPos;//エネミー定位置

    List<Enemy.Enemy.State> enemysState = new List<Enemy.Enemy.State>();
    public List<Enemy.Enemy> enemyObjects = new List<Enemy.Enemy>();
    public int targetId = 0;

    public ReactiveCollection<Enemy.Enemy> actEnemy = new ReactiveCollection<Enemy.Enemy>();



    [Header("カード生成系")]
    [SerializeField] Transform fightCanvas;
    public GameObject cardPrefab;
    List<GameObject> cards=new List<GameObject>();

    [SerializeField] List<Transform> cardBasePos;//カード定位置
    [SerializeField] Transform cardReloadPos;//リロードじ移動位置
    [SerializeField] float reloadMoveSpeed;
    [SerializeField] float cardInstatiateDelay;

    [SerializeField] float fireMaxTime;
    [SerializeField] float reloadMaxTime;
    [SerializeField] float cockingMaxTime;
    bool IsCardMoveNow;

    #region InitFunction

    private void Awake()
    {
        if (SingletonCheck(this, true))
        {
        }
    }
    void Start()
    {
        bandMana = BandManager.instance;
        player = Player.instance;
        plState = player.gameState;
        //playerTurn = true;

        //// 行動する敵を順番にアニメーションし、全てが終わると行動できるようにする
        //actEnemy.ObserveRemove().Subscribe(x =>
        //{
        //    if (actEnemy.Count == 0)
        //    {
        //        playerTurn = true;
        //        plState.Def.Value = 0;
        //        return;
        //    }
        //    //敵の行動処理
        //    actEnemy[0].AttackAnimation();
        //}).AddTo(this);

        //戦闘開始演出


        //エネミー出現　演出込
        AdventEnemys();

        //デッキをシャッフルする
        ShuffleDeck();

        InstantiateGunInCards();
    }
    private void Update()
    {
    }

    /// <summary>
    /// カード初期生成
    /// </summary>
    void InstantiateGunInCards()
    {

        for (int i = 0; i < 6; i++)
        {
            InstantiateGunInCard(i);
            
        }
    }
    void InstantiateGunInCard(int num, bool emptyCard = false, bool delay = true)
    {
        //生成
        var obj=Instantiate(cardPrefab, cardReloadPos.position, Quaternion.identity,fightCanvas);
        Card.Card.State newCardState;
        //空の弾丸を入れる場合は0番を参照する
        if (emptyCard)
        {
            newCardState = Card.Search.GetCard(0);
            Card.Card EmptyCard = new Card.Card();
            EmptyCard.SetState(newCardState);
            gunInCards.Add(EmptyCard);
        }
        else
        {
            newCardState = deckInCards[0].state;
            gunInCards.Add(deckInCards[0]);//デッキのカードを手札に
            deckInCards.RemoveAt(0);//デッキのカードを削除
        }
       
        obj.transform.GetChild(0).GetComponent<Card.Card>().Initialize(newCardState);

        var delayTime = num * cardInstatiateDelay;
        if (delay == false) delayTime = 0.001f;

        DOVirtual.DelayedCall(delayTime, () => {
            obj.transform.DOMoveY(cardBasePos[num].position.y, reloadMoveSpeed).OnComplete(() => { obj.transform.DOMoveX(cardBasePos[num].position.x, reloadMoveSpeed); });
        });
        //定位置まで移動させる
        cards.Add(obj);
    }
    void InstantiateGunInCard(int num,Card.Card card)
    {
        //データをいれかえる
        gunInCards.Add(card);//デッキのカードを手札に

        //生成
        var obj = Instantiate(cardPrefab, cardReloadPos.position, Quaternion.identity, fightCanvas);
        Card.Card.State newCardState;

        newCardState = card.state;
        obj.transform.GetChild(0).GetComponent<Card.Card>().Initialize(newCardState);

         obj.transform.DOMoveY(cardBasePos[num].position.y, reloadMoveSpeed).OnComplete(() => { obj.transform.DOMoveX(cardBasePos[num].position.x, reloadMoveSpeed); });
        //定位置まで移動させる
        cards.Add(obj);
    }
    void StartTurn_Player()
    {
        Player.instance.ResetDF();
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

    void CardAction(Enemy.Enemy _enemy)
    {
        //カードの行動をする
        var selectCard = gunInCards[0];
        var cardState = selectCard.state;
        int _damage = cardState.Damage + selectCard.powerUp.AT + plState.Atk.Value + plState.Atk_Never.Value;
        // 攻撃デバフが存在する場合、値の補正を行う
        if (plState.ATWeaken.Value > 0)
        {
            _damage *= player.Rate.AT;
            _damage /= 100;
        }
        //// 攻撃処理

        // 攻撃回数分ループする
        var attackNum = cardState.MultiAttack;
        if (cardState.Damage > 0 && attackNum == 0)
        {
            attackNum++;
        }
        for (int i = 0; i < attackNum; i++)
        {
            //全体攻撃
            if (cardState.Whole)
            {
                foreach (var _eObj in enemyObjects)
                {
                    _eObj.ReceiveDamage(_damage);
                    _eObj.ReceiveStan(cardState.buff[(int)BuffEnum.Bf_Stan]);
                    _eObj.ReceiveATWeaken(cardState.buff[(int)BuffEnum.Bf_Weaken_Attack]);
                    _eObj.ReceiveDFWeaken(cardState.buff[(int)BuffEnum.Bf_Weaken_Diffence]);
                }
            }
            // 単体攻撃
            else
            {

                _enemy.ReceiveDamage(_damage);
                _enemy.ReceiveStan(cardState.buff[(int)BuffEnum.Bf_Stan]);
                _enemy.ReceiveATWeaken(cardState.buff[(int)BuffEnum.Bf_Weaken_Attack]);
                _enemy.ReceiveDFWeaken(cardState.buff[(int)BuffEnum.Bf_Weaken_Diffence]);
            }
        }

        //バフなど
       int _progress = cardState.AP;//コストを入れておく
        plState.Def.Value = Mathf.Clamp(cardState.buff[(int)BuffEnum.Bf_Diffence] > 0 ? cardState.buff[(int)BuffEnum.Bf_Diffence] + plState.Def_Never.Value : 0, 0, 999);
        plState.Atk.Value = cardState.buff[(int)BuffEnum.Bf_Attack];//１ターン攻撃強化
        plState.Atk_Never.Value += cardState.buff[(int)BuffEnum.Bf_Attack_Never];//永続攻撃強化
        plState.Def_Never.Value += cardState.buff[(int)BuffEnum.Bf_Diffence_Never];//永続防御強化

        bandMana.playerHP.Value += cardState.buff[(int)BuffEnum.Bf_Heal];//回復処理

        // 強制リロード
        if (cardState.Reload > 0)
        {
            for (int i = cardState.Reload; i > 0; i--)
            {
                Reload();
            }
        }
        //捨て札にカードを入れて手札から削除
        else
        {
            IsCardMoveNow = true;
            DOVirtual.DelayedCall(cockingMaxTime, () => { IsCardMoveNow = false; });
            //先頭のカードを削除して空のカードを生成する
            cards[0].transform.DOMoveY(cardReloadPos.position.y, reloadMoveSpeed).OnComplete(() =>
            {
                //カード本体の削除
                Destroy(cards[0]);
                cards.RemoveAt(0);
                //カードデータの削除
                //カード生成
                DOVirtual.DelayedCall(1, () =>
                {
                    InstantiateGunInCard(5, true,false);
                });
                gunInCards.RemoveAt(0);
                for (int i = 0; i < 5; i++)
                {
                    cards[i].transform.DOMoveX(cardBasePos[i].position.x, reloadMoveSpeed);
                }
            });
        }
    }
    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <param name="_enemy"></param>
    void Fire(Enemy.Enemy _enemy)
    {
        AudioSystem.AudioControl.Instance.SE.CardSePlayOneShot(gunInCards[0].state.SE);

        //カードを廃棄して空のカードを生成する
        CardAction(_enemy);

        //デバフ減少
        plState.ATWeaken.Value = Mathf.Clamp(plState.ATWeaken.Value - 1, 0, 9999);
        plState.DFWeaken.Value = Mathf.Clamp(plState.DFWeaken.Value - 1, 0, 9999);

        ProgressTurn(0);

        return;
        //// エネミーのターン経過処理
        //

        ////プレイヤーの攻撃アニメーション
        //Player.instance.AttackAnim();
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
            bandMana.playerHP.Value -= dmg - plState.Def.Value;
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



    #endregion

    #region Card

    /// <summary>
    /// デッキリストからカードをシャッフルして山札を構築する
    /// </summary>
    void ShuffleDeck()
    {
        //デッキリストをシャッフルしてデッキに入れる
        deckInCards = DeckListManager.instance.deckList.OrderBy(a => Guid.NewGuid()).ToList();
    }

    /// <summary>
    /// 現在ターゲットIDへの攻撃処理
    /// </summary>
    public void Fire()
    {
        Fire(enemyObjects[targetId]);
    }
    /// <summary>
    /// リボルバーをリロードする
    /// </summary>
    /// <param name="bulletNum"></param>
    public void Reload(int bulletNum = 6)
    {
        //リロード中はリロードできないようにする
        if (IsCardMoveNow) { return; }
        IsCardMoveNow = true;
        DOVirtual.DelayedCall(reloadMaxTime, () => { IsCardMoveNow = false; });
        //山札の数が６枚以下の場合捨て札を山に加える
        if (deckInCards.Count < 6)
        {
            foreach(var card in trashInCards)
            {
                deckInCards.Add(card);
            }
            trashInCards.Clear();

            ShuffleDeck();
        }
        //カードを移動後すべて削除
        for (int i = 0; i < 6; i++)
        {
            var num = i;
            cards[num].transform.DOMoveY(cardReloadPos.position.y, reloadMoveSpeed).OnComplete(() =>
            {
                //カード本体の削除
                Destroy(cards[0]);
                cards.RemoveAt(0);
                //捨て札にカードのデータを入れる
                trashInCards.Add(gunInCards[0]);
                //カードデータの削除
                gunInCards.RemoveAt(0);
            });
        }
        //カード生成
         DOVirtual.DelayedCall(1, () => { 
            
            InstantiateGunInCards(); 
        
        });


        AudioSystem.AudioControl.Instance.SE.PlaySeOneShot(SEList.Reload);

        //    ProgressTurn(reloadCost);
    }

    /// <summary>
    /// コッキングしてリボルバー内の弾丸を１つずらす
    /// </summary>
    public void Cocking()
    {
        if (IsCardMoveNow) { return; }
        IsCardMoveNow = true;
        DOVirtual.DelayedCall(cockingMaxTime, () => { IsCardMoveNow = false; });

        //戦闘のカードを削除して再生成する
        cards[0].transform.DOMoveY(cardReloadPos.position.y, reloadMoveSpeed).OnComplete(() =>
        {
            //カード本体の削除
            Destroy(cards[0]);
            cards.RemoveAt(0);
            //カードデータの削除
            //カード生成
            DOVirtual.DelayedCall(1, () =>
            {
                InstantiateGunInCard(5, gunInCards[0]);
            });
            gunInCards.RemoveAt(0);
            for (int i = 0; i < 5; i++)
            {
                cards[i].transform.DOMoveX(cardBasePos[i].position.x, reloadMoveSpeed);
            }
        });

        AudioSystem.AudioControl.Instance.SE.PlaySeOneShot(SEList.Cocking);

        //return;
        //// ターン処理が実行可能かチェック
        //if (!playerTurn) return;

        //// コッキング中は再度処理しない
        //if (cocking) return;

        

        ////gunInCards.Move(0, 5);
        //ProgressTurn(cockingCost);
        //cocking = true;
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
            var enemy = Instantiate(enemyPrefab, enemyAddventPos[enemyCount].position, Quaternion.identity);
            var script = enemy.GetComponent<Enemy.Enemy>();
            script.Initialize(enemysState[enemyCount]);
            script.fieldID = enemyCount;//ターゲット指定用のIDを代入

            enemyObjects.Add(script);//エネミーオブジェクトを保存

            enemyCount++;
        }

    }

    #endregion
}