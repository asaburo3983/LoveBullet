using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Card
{
    
    public class Deck : SingletonMonoBehaviour<Deck>
    {
        [SerializeField]GENRE startDeckGenre;
        [SerializeField] List<List<int>> startDecksId;

        [SerializeField, ReadOnly] List<State> deckList;

        [SerializeField, ReadOnly] List<State> deckInCards;
        [SerializeField, ReadOnly] List<State> gunInCards;
        [SerializeField, ReadOnly] List<State> trashInCards;

        public int reloadAP=1;
        public int cockingAP=1;

        public ReactiveProperty<int> AP;

        public int AP_Max = 3;

        // Start is called before the first frame update
        void Start()
        {
            InitializeDeck();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 初期デッキ作成
        /// </summary>
        void InitializeDeck()
        {
            var startDeck = startDecksId[(int)startDeckGenre];
            foreach (var cardId in startDeck)
            {
                deckList.Add(Search.GetCard(cardId));
            }
        }
        /// <summary>
        /// 戦闘が始まった際に出来を組み直す処理
        /// </summary>
        void InitializeFight()
        {
            deckInCards.Clear();
            gunInCards.Clear();
            trashInCards.Clear();

            //デッキリストをシャッフルしてデッキに入れる
            List<int> matchId = new List<int>();
            for(int i = 0; i < deckList.Count; i++)
            {
                bool match = false;
                int rand=-1;
                //ランダムのあたいが被らないようにする
                //TODO 現在処理不可を考えない処理
                do
                {
                    rand = Random.Range(0, deckList.Count);
                    for (int h = 0; h < matchId.Count; h++)
                    {
                        if (matchId[h] == rand)
                        {
                            match = true;
                            return;
                        }
                    }
                } 
                while (match == true);
                matchId.Add(rand);
                deckInCards[i] = deckList[rand];
            }
        }

        void StartTurn()
        {
            AP.Value = AP_Max;
        }
        /// <summary>
        /// リボルバーをリロードする
        /// </summary>
        /// <param name="bulletNum"></param>
        void Reload(int bulletNum = 6)
        {
            AP.Value -= reloadAP;

            //リボルバー内を空に
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
                gunInCards.Add(deckInCards[i]);
                deckInCards.RemoveAt(0);
            }
        }

        /// <summary>
        /// 敵に攻撃を与える処理
        /// </summary>
        /// <param name="target"></param>
        /// <param name="allTarget"></param>
        void Shot(int target = 0, bool allTarget = false)
        {
            //敵にダメージなり与える処理

            //AP減少
            AP.Value -= gunInCards[0].AP;
            //捨て札にカードを入れてリボルバーから抜く
            trashInCards.Add(gunInCards[0]);
            gunInCards.RemoveAt(0);
        }

        /// <summary>
        /// コッキングしてリボルバー内の弾丸を１つずらす
        /// </summary>
        void Cocking()
        {
            AP.Value -= cockingAP;

            var zeroCard = gunInCards[0];
            var sixCard = gunInCards[5];
            gunInCards[0] = sixCard;
            gunInCards[5] = zeroCard;
        }
        /// <summary>
        /// 捨て札にあるスキルを再度山に戻す
        /// </summary>
        void ResetTrush()
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
                    rand = Random.Range(0, trashInCards.Count);
                    for (int h = 0; h < matchId.Count; h++)
                    {
                        if (matchId[h] == rand)
                        {
                            match = true;
                            return;
                        }
                    }
                }
                while (match == true);
                matchId.Add(rand);

                //トラッシュのカードを山に戻す
                deckInCards.Add(deckInCards[rand]);
            }

            //トラッシュのカードをなくす
            trashInCards.Clear();
        }
    }
}