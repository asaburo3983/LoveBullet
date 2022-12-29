using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    { 
        public class State
        {
            public int number;
            public int type;
            public int strengeth;

            public string name;
            public string explanation;
            public int hpMax;
            public int hpFluctuationPlus;
            public int hpFluctuationMinus;

            public List<int> pattern = new List<int>();
            public List<int> value = new List<int>();
        }
        State state;

        [System.Serializable]
        public struct InGameState
        {
            public IntReactiveProperty turn;
            public IntReactiveProperty hp;
            public IntReactiveProperty DF;
            public IntReactiveProperty ATWeaken;
            public IntReactiveProperty DFWeaken;
            public int stan;
            [ReadOnly]public int currentIdx;
        }
        public InGameState gameState;
        [System.Serializable]
        public struct UI
        {
            public GameObject turn;
            public GameObject name;
            public GameObject hp;
            public GameObject DF;
            public GameObject ATWeaken;
            public GameObject DFWeaken;
        }
        [SerializeField] UI ui;

        [SerializeField] SpriteRenderer spriteRendere;

        private void Start()
        {
            //���S����
            gameState.hp.Where(x => x <= 0).Subscribe(x => {

                var fight = Card.Fight.instance;
                int _id = fight.enemyObjects.IndexOf(this);
                
                // �폜�ɔ����^�[�Q�b�g��ID��ύX����
                if(fight.TargetId == _id) {

                    if(fight.enemyObjects.Count - 1 == _id) {
                        fight.SetTarget(0);
                    }
                }
                else if(fight.TargetId > _id) {
                    fight.SetTarget(fight.TargetId - 1);
                }

                Destroy(gameObject);
            }).AddTo(this);

            // �G�l�~�[�s������
            gameState.turn.Pairwise()
                .Where(x => x.Previous > 0)
                .Where(x => x.Current <= 0)
                .Subscribe(x => {
                    Action();
                }).AddTo(this);

            // �����^�[�����ݒ�
            gameState.turn.Value = CacheData.instance.enemyActivePattern[state.pattern[gameState.currentIdx]].Turn;
        }
        //TODo ���Ńe�N�X�`����ݒ肷��
        void SetTexture()
        {
            //���\�[�X�̂��牼�e�N�X�`����ǂݍ���œ����
            var path= "Texture/Fight/Enemy/Enemy"+ state.number.ToString();
            spriteRendere.sprite = Resources.Load<Sprite>(path);

            GetComponent<BoxCollider2D>().size = spriteRendere.bounds.size;

        }
        /// <summary>
        /// �X�e�[�^�X�l��������
        /// UI�ȂǂɃf�[�^��������
        /// </summary>
        /// <param name="_state"></param>
        public void Initialize(State _state)
        {
            state = _state;
            gameState.turn.Value = 0;
            gameState.hp.Value = state.hpMax;
            gameState.DF.Value = 0;
            gameState.ATWeaken.Value = 0;
            gameState.DFWeaken.Value = 0;

            //�Ƃ肠�����e�L�X�g���
            Rough.SetText(ui.turn, gameState.turn.Value);
            Rough.SetText(ui.name, state.name);
            Rough.SetText(ui.hp, gameState.hp.Value);
            Rough.SetText(ui.DF, gameState.DF.Value);
            Rough.SetText(ui.ATWeaken, gameState.ATWeaken.Value);
            Rough.SetText(ui.DFWeaken, gameState.DFWeaken.Value);

            //�G�l�~�[�ԍ��ɑΉ������O���t�B�b�N������
            SetTexture();
        }

        public static State GetEnemyState(int _id)
        {
            if (_id < 0 || _id >= CacheData.instance.enemyStates.Count)
            {
                Debug.LogError("�f�[�^�O���Q�Ƃ��悤�Ƃ��Ă��܂�");
            }
           return CacheData.instance.enemyStates[_id];
        }

        /// <summary>
        /// �G���s������
        /// </summary>
        public void Action()
        {
            var playerState = Player.instance.gameState;
            var activeId = state.pattern[gameState.currentIdx];
            var actiovePattern = CacheData.instance.enemyActivePattern[activeId];

            //TODO �Ƃ肠�����U���Ɩh�䏈�������쐬
            Player.ReceiveDamage(actiovePattern.AT);//�U��
            gameState.DF.Value += actiovePattern.DF;//�h��

            // �s����������炷
            gameState.currentIdx++;
            gameState.currentIdx %= state.pattern.Count;

            // �s���܂ł̃^�[���ݒ�
            var act = CacheData.instance.enemyActivePattern[gameState.currentIdx];
            gameState.turn.Value = act.Turn + Random.Range(0,act.Fluctuation);

        }
        public void ReceiveDamage(int _damage)
        {

            // �h��o�t�v�Z
            if(_damage< gameState.DF.Value) {
                gameState.DF.Value -= _damage;
                return;
            }
            else {
                _damage -= gameState.DF.Value;
                gameState.DF.Value = 0;
            }
            gameState.hp.Value -= (_damage - gameState.DF.Value);
        }

        public void ReceiveStan(int _stan)
        {
            gameState.stan += _stan;
        }

        public void ReceiveATWeaken(int _weak)
        {
            gameState.ATWeaken.Value += _weak;
        }

        public void ReceiveDFWeaken(int _weak)
        {
            gameState.DFWeaken.Value += _weak;
        }

        public void ProgressTurn(int _progressTurn)
        {
            if (gameState.stan > 0) {
                gameState.stan--;
            }
            else {
                gameState.turn.Value -= _progressTurn;
            }
        }

        public void ResetDF()
        {
            gameState.DF.Value = 0;
        }

        public void SetTarget()
        {
            Card.Fight.instance.SetTarget(this);
        }

        private void OnDestroy()
        {
            Card.Fight.instance.enemyObjects.Remove(this);
        }
    }
}
