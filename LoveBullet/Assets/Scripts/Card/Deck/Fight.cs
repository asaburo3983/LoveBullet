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
        [SerializeField] Card.GENRE startDeckGenre;
        [SerializeField] List<int> startDecksId = new List<int>();
        [SerializeField] List<Card.State> deckList = new List<Card.State>();
        [SerializeField] List<Card.State> deckInCards = new List<Card.State>();

        [SerializeField] List<Card.State> gunInCards = new List<Card.State>();
        public List<Card.State> GunInCards {get{ return gunInCards; } }
       [SerializeField] List<Card.State> trashInCards = new List<Card.State>();

        Player player;
        Player.InGameState plState;

        [Header("�J�[�hUI�\���n�i���j")]
        [SerializeField] Card card;
        [SerializeField] int explanationCardNum = 0;

        [Header("���͌n")]
        [SerializeField] public bool shotInput;
        [SerializeField] public bool reloadInput;
        [SerializeField] public bool cockingInput;

        [Header("���̑����[���n")]
        [SerializeField,ReadOnly] int floor;
        [SerializeField,ReadOnly] ReactiveProperty<TURN> whoTurn;
        [SerializeField] public int shotTarget = 0;

        [Header("�G�����n")]
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

                //�^�[���؂�ւ�����
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

                //TODO �퓬�͂��ߏ������u��
                StartFight();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //�퓬�̃J�[�h����\��
            if (gunInCards.Count > 0) {
                card.Initialize(gunInCards[explanationCardNum]);
            }

            //�^�[�Q�b�g�␳
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
                //�e�������i���͕͂ʂōs���j
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
            //�G�l�~�[���ō폜����Ă����ꍇ���X�g����폜���Ă���
            for(int i=0;i< enemyObjects.Count; i++)
            {
                if (enemyObjects[i] == null)
                {
                    enemyObjects.RemoveAt(i);
                }
            }
            foreach (var enemy in enemyObjects)
            {
                enemy.gameState.acted.Value = false;//���ׂĂ̓G���s���ς݂���������
                enemy.Action();//�G�ɏ��Ԃɍs��������
                enemy.ResetDF();//DF�����Z�b�g����
            }
        }
        void EndTurn_Enemy()
        {

        }

        /// <summary>
        /// �v���C���[�^�[���̏I���@AP���O�ɂȂ�����
        /// </summary>
        void TurnEnd_Player()
        {
            //AP��0�ɂȂ�ƃ^�[�����I������
            if (plState.AP.Value <= 0)
            {
                whoTurn.Value = TURN.Enemy;
            }
        }
        /// <summary>
        /// �G�l�~�[�^�[���̏I���@���ׂẴG�l�~�[���s���ς݂ɂȂ�����
        /// </summary>
        void TurnEnd_Enemy()
        {
            //���ׂĂ̓G���s���ς݂Ȃ�^�[����ύX����
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
            Player.MinusAP(plState.reloadAP);

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
            Player.MinusAP(gunInCards[0].AP);
            //��̃J�[�h�̏ꍇ�̏��������邩��
            if (gunInCards[0].id == 0)
            {
            }
            else
            {
                CardSkill(shotTarget);
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

            Player.MinusAP(plState.cockingAP);

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

        /// <summary>
        /// �e�ۂ̔\�͂��g�p����
        /// </summary>
        /// <param name="target"></param>
        /// <param name="allTarget"></param>
        void CardSkill(int target = 0, bool allTarget = false)
        {
            if (enemyObjects.Count <= 0) { Debug.LogError("�G�����݂��Ȃ���ԂŃX�L���𔭓����Ă��܂�"); return; }
            //�G�Ƀ_���[�W�Ȃ�^���鏈��
            enemyObjects[target].ReceiveDamage(gunInCards[0].AT);//�G�֍U������
            plState.DF.Value += gunInCards[0].DF;//�v���C���[�֖h��ǉ�
        }
        #endregion

        #region Enemy
        /// <summary>
        /// �K�w�ɉ������G���o��������
        /// </summary>
        void AdventEnemys()
        {
            //�G�����Z�b�g����
            enemysState.Clear();

            //�G�̏o�������ɂ���
            //�Ƃ肠�����w��p�^�[���ŏo��������i���DB�����Ďw��p�^�[���̒��ł������_���ŏo��������Ȃǂ��s���j
            var rand = Random.Range(0, Enemy.AddventPattern.GetGroupMax(floor));
            var enemyGroup = Enemy.AddventPattern.GetGroup(floor, rand);

            int enemyCount = 0;
            foreach (var enemyId in enemyGroup)
            {
                enemyCount++;
                if (enemyId == -1) continue;
                enemysState.Add(Enemy.Enemy.GetEnemyState(enemyId));
                //�G����
                var enemy = Instantiate(enemyBase, enemyPos[enemyCount - 1], Quaternion.identity);
                var script = enemy.GetComponent<Enemy.Enemy>();
                script.Initialize(enemysState[enemyId-1]);

                enemyObjects.Add(script);
            }

        }

        #endregion
    }
}