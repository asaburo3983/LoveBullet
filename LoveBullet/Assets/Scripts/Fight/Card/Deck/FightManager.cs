using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UniRx;
using DG.Tweening;

public class FightManager : SingletonMonoBehaviour<FightManager>
{

    //�}�l�[�W���[�n
    BandManager bandMana;

    [Header("�J�[�h�Ǘ��n")]
    [SerializeField] List<Card.Card> deckInCards = new List<Card.Card>();
    public List<Card.Card> gunInCards = new List<Card.Card>();
    [SerializeField] List<Card.Card> trashInCards = new List<Card.Card>();
  
    //�v���C���[�n
    Player player;
    Player.InGameState plState;

    [Header("���̑����[���n")]
    public static int floor;
    [SerializeField] int reloadCost = 1;
    [SerializeField] int cockingCost = 1;
    [SerializeField, ReadOnly] bool playerTurn = true;
    public bool PlayerTurn => playerTurn;

    [Header("�G�����n")]
    public GameObject enemyPrefab;
    [SerializeField] List<Transform> enemyStartPos;//�G�l�~�[�����ʒu
    [SerializeField] Transform enemyAddventPos;//�G�l�~�[��ʒu
    [SerializeField] float enemyAddventMoveTime;
    List<Enemy.Enemy.State> enemysState = new List<Enemy.Enemy.State>();
    public List<Enemy.Enemy> enemyObjects = new List<Enemy.Enemy>();
    public int targetId = 0;

    public ReactiveCollection<Enemy.Enemy> actEnemy = new ReactiveCollection<Enemy.Enemy>();



    [Header("�J�[�h�����n")]
    [SerializeField] Transform fightCanvas;
    public GameObject cardPrefab;
    List<GameObject> cards=new List<GameObject>();

    [SerializeField] List<Transform> cardBasePos;//�J�[�h��ʒu
    [SerializeField] Transform cardReloadPos;//�����[�h���ړ��ʒu
    [SerializeField] float reloadMoveSpeed;
    [SerializeField, Tooltip("�J�[�h�̈ړ���ɃJ�[�h�𐶐�����܂ł̎���")] float cardInstatiateDelay;

    [SerializeField] float fireMaxTime;
    [SerializeField] float reloadMaxTime;
    [SerializeField] float cockingMaxTime;
    bool IsCardMoveNow;

    [Header("�X�N���[���`��n")]
    ReactiveProperty<bool> isEndFight = new ReactiveProperty<bool>();
    [SerializeField] GameObject startFight;
    [SerializeField] GameObject endFight;
    [SerializeField] float screenFadeTime;

    [Header("���U���g�\���n")]
    [SerializeField] float resultDrawTime;

    [Header("�G�t�F�N�g")]
    [SerializeField] List<GameObject> cardEffect;
    #region InitFunction

    private void Awake()
    {
        if (SingletonCheck(this, true))
        {
        }
    }
    void Start()
    {
        bandMana = BandManager.instance;
        player = Player.instance;
        plState = player.gameState;
        playerTurn = true;

        //// �s������G�����ԂɃA�j���[�V�������A�S�Ă��I���ƍs���ł���悤�ɂ���
        actEnemy.ObserveRemove().Subscribe(x =>
        {
            if (actEnemy.Count == 0)
            {
                playerTurn = true;
                plState.Def.Value = 0;
                return;
            }
            //�G�̍s������
            actEnemy[0].AttackAnimation();
        }).AddTo(this);

        //�퓬�J�n���o
        FloorStart();

        //�G�l�~�[�o���@���o��
        AdventEnemys();

        //�f�b�L���V���b�t������
        ShuffleDeck();
        //�J�[�h�̏�������
        InstantiateGunInCards();

        //�퓬�I������
        isEndFight.Where(x => x == true).Subscribe(x => { FloorClear(); }).AddTo(this);
    }
    private void Update()
    {
        if (enemyObjects.Count <= 0)
        {
            //�G�l�~�[�����ׂĂ��Ȃ��Ȃ����ۂɊK�w�N���A���s��
            isEndFight.Value = true;
        }
       
        }
    void FloorStart()
    {
        //�Q�[���J�n���o���s��
        startFight.SetActive(true);
        var canvasGroup = startFight.GetComponent<CanvasGroup>();
        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 0.0f, screenFadeTime).OnComplete(() =>
        {
            startFight.SetActive(false);
        });
    }
    void FloorClear()
    {
        //�N���A���o���s��
        endFight.SetActive(true);
        var canvasGroup = endFight.GetComponent<CanvasGroup>();
        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 1.0f, screenFadeTime).OnComplete(() => {
            //���b�ナ�U���g��\������
            DOVirtual.DelayedCall(resultDrawTime, () => ResultManager.instance.EnableCanvas()).OnComplete(()=>{ endFight.SetActive(false); });
        });

        //�N���A���o�ナ�U���g��\������
    }
    /// <summary>
    /// �J�[�h��������
    /// </summary>
    void InstantiateGunInCards()
    {

        for (int i = 0; i < 6; i++)
        {
            InstantiateGunInCard(i);
            
        }
    }
    void InstantiateGunInCard(int num, bool emptyCard = false, bool delay = true)
    {
        //����
        var obj=Instantiate(cardPrefab, cardReloadPos.position, Quaternion.identity,fightCanvas);
        Card.Card.State newCardState;
        //��̒e�ۂ�����ꍇ��0�Ԃ��Q�Ƃ���
        if (emptyCard)
        {
            newCardState = Card.Search.GetCard(0);
            Card.Card EmptyCard = new Card.Card();
            EmptyCard.SetState(newCardState);
            gunInCards.Add(EmptyCard);
        }
        else
        {
            newCardState = deckInCards[0].state;
            gunInCards.Add(deckInCards[0]);//�f�b�L�̃J�[�h����D��
            deckInCards.RemoveAt(0);//�f�b�L�̃J�[�h���폜
        }
       
        obj.transform.GetChild(0).GetComponent<Card.Card>().Initialize(newCardState);

        var delayTime = num * cardInstatiateDelay;
        if (delay == false) delayTime = 0.001f;

        DOVirtual.DelayedCall(delayTime, () => {
            obj.transform.DOMoveY(cardBasePos[num].position.y, reloadMoveSpeed).OnComplete(() => { obj.transform.DOMoveX(cardBasePos[num].position.x, reloadMoveSpeed); });
        });
        //��ʒu�܂ňړ�������
        cards.Add(obj);
    }
    void InstantiateGunInCard(int num,Card.Card card)
    {
        //�f�[�^�����ꂩ����
        gunInCards.Add(card);//�f�b�L�̃J�[�h����D��

        //����
        var obj = Instantiate(cardPrefab, cardReloadPos.position, Quaternion.identity, fightCanvas);
        Card.Card.State newCardState;

        newCardState = card.state;
        obj.transform.GetChild(0).GetComponent<Card.Card>().Initialize(newCardState);

         obj.transform.DOMoveY(cardBasePos[num].position.y, reloadMoveSpeed).OnComplete(() => { obj.transform.DOMoveX(cardBasePos[num].position.x, reloadMoveSpeed); });
        //��ʒu�܂ňړ�������
        cards.Add(obj);
    }

    #endregion

    #region Action

    void CardAction(Enemy.Enemy _enemy)
    {
        //�J�[�h�̍s��������
        var selectCard = gunInCards[0];
        var cardState = selectCard.state;
        int _damage = cardState.Damage + selectCard.powerUp.AT + plState.Atk.Value + plState.Atk_Never.Value;
        // �U���f�o�t�����݂���ꍇ�A�l�̕␳���s��
        if (plState.ATWeaken.Value > 0)
        {
            _damage *= player.Rate.AT;
            _damage /= 100;
        }
        //// �U������

        // �U���񐔕����[�v����
        var attackNum = cardState.MultiAttack;
        if (cardState.Damage > 0 && attackNum == 0)
        {
            attackNum++;
        }
        for (int i = 0; i < attackNum; i++)
        {
            //�S�̍U��
            if (cardState.Whole)
            {
                foreach (var _eObj in enemyObjects)
                {
                    _eObj.ReceiveDamage(_damage);
                    _eObj.ReceiveStan(cardState.buff[(int)BuffEnum.Bf_Stan]);
                    _eObj.ReceiveATWeaken(cardState.buff[(int)BuffEnum.Bf_Weaken_Attack]);
                    _eObj.ReceiveDFWeaken(cardState.buff[(int)BuffEnum.Bf_Weaken_Diffence]);
                }
            }
            // �P�̍U��
            else
            {

                _enemy.ReceiveDamage(_damage);
                _enemy.ReceiveStan(cardState.buff[(int)BuffEnum.Bf_Stan]);
                _enemy.ReceiveATWeaken(cardState.buff[(int)BuffEnum.Bf_Weaken_Attack]);
                _enemy.ReceiveDFWeaken(cardState.buff[(int)BuffEnum.Bf_Weaken_Diffence]);
            }
        }

        //�o�t�Ȃ�
       int _progress = cardState.AP;//�R�X�g�����Ă���
        plState.Def.Value = Mathf.Clamp(cardState.buff[(int)BuffEnum.Bf_Diffence] > 0 ? cardState.buff[(int)BuffEnum.Bf_Diffence] + plState.Def_Never.Value : 0, 0, 999);
        plState.Atk.Value = cardState.buff[(int)BuffEnum.Bf_Attack];//�P�^�[���U������
        plState.Atk_Never.Value += cardState.buff[(int)BuffEnum.Bf_Attack_Never];//�i���U������
        plState.Def_Never.Value += cardState.buff[(int)BuffEnum.Bf_Diffence_Never];//�i���h�䋭��

        bandMana.playerHP.Value += cardState.buff[(int)BuffEnum.Bf_Heal];//�񕜏���
        if(bandMana.playerHP.Value> bandMana.playerMaxHP.Value)
        {
            bandMana.playerHP.Value = bandMana.playerMaxHP.Value;
        }
        // ���������[�h
        if (cardState.Reload > 0)
        {
            for (int i = cardState.Reload; i > 0; i--)
            {
                Reload();
            }
        }
        //�̂ĎD�ɃJ�[�h�����Ď�D����폜
        else
        {
            IsCardMoveNow = true;
            DOVirtual.DelayedCall(cockingMaxTime, () => { IsCardMoveNow = false; });
            //�擪�̃J�[�h���폜���ċ�̃J�[�h�𐶐�����
            cards[0].transform.DOMoveY(cardReloadPos.position.y, reloadMoveSpeed).OnComplete(() =>
            {
                //�J�[�h�{�̂̍폜
                Destroy(cards[0]);
                cards.RemoveAt(0);
                //�J�[�h�f�[�^�̍폜
                //�J�[�h����
                DOVirtual.DelayedCall(cardInstatiateDelay, () =>
                {
                    InstantiateGunInCard(5, true,false);
                });
                gunInCards.RemoveAt(0);
                for (int i = 0; i < 5; i++)
                {
                    cards[i].transform.DOMoveX(cardBasePos[i].position.x, reloadMoveSpeed);
                }
            });
        }
    }

    void CardEffect()
    {
        var _enemy = enemyObjects[targetId];
        var playerPos = player.transform.GetChild(0).position;
        var enemyPos = _enemy.transform.GetChild(0).position;
        //�G�t�F�N�g�̕\��
        //�G�̈ʒu�ɍU���G�t�F�N�g��\��
        if (gunInCards[0].state.Damage > 0)
        {
            Instantiate(cardEffect[0], enemyPos, Quaternion.identity);
        }
        //���g�̈ʒu�ɖh��G�t�F�N�g��\��
        if (gunInCards[0].state.buff[(int)BuffEnum.Bf_Diffence] > 0)
        {
            Instantiate(cardEffect[1], playerPos, Quaternion.identity);
        }
        //����G�t�F�N�g�̕\��
        if(gunInCards[0].state.buff[(int)BuffEnum.Bf_Diffence] <= 0 && gunInCards[0].state.Damage <= 0)
        {
            Instantiate(cardEffect[2], playerPos, Quaternion.identity);
        }

    }
    /// <summary>
    /// �U������
    /// </summary>
    /// <param name="_enemy"></param>
    public void Fire()
    {
        var _enemy = enemyObjects[targetId];
        //�J�[�h�̈ړ����͏��������Ȃ�
        if (IsCardMoveNow || playerTurn == false) { return; }
        IsCardMoveNow = true;
        DOVirtual.DelayedCall(fireMaxTime, () => { IsCardMoveNow = false; });

        //AudioSystem.AudioControl.Instance.SE.CardSePlayOneShot(gunInCards[0].state.SE);

        //�^�[���o�ߏ���
        ProgressTurn(gunInCards[0].state.AP);
        //�G�t�F�N�g�̕\�����s��
        CardEffect();
        //�J�[�h�̃V�X�e���������s��
        CardAction(_enemy);

        //�f�o�t����
        plState.ATWeaken.Value = Mathf.Clamp(plState.ATWeaken.Value - 1, 0, 9999);
        plState.DFWeaken.Value = Mathf.Clamp(plState.DFWeaken.Value - 1, 0, 9999);



        Player.instance.AttackAnim();

      

    }

    /// <summary>
    /// �^�[����i�s����
    /// </summary>
    /// <param name="_progressTurn">�o�߂���^�[��</param>
    public void ProgressTurn(int _progressTurn)
    {
        bool _flg = true;
        foreach (var _enemy in enemyObjects)
        {
            if (_enemy.ProgressTurn(_progressTurn))
            {
                _flg = playerTurn = false;
                actEnemy.Add(_enemy);
            }
        }
        // �G���s��������
        if (!_flg) actEnemy[0].AttackAnimation();
    }

    // �^�[�Q�b�g�Ďw��
    public void SetTarget(int _id)
    {
        targetId = _id;
    }
    #endregion

    #region Card

    /// <summary>
    /// �f�b�L���X�g����J�[�h���V���b�t�����ĎR�D���\�z����
    /// </summary>
    void ShuffleDeck()
    {
        //�f�b�L���X�g���V���b�t�����ăf�b�L�ɓ����
        deckInCards = DeckListManager.deckList.OrderBy(a => Guid.NewGuid()).ToList();
    }

    /// <summary>
    /// ���{���o�[�������[�h����
    /// </summary>
    /// <param name="bulletNum"></param>
    public void Reload(int bulletNum = 6)
    {
        //�����[�h���̓����[�h�ł��Ȃ��悤�ɂ���
        if (IsCardMoveNow || playerTurn == false) { return; }
        IsCardMoveNow = true;
        DOVirtual.DelayedCall(reloadMaxTime, () => { IsCardMoveNow = false; });
        //�R�D�̐����U���ȉ��̏ꍇ�̂ĎD���R�ɉ�����
        if (deckInCards.Count < 6)
        {
            foreach(var card in trashInCards)
            {
                deckInCards.Add(card);
            }
            trashInCards.Clear();

            ShuffleDeck();
        }
        //�J�[�h���ړ��シ�ׂč폜
        for (int i = 0; i < 6; i++)
        {
            var num = i;
            cards[num].transform.DOMoveY(cardReloadPos.position.y, reloadMoveSpeed).OnComplete(() =>
            {
                //�J�[�h�{�̂̍폜
                Destroy(cards[0]);
                cards.RemoveAt(0);
                //�̂ĎD�ɃJ�[�h�̃f�[�^������
                trashInCards.Add(gunInCards[0]);
                //�J�[�h�f�[�^�̍폜
                gunInCards.RemoveAt(0);
            });
        }
        //�J�[�h����
         DOVirtual.DelayedCall(cardInstatiateDelay, () => { 
            
            InstantiateGunInCards(); 
        
        });


        //AudioSystem.AudioControl.Instance.SE.PlaySeOneShot(SEList.Reload);

        ProgressTurn(reloadCost);
    }

    /// <summary>
    /// �R�b�L���O���ă��{���o�[���̒e�ۂ��P���炷
    /// </summary>
    public void Cocking()
    {
        if (IsCardMoveNow || playerTurn == false) { return; }
        IsCardMoveNow = true;
        DOVirtual.DelayedCall(cockingMaxTime, () => { IsCardMoveNow = false; });

        //�퓬�̃J�[�h���폜���čĐ�������
        cards[0].transform.DOMoveY(cardReloadPos.position.y, reloadMoveSpeed).OnComplete(() =>
        {
            //�J�[�h�{�̂̍폜
            Destroy(cards[0]);
            cards.RemoveAt(0);
            //�J�[�h�f�[�^�̍폜
            //�J�[�h����
            DOVirtual.DelayedCall(cardInstatiateDelay, () =>
            {
                InstantiateGunInCard(5, gunInCards[0]);
            });
            gunInCards.RemoveAt(0);
            for (int i = 0; i < 5; i++)
            {
                cards[i].transform.DOMoveX(cardBasePos[i].position.x, reloadMoveSpeed);
            }
        });

        AudioSystem.AudioControl.Instance.SE.PlaySeOneShot(SEList.Cocking);

        ProgressTurn(cockingCost);
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
            var enemy = Instantiate(enemyPrefab, enemyAddventPos.position, Quaternion.identity);
            enemy.transform.DOMove(enemyStartPos[enemyCount].position, enemyAddventMoveTime);//�o�ꎞ�̈ړ�����
            var script = enemy.GetComponent<Enemy.Enemy>();
            script.Initialize(enemysState[enemyCount]);
            script.fieldID = enemyCount;//�^�[�Q�b�g�w��p��ID����

            enemyObjects.Add(script);//�G�l�~�[�I�u�W�F�N�g��ۑ�

            enemyCount++;
        }

    }

    #endregion


    #region Debug
    public void AllDeathEnemys()
    {
        foreach (var _eObj in enemyObjects)
        {
            _eObj.ReceiveDamage(99999);
        }
    }

    #endregion


}