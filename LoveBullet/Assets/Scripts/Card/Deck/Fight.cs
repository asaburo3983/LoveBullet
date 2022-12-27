using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Card
{

    public class Fight : SingletonMonoBehaviour<Fight>
    {
        enum TURN
        {
            Player,
            Enemy,
        }

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
        [SerializeField,ReadOnly] ReactiveProperty<TURN> whoTurn;
        [SerializeField] public int shotTarget = 0;

        [Header("敵処理系")]
        public GameObject enemyBase;
        public List<Vector3> enemyPos;
        List<Enemy.Enemy.State> enemysState = new List<Enemy.Enemy.State>();
        public List<Enemy.Enemy> enemyObjects = new List<Enemy.Enemy>();
        

        // Start is called before the first frame update
        void Start()
        {
            if (SingletonCheck(this))
            {
                player = Player.instance;
                plState = player.gameState;

                InitializeStartDeck();
                whoTurn.Value = TURN.Player;

                //ターン切り替え処理
                whoTurn.Where(x => x == Fight.TURN.Player).Subscribe(x =>
                {
                    EndTurn_Enemy();
                    StartTurn_Player();
                }
                ).AddTo(this);
                whoTurn.Where(x => x == Fight.TURN.Enemy).Subscribe(x => {
                    EndTurn_Player();
                    StartTurn_Enemy();
                    }).AddTo(this);

                //TODO 戦闘はじめ処理仮置き
                StartFight();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //戦闘のカード情報を表示
            if (gunInCards.Count > 0) {
                card.Initialize(gunInCards[explanationCardNum]);
            }

            //ターゲット補正
            if (enemyObjects.Count <= shotTarget)
            {
                shotTarget = 0;
            }
            else if (shotTarget < 0)
            {
                shotTarget = enemyObjects.Count - 1;
            }

            if (whoTurn.Value == TURN.Player)
            {
                //各自処理（入力は別で行う）
                Reload(6);

                Shot();

                Cocking();

                TurnEnd_Player();
            }
            else if(whoTurn.Value == TURN.Enemy)
            {
                TurnEnd_Enemy();
            }



        }


        void StartFight()
        {

            Player.ResetAP();

            whoTurn.Value = TURN.Player;

            StartFight_ShuffleCard();

            AdventEnemys();

            reloadInput = true;
        }
        void StartTurn_Player()
        {
            Player.ResetAP();
            Player.ResetDF();
        }
        void EndTurn_Player()
        {
            whoTurn.Value = TURN.Enemy;
            
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
                enemy.gameState.acted.Value = false;//すべての敵を行動済みを解除する
                enemy.Action();//敵に順番に行動させる
                enemy.ResetDF();//DFをリセットする
            }
        }
        void EndTurn_Enemy()
        {

        }

        /// <summary>
        /// プレイヤーターンの終了　APが０になった際
        /// </summary>
        void TurnEnd_Player()
        {
            //APが0になるとターンを終了する
            if (plState.AP.Value <= 0)
            {
                whoTurn.Value = TURN.Enemy;
            }
        }
        /// <summary>
        /// エネミーターンの終了　すべてのエネミーが行動済みになった際
        /// </summary>
        void TurnEnd_Enemy()
        {
            //すべての敵が行動済みならターンを変更する
            bool endTurn = true;
            foreach (var enemy in enemyObjects)
            {
                if (enemy.gameState.acted.Value == false)
                {
                    endTurn = false;
                }
            }
            if (endTurn)
            {
                whoTurn.Value = TURN.Player;
            }
        }

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
            List<int> matchId = new List<int>();
            for (int i = 0; i < deckList.Count; i++)
            {
                bool match = false;
                int rand = -1;
                //ランダムのあたいが被らないようにする
                //TODO 現在処理不可を考えない処理
                do
                {
                    match = false;
                    rand = Random.Range(0, deckList.Count);
                    for (int h = 0; h < matchId.Count; h++)
                    {
                        if (matchId[h] == rand)
                        {
                            match = true;
                        }
                    }
                }
                while (match == true);
                matchId.Add(rand);
                deckInCards.Add(deckList[rand]);
            }
        }
        /// <summary>
        /// リボルバーをリロードする
        /// </summary>
        /// <param name="bulletNum"></param>
        public void Reload(int bulletNum = 6)
        {
            if (reloadInput==false) { return; }
            Player.MinusAP(plState.reloadAP);

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


            //AP減少
            Player.MinusAP(gunInCards[0].AP);
            //空のカードの場合の処理を入れるかも
            if (gunInCards[0].id == 0)
            {
            }
            else
            {
                CardSkill(shotTarget);
            }
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

            Player.MinusAP(plState.cockingAP);

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
            //デッキリストをシャッフルしてデッキに入れる
            List<int> matchId = new List<int>();
            for (int i = 0; i < trashInCards.Count; i++)
            {
                bool match = false;
                int rand = -1;
                //ランダムのあたいが被らないようにする
                //TODO 現在処理不可を考えない処理
                do
                {
                    match = false;
                    rand = Random.Range(0, trashInCards.Count);
                    for (int h = 0; h < matchId.Count; h++)
                    {
                        if (matchId[h] == rand)
                        {
                            match = true;
                        }
                    }
                }
                while (match == true);
                matchId.Add(rand);

                //トラッシュのカードを山に戻す
                deckInCards.Add(trashInCards[rand]);
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
            var rand = Random.Range(0, Enemy.AddventPattern.GetGroupMax(floor));
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