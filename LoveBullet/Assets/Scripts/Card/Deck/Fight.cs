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

        [Header("�J�[�h�Ǘ��n")]
        [SerializeField]Card.GENRE startDeckGenre;
        [SerializeField] List<int> startDecksId=new List<int>();
        [SerializeField] List<Card.State> deckList = new List<Card.State>();
        [SerializeField] List<Card.State> deckInCards = new List<Card.State>();
        [SerializeField] List<Card.State> gunInCards = new List<Card.State>();
        [SerializeField] List<Card.State> trashInCards = new List<Card.State>();

        Player player;
        Player.InGameState plState;

        [Header("�J�[�hUI�\���n�i���j")]
        [SerializeField] Card card;
        [SerializeField] int explanationCardNum = 0;

        [Header("���͌n")]
        [SerializeField] bool shotInput;
        [SerializeField] bool reloadInput;
        [SerializeField] bool cockingInput;

        [Header("���̑����[���n")]
        [SerializeField,ReadOnly] int floor;
        [SerializeField,ReadOnly]TURN whoTurn;
        [SerializeField,ReadOnly]int shotTarget = 0;

        [Header("�G�����n")]
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
            //�퓬�̃J�[�h����\��
            if (gunInCards.Count > 0) {
                card.Initialize(gunInCards[explanationCardNum]);
            }

            if (whoTurn == TURN.Player)
            {
                //�e�������i���͕͂ʂōs���j
                Reload(6);

                Shot();

                Cocking();

                //AP��0�ɂȂ�ƃ^�[�����I������
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
        /// �����f�b�L�쐬
        /// </summary>
        void InitializeStartDeck()
        {

            foreach (var cardId in startDecksId)
            {
                deckList.Add(Search.GetCard(cardId));
            }
        }

        /// <summary>
        /// �f�b�L���X�g����J�[�h���V���b�t�����ĎR�D���\�z����
        /// </summary>
        void StartFight_ShuffleCard()
        {
            deckInCards.Clear();
            gunInCards.Clear();
            trashInCards.Clear();

            //�f�b�L���X�g���V���b�t�����ăf�b�L�ɓ����
            List<int> matchId = new List<int>();
            for (int i = 0; i < deckList.Count; i++)
            {
                bool match = false;
                int rand = -1;
                //�����_���̂����������Ȃ��悤�ɂ���
                //TODO ���ݏ����s���l���Ȃ�����
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
        /// ���{���o�[�������[�h����
        /// </summary>
        /// <param name="bulletNum"></param>
        public void Reload(int bulletNum = 6)
        {
            if (reloadInput==false) { return; }
            plState.AP.Value -= plState.reloadAP;

            //���{���o�[������� ���g������Ȃ�Trush�Ɉړ�������
            foreach(var card in gunInCards)
            {
                if (card.id != 0) trashInCards.Add(card);
            }
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
                gunInCards.Add(deckInCards[0]);
                deckInCards.RemoveAt(0);
            }
            reloadInput = false;
        }

        /// <summary>
        /// �G�ɍU����^���鏈��
        /// </summary>
        /// <param name="target"></param>
        /// <param name="allTarget"></param>
        public void Shot(int target = 0, bool allTarget = false)
        {
            if (shotInput==false) { return; }
           

            //AP����
            plState.AP.Value -= gunInCards[0].AP;
            //��̃J�[�h�̏ꍇ�̏��������邩��
            if (gunInCards[0].id == 0)
            {
                //�G�Ƀ_���[�W�Ȃ�^���鏈���͂����Ȃ�
            }
            else
            {
                //�G�Ƀ_���[�W�Ȃ�^���鏈��
            }
            //�̂ĎD�ɃJ�[�h�����ă��{���o�[���甲��
            if (gunInCards[0].id != 0) trashInCards.Add(gunInCards[0]);//����̒e�ۂ̏ꍇ�̂ĎD�ɒu���Ȃ�
            gunInCards.RemoveAt(0);
            shotInput = false;

            //�T�Ԃɋ�̒e������K�v������
            gunInCards.Add(Search.GetCard(0));
        }

        /// <summary>
        /// �R�b�L���O���ă��{���o�[���̒e�ۂ��P���炷
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
        /// �̂ĎD�ɂ���X�L�����ēx�R�ɖ߂�
        /// </summary>
        public void ResetTrush()
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

                //�g���b�V���̃J�[�h���R�ɖ߂�
                deckInCards.Add(trashInCards[rand]);
            }

            //�g���b�V���̃J�[�h���Ȃ���
            trashInCards.Clear();
        }

        #endregion

        #region Enemy
        /// <summary>
        /// �K�w�ɉ������G���o��������
        /// </summary>
        void AdventEnemys()
        {
            //�G�����Z�b�g����
            enemys.Clear();

            //�G�̏o�������ɂ���
            //�Ƃ肠�����w��p�^�[���ŏo��������i���DB�����Ďw��p�^�[���̒��ł������_���ŏo��������Ȃǂ��s���j
            var rand = Random.Range(0, Enemy.AddventPattern.GetGroupMax(floor));
            var enemyGroup = Enemy.AddventPattern.GetGroup(floor, rand);

            int enemyCount = 0;
            foreach (var enemyId in enemyGroup)
            {
                enemyCount++;
                if (enemyId == -1) continue;
                enemys.Add(Enemy.Enemy.GetEnemyState(enemyId));
                //�G����
                var enemy = Instantiate(enemyBase, enemyPos[enemyCount - 1], Quaternion.identity);
                var script = enemy.GetComponent<Enemy.Enemy>();
                script.Initialize(enemys[enemyId]);

                enemyObjects.Add(script);
            }

        }

        #endregion
    }
}