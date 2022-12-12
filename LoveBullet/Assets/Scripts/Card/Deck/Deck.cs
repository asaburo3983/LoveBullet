using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Card
{
    
    public class Deck : MonoBehaviour
    {
        [SerializeField]GENRE startDeckGenre;
        [SerializeField] List<List<int>> startDecksId;

        [SerializeField, ReadOnly] List<State> deckList;

        [SerializeField, ReadOnly] List<State> deckInCard;
        [SerializeField, ReadOnly] List<State> gunInCard;
        [SerializeField, ReadOnly] List<State> trashInCard;

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
        void InitializeFight()
        {
            deckInCard.Clear();
            gunInCard.Clear();
            trashInCard.Clear();

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
                deckInCard[i] = deckList[rand];
            }
        }

    }
}