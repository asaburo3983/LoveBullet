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
        /// �����f�b�L�쐬
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
        /// �퓬���n�܂����ۂɏo����g�ݒ�������
        /// </summary>
        void InitializeFight()
        {
            deckInCards.Clear();
            gunInCards.Clear();
            trashInCards.Clear();

            //�f�b�L���X�g���V���b�t�����ăf�b�L�ɓ����
            List<int> matchId = new List<int>();
            for(int i = 0; i < deckList.Count; i++)
            {
                bool match = false;
                int rand=-1;
                //�����_���̂����������Ȃ��悤�ɂ���
                //TODO ���ݏ����s���l���Ȃ�����
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
        /// ���{���o�[�������[�h����
        /// </summary>
        /// <param name="bulletNum"></param>
        void Reload(int bulletNum = 6)
        {
            AP.Value -= reloadAP;

            //���{���o�[�������
            gunInCards.Clear();
            //�f�b�L���J�[�h�����߂�e�̌���菭�Ȃ���
            if (deckInCards.Count < bulletNum)
            {
                //�R�D���Ȃ��̂Ŏ̂ĎD���V���b�t�����ĎR�D�ɉ�����
                ResetTrush();
                bulletNum = deckInCards.Count;
            }
            //�f�b�L����J�[�h�ǉ�
            for (int i = 0; i < bulletNum; i++)
            {
                gunInCards.Add(deckInCards[i]);
                deckInCards.RemoveAt(0);
            }
        }

        /// <summary>
        /// �G�ɍU����^���鏈��
        /// </summary>
        /// <param name="target"></param>
        /// <param name="allTarget"></param>
        void Shot(int target = 0, bool allTarget = false)
        {
            //�G�Ƀ_���[�W�Ȃ�^���鏈��

            //AP����
            AP.Value -= gunInCards[0].AP;
            //�̂ĎD�ɃJ�[�h�����ă��{���o�[���甲��
            trashInCards.Add(gunInCards[0]);
            gunInCards.RemoveAt(0);
        }

        /// <summary>
        /// �R�b�L���O���ă��{���o�[���̒e�ۂ��P���炷
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
        /// �̂ĎD�ɂ���X�L�����ēx�R�ɖ߂�
        /// </summary>
        void ResetTrush()
        {
            //�f�b�L���X�g���V���b�t�����ăf�b�L�ɓ����
            List<int> matchId = new List<int>();
            for (int i = 0; i < trashInCards.Count; i++)
            {
                bool match = false;
                int rand = -1;
                //�����_���̂����������Ȃ��悤�ɂ���
                //TODO ���ݏ����s���l���Ȃ�����
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

                //�g���b�V���̃J�[�h���R�ɖ߂�
                deckInCards.Add(deckInCards[rand]);
            }

            //�g���b�V���̃J�[�h���Ȃ���
            trashInCards.Clear();
        }
    }
}