using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;

namespace Card
{

    public class Fight : SingletonMonoBehaviour<Fight>
    {
        [Header("カード管理系")]
        [SerializeField] Card.GENRE startDeckGenre;
        [SerializeField] List<int> startDecksId = new List<int>();
        [SerializeField] List<Card.State> deckList = new List<Card.State>();
        [SerializeField] List<Card.State> deckInCards = new List<Card.State>();

        [SerializeField] List<Card.State> gunInCards = new List<Card.State>();
        public List<Card.State> GunInCards {get{ return gunInCards; } }
       [SerializeField] List<Card.State> trashInCards = new List<Card.State>();

        Player player;
        Player.InGameState plState;

        [Header("カードUI表示系（仮）")]
        [SerializeField] Card card;
        [SerializeField] int explanationCardNum = 0;

        [Header("入力系")]
        [SerializeField] public bool shotInput;
        [SerializeField] public bool reloadInput;
        [SerializeField] public bool cockingInput;

        [Header("その他ルール系")]
        [SerializeField,ReadOnly] int floor;

        [Header("敵処理系")]
        public GameObject enemyBase;
        public List<Vector3> enemyPos;
        List<Enemy.Enemy.State> enemysState = new List<Enemy.Enemy.State>();
        public List<Enemy.Enemy> enemyObjects = new List<Enemy.Enemy>();

        #region InitFunction

        void Start()
        {
            if (SingletonCheck(this))
            {
                player = Player.instance;
                plState = player.gameState;

                InitializeStartDeck();

                //TODO 戦闘はじめ処理仮置き
                StartFight();
            }
        }

        void StartFight()
        {
            StartFight_ShuffleCard();

            AdventEnemys();

            reloadInput = true;
        }
        void StartTurn_Player()
        {
            Player.ResetDF();
        }

        void StartTurn_Enemy()
        {
            //エネミー側で削除されていた場合リストから削除しておく
            for(int i=0;i< enemyObjects.Count; i++)
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
            var _card = gunInCards[0];
            int _damage = _card.AT;

            // 攻撃デバフが存在する場合、値の補正を行う
            if (plState.ATWeaken.Value > 0) {
                _damage *= player.Rate.AT;
                _damage /= 100;
            }
            
            // 攻撃処理
            if (gunInCards[0].Whole) {

                // 全体攻撃
                foreach(var _eObj in enemyObjects) {
                    _eObj.ReceiveDamage(_damage);
                    _eObj.ReceiveStan(_card.Stan);
                    _eObj.ReceiveATWeaken(_card.ATWeaken);
                    _eObj.ReceiveDFWeaken(_card.DFWeaken);
                }
            }
            else {
                // 単体攻撃
                _enemy.ReceiveDamage(_damage);
                _enemy.ReceiveStan(_card.Stan);
                _enemy.ReceiveATWeaken(_card.ATWeaken);
                _enemy.ReceiveDFWeaken(_card.DFWeaken);
            }

            // プレイヤーステータス加減
            int _progress = _card.AP;
            plState.DF.Value += _card.DF;
            plState.freeCocking += _card.Cocking;
            plState.ATWeaken.Value = Mathf.Clamp(plState.ATWeaken.Value - 1, 0, 9999);
            plState.DFWeaken.Value = Mathf.Clamp(plState.DFWeaken.Value - 1, 0, 9999);


            // 強制リロードするか
            if (_card.Reload > 0) {
                for(int i= _card.Reload; i > 0; i--) {
                    Reload();
                }
            }
            else {
                //捨て札にカードを入れてリボルバーから抜く
                if (_card.id != 0) trashInCards.Add(_card);//からの弾丸の場合捨て札に置かない
                gunInCards.RemoveAt(0);

                //５番に空の弾を入れる必要がある
                gunInCards.Add(Search.GetCard(0));
            }

            // エネミーのターン経過処理
            ProgressTurn(_progress);
        }


        /// <summary>
        /// ターンを進行する
        /// </summary>
        /// <param name="_progressTurn">経過するターン</param>
        public void ProgressTurn(int _progressTurn)
        {
            foreach(var _enemy in enemyObjects) {
                _enemy.ProgressTurn(_progressTurn);
            }
        }

        /// <summary>
        /// プレイヤーの被ダメージ処理
        /// </summary>
        /// <param name="_damage"></param>
        public void ReceiveDamage(int _damage)
        {
            int dmg = _damage;
            // 防御デバフが存在する場合、値の補正を行う
            if (plState.DFWeaken.Value > 0) {
                dmg *= player.Rate.DF;
                dmg /= 100;
            }

            // 防御バフがある場合、ダメージ減算
            if (dmg <= plState.DF.Value) {
                plState.DF.Value -= dmg;
            }
            else {
                plState.hp.Value -= dmg - plState.DF.Value;
            }
        }

        /// <summary>
        /// プレイヤーの被デバフ処理
        /// </summary>
        /// <param name="_atk"></param>
        /// <param name="_def"></param>
        public void ReceiveWeaken(int _atk,int _def)
        {
            plState.ATWeaken.Value += _atk;
            plState.DFWeaken.Value += _def;
        }


        #endregion

        #region Card

        /// <summary>
        /// 初期デッキ作成
        /// </summary>
        void InitializeStartDeck()
        {

            foreach (var cardId in startDecksId)
            {
                deckList.Add(Search.GetCard(cardId));
            }
        }

        /// <summary>
        /// デッキリストからカードをシャッフルして山札を構築する
        /// </summary>
        void StartFight_ShuffleCard()
        {
            deckInCards.Clear();
            gunInCards.Clear();
            trashInCards.Clear();

            //デッキリストをシャッフルしてデッキに入れる
            deckInCards = deckList.OrderBy(a => Guid.NewGuid()).ToList();

            reloadInput = true;
            Reload();
        }


        /// <summary>
        /// リボルバーをリロードする
        /// </summary>
        /// <param name="bulletNum"></param>
        public void Reload(int bulletNum = 6)
        {
            if (reloadInput==false) { return; }

            //リボルバー内を空に 中身があるならTrushに移動させる
            foreach(var card in gunInCards)
            {
                if (card.id != 0) trashInCards.Add(card);
            }
            gunInCards.Clear();


            //デッキ内カードが込める弾の個数より少ない時
            if (deckInCards.Count < bulletNum)
            {
                //山札がないので捨て札をシャッフルして山札に加える
                ResetTrush();
                bulletNum = deckInCards.Count;
            }
            //デッキからカード追加
            for (int i = 0; i < bulletNum; i++)
            {
                gunInCards.Add(deckInCards[0]);
                deckInCards.RemoveAt(0);
            }
            reloadInput = false;
        }


        /// <summary>
        /// 敵に攻撃を与える処理
        /// </summary>
        /// <param name="target"></param>
        /// <param name="allTarget"></param>
        public void Shot(int target = 0, bool allTarget = false)
        {
            if (shotInput==false) { return; }
            //捨て札にカードを入れてリボルバーから抜く
            if (gunInCards[0].id != 0) trashInCards.Add(gunInCards[0]);//からの弾丸の場合捨て札に置かない
            gunInCards.RemoveAt(0);
            shotInput = false;

            //５番に空の弾を入れる必要がある
            gunInCards.Add(Search.GetCard(0));
        }

        /// <summary>
        /// コッキングしてリボルバー内の弾丸を１つずらす
        /// </summary>
        public void Cocking()
        {
            if (cockingInput==false) { return; }


            var zeroCard = gunInCards[0];
            gunInCards.RemoveAt(0);
            gunInCards.Add(zeroCard);
            cockingInput = false;
        }
        /// <summary>
        /// 捨て札にあるスキルを再度山に戻す
        /// </summary>
        public void ResetTrush()
        {
            // トラッシュリストをシャッフルしてデッキに入れる
            trashInCards = trashInCards.OrderBy(a => Guid.NewGuid()).ToList();
            
            foreach(var _cards in trashInCards) {
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
            enemyObjects[target].ReceiveDamage(gunInCards[0].AT);//敵へ攻撃処理
            plState.DF.Value += gunInCards[0].DF;//プレイヤーへ防御追加
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
                enemyCount++;
                if (enemyId == -1) continue;
                enemysState.Add(Enemy.Enemy.GetEnemyState(enemyId));
                //敵生成
                var enemy = Instantiate(enemyBase, enemyPos[enemyCount - 1], Quaternion.identity);
                var script = enemy.GetComponent<Enemy.Enemy>();
                script.Initialize(enemysState[enemyId-1]);

                enemyObjects.Add(script);
            }

        }

        #endregion
    }
}