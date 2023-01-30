using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    public class Search
    {
        public static Card.State GetCard(int id)
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
        public static List<Card.State> GetCards(
        List<Card.State> cards=null,
        Card.GENRE genre = Card.GENRE.Max,
        Card.TYPE type = Card.TYPE.Max,
        string name = null,
        int AP = -1,
        int AT = -1,
        int DF = -1)
        {
            List<Card.State> rt_cards = new List<Card.State>();
            List<Card.State> searchCards = cards;
            searchCards = cards ?? CacheData.instance.cardStates;//�J�[�h�v�[���̎w�肪�Ȃ���΂��ׂẴf�[�^����T��

            foreach (var card in searchCards)
            {
                bool correct = true;

                if (genre != Card.GENRE.Max && card.genre != genre)
                {
                    correct = false;
                }
                if (type != Card.TYPE.Max && card.type != type)
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
                if (AT != -1 && card.Damage != AT)
                {
                    correct = false;
                }
                if (DF != -1 && card.buff[(int)BuffEnum.Bf_Diffence] != DF)
                {
                    correct = false;
                }

                if (correct) rt_cards.Add(card);
            }
            return rt_cards;
        }

    }

}