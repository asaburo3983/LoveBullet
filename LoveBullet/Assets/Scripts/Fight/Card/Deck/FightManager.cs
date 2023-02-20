using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
  
    //プレイヤー系
    Player player;
    Player.InGameState plState;

    [Header("その他ルール系")]
    public static int floor;
    [SerializeField] int reloadCost = 1;
    [SerializeField] int cockingCost = 1;
    [SerializeField, ReadOnly] bool playerTurn = true;
    public bool PlayerTurn => playerTurn;

    [Header("敵処理系")]
    public GameObject enemyPrefab;
    [SerializeField] List<Transform> enemyStartPos;//エネミー初期位置
    [SerializeField] Transform enemyAddventPos;//エネミー定位置
    [SerializeField] float enemyAddventMoveTime;
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
    [SerializeField, Tooltip("カードの移動後にカードを生成するまでの時間")] float cardInstatiateDelay;

    [SerializeField] float fireMaxTime;
    [SerializeField] float reloadMaxTime;
    [SerializeField] float cockingMaxTime;
    bool IsCardMoveNow;

    [Header("スクリーン描画系")]
    ReactiveProperty<bool> isEndFight = new ReactiveProperty<bool>();
    [SerializeField] GameObject startFight;
    [SerializeField] GameObject endFight;
    [SerializeField] float screenFadeTime;

    [Header("リザルト表示系")]
    [SerializeField] float resultDrawTime;

    [Header("エフェクト")]
    [SerializeField] List<GameObject> cardEffect;
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
        playerTurn = true;

        //// 行動する敵を順番にアニメーションし、全てが終わると行動できるようにする
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

        //戦闘開始演出
        FloorStart();

        //エネミー出現　演出込
        AdventEnemys();

        //デッキをシャッフルする
        ShuffleDeck();
        //カードの初期生成
        InstantiateGunInCards();

        //戦闘終了処理
        isEndFight.Where(x => x == true).Subscribe(x => { FloorClear(); }).AddTo(this);
    }
    private void Update()
    {
        if (enemyObjects.Count <= 0)
        {
            //エネミーがすべていなくなった際に階層クリアを行う
            isEndFight.Value = true;
        }
       
        }
    void FloorStart()
    {
        //ゲーム開始演出を行う
        startFight.SetActive(true);
        var canvasGroup = startFight.GetComponent<CanvasGroup>();
        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 0.0f, screenFadeTime).OnComplete(() =>
        {
            startFight.SetActive(false);
        });
    }
    void FloorClear()
    {
        //クリア演出を行う
        endFight.SetActive(true);
        var canvasGroup = endFight.GetComponent<CanvasGroup>();
        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 1.0f, screenFadeTime).OnComplete(() => {
            //数秒後リザルトを表示する
            DOVirtual.DelayedCall(resultDrawTime, () => ResultManager.instance.EnableCanvas()).OnComplete(()=>{ endFight.SetActive(false); });
        });

        //クリア演出後リザルトを表示する
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
        if(bandMana.playerHP.Value> bandMana.playerMaxHP.Value)
        {
            bandMana.playerHP.Value = bandMana.playerMaxHP.Value;
        }
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
                DOVirtual.DelayedCall(cardInstatiateDelay, () =>
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

    void CardEffect()
    {
        var _enemy = enemyObjects[targetId];
        var playerPos = player.transform.GetChild(0).position;
        var enemyPos = _enemy.transform.GetChild(0).position;
        //エフェクトの表示
        //敵の位置に攻撃エフェクトを表示
        if (gunInCards[0].state.Damage > 0)
        {
            Instantiate(cardEffect[0], enemyPos, Quaternion.identity);
        }
        //自身の位置に防御エフェクトを表示
        if (gunInCards[0].state.buff[(int)BuffEnum.Bf_Diffence] > 0)
        {
            Instantiate(cardEffect[1], playerPos, Quaternion.identity);
        }
        //特殊エフェクトの表示
        if(gunInCards[0].state.buff[(int)BuffEnum.Bf_Diffence] <= 0 && gunInCards[0].state.Damage <= 0)
        {
            Instantiate(cardEffect[2], playerPos, Quaternion.identity);
        }

    }
    /// <summary>
    /// 攻撃処理
    /// </summary>
    /// <param name="_enemy"></param>
    public void Fire()
    {
        var _enemy = enemyObjects[targetId];
        //カードの移動中は処理をしない
        if (IsCardMoveNow || playerTurn == false) { return; }
        IsCardMoveNow = true;
        DOVirtual.DelayedCall(fireMaxTime, () => { IsCardMoveNow = false; });

        //AudioSystem.AudioControl.Instance.SE.CardSePlayOneShot(gunInCards[0].state.SE);

        //ターン経過処理
        ProgressTurn(gunInCards[0].state.AP);
        //エフェクトの表示を行う
        CardEffect();
        //カードのシステム処理を行う
        CardAction(_enemy);

        //デバフ減少
        plState.ATWeaken.Value = Mathf.Clamp(plState.ATWeaken.Value - 1, 0, 9999);
        plState.DFWeaken.Value = Mathf.Clamp(plState.DFWeaken.Value - 1, 0, 9999);



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
        deckInCards = DeckListManager.deckList.OrderBy(a => Guid.NewGuid()).ToList();
    }

    /// <summary>
    /// リボルバーをリロードする
    /// </summary>
    /// <param name="bulletNum"></param>
    public void Reload(int bulletNum = 6)
    {
        //リロード中はリロードできないようにする
        if (IsCardMoveNow || playerTurn == false) { return; }
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
         DOVirtual.DelayedCall(cardInstatiateDelay, () => { 
            
            InstantiateGunInCards(); 
        
        });


        //AudioSystem.AudioControl.Instance.SE.PlaySeOneShot(SEList.Reload);

        ProgressTurn(reloadCost);
    }

    /// <summary>
    /// コッキングしてリボルバー内の弾丸を１つずらす
    /// </summary>
    public void Cocking()
    {
        if (IsCardMoveNow || playerTurn == false) { return; }
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
            DOVirtual.DelayedCall(cardInstatiateDelay, () =>
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

        ProgressTurn(cockingCost);
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
            var enemy = Instantiate(enemyPrefab, enemyAddventPos.position, Quaternion.identity);
            enemy.transform.DOMove(enemyStartPos[enemyCount].position, enemyAddventMoveTime);//登場時の移動処理
            var script = enemy.GetComponent<Enemy.Enemy>();
            script.Initialize(enemysState[enemyCount]);
            script.fieldID = enemyCount;//ターゲット指定用のIDを代入

            enemyObjects.Add(script);//エネミーオブジェクトを保存

            enemyCount++;
        }

    }

    #endregion


    #region Debug
    public void AllDeathEnemys()
    {
        foreach (var _eObj in enemyObjects)
        {
            _eObj.ReceiveDamage(99999);
        }
    }

    #endregion


}