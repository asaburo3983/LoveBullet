using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using DG.Tweening;

namespace Card
{

    public class Fight : SingletonMonoBehaviour<Fight>
    {
        [SerializeField] Transform fightCanvas;

        [Header("�J�[�h�Ǘ��n")]
        [SerializeField] Card.GENRE startDeckGenre;
        [SerializeField] List<int> startDecksId = new List<int>();
        public List<Card.State> deckList = new List<Card.State>();
        [SerializeField] List<Card.State> deckInCards = new List<Card.State>();

        ReactiveCollection<Card.State> gunInCards = new ReactiveCollection<Card.State>();
        public List<Card.State> GunInCards { get { return gunInCards.ToList(); } }
        public ReactiveCollection<Card.State> gunInCardsReactive => gunInCards;

        // �f�o�b�O�p�̏e���̃J�[�h�\������
#if UNITY_EDITOR
        [SerializeField]List<Card.State> GunInCard;
        private void FixedUpdate()
        {
            GunInCard = gunInCards.ToList();
        }
#endif
        [SerializeField] List<Card.State> trashInCards = new List<Card.State>();

        Player player;
        Player.InGameState plState;

        [Header("�J�[�hUI�\���n�i���j")]
        [SerializeField] Card card;
        [SerializeField] int explanationCardNum = 0;

        [Header("���̑����[���n")]
        [SerializeField,ReadOnly] int floor;
        [SerializeField] int reloadCost = 1;
        [SerializeField] int cockingCost = 1;
        [SerializeField, ReadOnly] bool playerTurn = true;
        public bool PlayerTurn => playerTurn;
        bool cocking = false;
        public bool CockingFlg { get { return cocking; } set { cocking = value; } }

        [Header("�G�����n")]
        public GameObject enemyBase;
        public List<Vector3> enemyPos;
        List<Enemy.Enemy.State> enemysState = new List<Enemy.Enemy.State>();
        public List<Enemy.Enemy> enemyObjects = new List<Enemy.Enemy>();
        [SerializeField,ReadOnly]int targetId = 0;
        public int TargetId => targetId;

        public ReactiveCollection<Enemy.Enemy> actEnemy = new ReactiveCollection<Enemy.Enemy>();

        // TweenAnimation
        Tween fireTw = null;

        [Header("�A�j���[�V����")]
        [SerializeField] float fireMoveDist = 1.0f;
        [SerializeField] float fireMoveTime = 0.1f;

        #region InitFunction

        private void Awake()
        {
            if (SingletonCheck(this,true))
            {
            }
        }
        void Start()
        {
            player = Player.instance;
            plState = player.gameState;
            playerTurn = true;

            InitializeStartDeck();

            //TODO �퓬�͂��ߏ������u��
            //StartFight();

            // �s������G�����ԂɃA�j���[�V�������A�S�Ă��I���ƍs���ł���悤�ɂ���
            actEnemy.ObserveRemove().Subscribe(x => {
                if (actEnemy.Count == 0) {
                    playerTurn = true;
                    return;
                }
                //�G�̍s������
                actEnemy[0].AttackAnimation();
            }).AddTo(this);

        }
        private void Update()
        {
            if (enemyObjects.Count == 0 && ResultManager.instance.isResult == false)
            {
                //TODO���u��
                ResultManager.instance.StartResult(15, true);
            }
        }
        //�퓬���J�n����
        public void StartFight()
        {
            StartFight_ShuffleCard();

            AdventEnemys();
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
            // �^�[�����������s�\���`�F�b�N
            if (!playerTurn) return;

            // �R�b�L���O���͍ēx�������Ȃ�
            if (cocking) return;

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

                gunInCards[0] = Search.GetCard(0);
                gunInCards.Move(0, 5);
                cocking = true;
            }

            // �G�l�~�[�̃^�[���o�ߏ���
            ProgressTurn(_progress);

            // �v���C���[�A�j���[�V����
            if (fireTw != null) fireTw.Kill(true);
            fireTw = player.transform.DOLocalMoveX(player.transform.localPosition.x + fireMoveDist, fireMoveTime)
                .SetLoops(2, LoopType.Yoyo).OnComplete(() => fireTw = null);
        }
        
        /// <summary>
        /// �^�[����i�s����
        /// </summary>
        /// <param name="_progressTurn">�o�߂���^�[��</param>
        public void ProgressTurn(int _progressTurn)
        {
            bool _flg = true;
            foreach(var _enemy in enemyObjects) {
                if (_enemy.ProgressTurn(_progressTurn)) {
                    _flg = playerTurn = false;
                    actEnemy.Add(_enemy);
                }
            }

            // �G���s��������
            if (!_flg) actEnemy[0].AttackAnimation();
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


        // �^�[�Q�b�g�Ďw��
        public void SetTarget(int _id)
        {
            targetId = _id;
        }

        public void SetTarget(Enemy.Enemy _enemy)
        {
            targetId = enemyObjects.IndexOf(_enemy);
        }


        /// <summary>
        /// ���݃^�[�Q�b�gID�ւ̍U������
        /// </summary>
        public void Fire()
        {
            Fire(enemyObjects[targetId]);
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

            Reload();
        }


        /// <summary>
        /// ���{���o�[�������[�h����
        /// </summary>
        /// <param name="bulletNum"></param>
        public void Reload(int bulletNum = 6)
        {
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
            }
            //�f�b�L����J�[�h�ǉ�
            for (int i = 0; i < bulletNum; i++)
            {
                gunInCards.Add(deckInCards[0]);
                deckInCards.RemoveAt(0);
            }

            ProgressTurn(reloadCost);
        }

        /// <summary>
        /// �R�b�L���O���ă��{���o�[���̒e�ۂ��P���炷
        /// </summary>
        public void Cocking()
        {
            // �^�[�����������s�\���`�F�b�N
            if (!playerTurn) return;

            // �R�b�L���O���͍ēx�������Ȃ�
            if (cocking) return;

            gunInCards.Move(0, 5);
            ProgressTurn(cockingCost);
            cocking = true;
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
               
                if (enemyId == -1) continue;
                enemysState.Add(Enemy.Enemy.GetEnemyState(enemyId));
                //�G����
                var enemy = Instantiate(enemyBase, enemyPos[enemyCount], Quaternion.identity, fightCanvas);
                var script = enemy.GetComponent<Enemy.Enemy>();
                script.Initialize(enemysState[enemyCount]);

                enemyObjects.Add(script);

                enemyCount++;
            }

        }

        #endregion
    }
}