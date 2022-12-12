using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    public class Search
    {
        public static State GetCard(int id)
        {
            return CacheData.instance.cardStates[id];
        }
        /// <summary>
        /// �J�[�h��T������@�l���������̂�Ԃ�
        /// �g��Ȃ����ڂ̓f�t�H���g�œ����Ă�����̂��g�p����
        /// </summary>
        /// <param name="cards">�T������J�[�h�v�[��</param>
        /// <param name="genre"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="AP"></param>
        /// <param name="AT"></param>
        /// <param name="DF"></param>
        /// <returns></returns>
        public static List<State> GetCards(
        List<State> cards=null,
        GENRE genre = GENRE.Max,
        TYPE type = TYPE.Max,
        string name = null,
        int AP = -1,
        int AT = -1,
        int DF = -1)
        {
            List<State> rt_cards = new List<State>();
            List<State> searchCards = cards;
            searchCards = cards ?? CacheData.instance.cardStates;//�J�[�h�v�[���̎w�肪�Ȃ���΂��ׂẴf�[�^����T��

            foreach (var card in searchCards)
            {
                bool correct = true;

                if (genre != GENRE.Max && card.genre != genre)
                {
                    correct = false;
                }
                if (type != TYPE.Max && card.type != type)
                {
                    correct = false;
                }
                if (name != null && card.name != name)
                {
                    correct = false;
                }
                if (AP != -1 && card.AP != AP)
                {
                    correct = false;
                }
                if (AT != -1 && card.AT != AT)
                {
                    correct = false;
                }
                if (DF != -1 && card.DF != DF)
                {
                    correct = false;
                }

                if (correct) rt_cards.Add(card);
            }
            return rt_cards;
        }

    }

}