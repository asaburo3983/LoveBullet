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
        [SerializeField]Card.GENRE startDeckGenre;
        [SerializeField] List<int> startDecksId=new List<int>();
        [SerializeField] List<Card.State> deckList = new List<Card.State>();
        [SerializeField] List<Card.State> deckInCards = new List<Card.State>();
        [SerializeField] List<Card.State> gunInCards = new List<Card.State>();
        [SerializeField] List<Card.State> trashInCards = new List<Card.State>();

        Player player;
        Player.InGameState plState;

        [Header("カードUI表示系（仮）")]
        [SerializeField] Card card;
        [SerializeField] int explanationCardNum = 0;

        [Header("入力系")]
        [SerializeField] bool shotInput;
        [SerializeField] bool reloadInput;
        [SerializeField] bool cockingInput;

        [Header("その他ルール系")]
        [SerializeField,ReadOnly] int floor;
        [SerializeField,ReadOnly]TURN whoTurn;
        [SerializeField,ReadOnly]int shotTarget = 0;

        [Header("敵処理系")]
        public GameObject enemyBase;
        public List<Vector3> enemyPos;
        List<Enemy.Enemy.State> enemys = new List<Enemy.Enemy.State>();
        List<Enemy.Enemy> enemyObjects = new List<Enemy.Enemy>();
        

        // Start is called before the first frame update
        void Start()
        {
            if (SingletonCheck(this))
            {
                player = Player.instance;
                plState = player.gameState;

                InitializeStartDeck();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //戦闘のカード情報を表示
            if (gunInCards.Count > 0) {
                card.Initialize(gunInCards[explanationCardNum]);
            }

            if (whoTurn == TURN.Player)
            {
                //各自処理（入力は別で行う）
                Reload(6);

                Shot();

                Cocking();

                //APが0になるとターンを終了する
                if (plState.AP.Value <= 0)
                {
                    EndTurn_Player();
                }
            }
            else if(whoTurn == TURN.Enemy)
            {

            }



        }


        void StartFight()
        {
            plState.AP.Value = 3;
            plState.APMax.Value = 3;

            Player.ResetAP();

            whoTurn = TURN.Player;

            StartFight_ShuffleCard();

            AdventEnemys();
        }
        void StartTurn_Player()
        {
            Player.ResetAP();

        }
        void EndTurn_Player()
        {
            whoTurn = TURN.Enemy;
        }
        void StartTurn_Enemy()
        {
            foreach(var enemy in enemys)
            {
                enemy.
            }
            gameState.acted.Value = false;
        }
        void EndTurn_Enemy()
        {

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
            plState.AP.Value -= plState.reloadAP;

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
            plState.AP.Value -= gunInCards[0].AP;
            //空のカードの場合の処理を入れるかも
            if (gunInCards[0].id == 0)
            {
                //敵にダメージなり与える処理はかかない
            }
            else
            {
                //敵にダメージなり与える処理
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

            plState.AP.Value -= plState.cockingAP;

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

        #endregion

        #region Enemy
        /// <summary>
        /// 階層に応じた敵を出現させる
        /// </summary>
        void AdventEnemys()
        {
            //敵をリセットする
            enemys.Clear();

            //敵の出現条件について
            //とりあえず指定パターンで出限させる（後にDB化して指定パターンの中でもランダムで出現させるなどを行う）
            var rand = Random.Range(0, Enemy.AddventPattern.GetGroupMax(floor));
            var enemyGroup = Enemy.AddventPattern.GetGroup(floor, rand);

            int enemyCount = 0;
            foreach (var enemyId in enemyGroup)
            {
                enemyCount++;
                if (enemyId == -1) continue;
                enemys.Add(Enemy.Enemy.GetEnemyState(enemyId));
                //敵生成
                var enemy = Instantiate(enemyBase, enemyPos[enemyCount - 1], Quaternion.identity);
                var script = enemy.GetComponent<Enemy.Enemy>();
                script.Initialize(enemys[enemyId]);

                enemyObjects.Add(script);
            }

        }

        #endregion
    }
}