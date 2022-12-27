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

        [Header("�G�����n")]
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

                //TODO �퓬�͂��ߏ������u��
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
                enemy.Action();//�G�ɏ��Ԃɍs��������
                enemy.ResetDF();//DF�����Z�b�g����
            }
        }

        #endregion

        #region Action

        /// <summary>
        /// �U������
        /// </summary>
        /// <param name="_enemy"></param>
        public void Fire(Enemy.Enemy _enemy)
        {
            var _card = gunInCards[0];
            int _damage = _card.AT;

            // �U���f�o�t�����݂���ꍇ�A�l�̕␳���s��
            if (plState.ATWeaken.Value > 0) {
                _damage *= player.Rate.AT;
                _damage /= 100;
            }
            
            // �U������
            if (gunInCards[0].Whole) {

                // �S�̍U��
                foreach(var _eObj in enemyObjects) {
                    _eObj.ReceiveDamage(_damage);
                    _eObj.ReceiveStan(_card.Stan);
                    _eObj.ReceiveATWeaken(_card.ATWeaken);
                    _eObj.ReceiveDFWeaken(_card.DFWeaken);
                }
            }
            else {
                // �P�̍U��
                _enemy.ReceiveDamage(_damage);
                _enemy.ReceiveStan(_card.Stan);
                _enemy.ReceiveATWeaken(_card.ATWeaken);
                _enemy.ReceiveDFWeaken(_card.DFWeaken);
            }

            // �v���C���[�X�e�[�^�X����
            int _progress = _card.AP;
            plState.DF.Value += _card.DF;
            plState.freeCocking += _card.Cocking;
            plState.ATWeaken.Value = Mathf.Clamp(plState.ATWeaken.Value - 1, 0, 9999);
            plState.DFWeaken.Value = Mathf.Clamp(plState.DFWeaken.Value - 1, 0, 9999);


            // ���������[�h���邩
            if (_card.Reload > 0) {
                for(int i= _card.Reload; i > 0; i--) {
                    Reload();
                }
            }
            else {
                //�̂ĎD�ɃJ�[�h�����ă��{���o�[���甲��
                if (_card.id != 0) trashInCards.Add(_card);//����̒e�ۂ̏ꍇ�̂ĎD�ɒu���Ȃ�
                gunInCards.RemoveAt(0);

                //�T�Ԃɋ�̒e������K�v������
                gunInCards.Add(Search.GetCard(0));
            }

            // �G�l�~�[�̃^�[���o�ߏ���
            ProgressTurn(_progress);
        }


        /// <summary>
        /// �^�[����i�s����
        /// </summary>
        /// <param name="_progressTurn">�o�߂���^�[��</param>
        public void ProgressTurn(int _progressTurn)
        {
            foreach(var _enemy in enemyObjects) {
                _enemy.ProgressTurn(_progressTurn);
            }
        }

        /// <summary>
        /// �v���C���[�̔�_���[�W����
        /// </summary>
        /// <param name="_damage"></param>
        public void ReceiveDamage(int _damage)
        {
            int dmg = _damage;
            // �h��f�o�t�����݂���ꍇ�A�l�̕␳���s��
            if (plState.DFWeaken.Value > 0) {
                dmg *= player.Rate.DF;
                dmg /= 100;
            }

            // �h��o�t������ꍇ�A�_���[�W���Z
            if (dmg <= plState.DF.Value) {
                plState.DF.Value -= dmg;
            }
            else {
                plState.hp.Value -= dmg - plState.DF.Value;
            }
        }

        /// <summary>
        /// �v���C���[�̔�f�o�t����
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
            deckInCards = deckList.OrderBy(a => Guid.NewGuid()).ToList();

            reloadInput = true;
            Reload();
        }


        /// <summary>
        /// ���{���o�[�������[�h����
        /// </summary>
        /// <param name="bulletNum"></param>
        public void Reload(int bulletNum = 6)
        {
            if (reloadInput==false) { return; }

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
            // �g���b�V�����X�g���V���b�t�����ăf�b�L�ɓ����
            trashInCards = trashInCards.OrderBy(a => Guid.NewGuid()).ToList();
            
            foreach(var _cards in trashInCards) {
                deckInCards.Add(_cards);
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
            var rand = UnityEngine.Random.Range(0, Enemy.AddventPattern.GetGroupMax(floor));
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