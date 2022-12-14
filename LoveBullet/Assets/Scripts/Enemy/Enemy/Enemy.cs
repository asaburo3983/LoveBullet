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
            public int type;
            public int strengeth;

            public string name;
            public string explanation;
            public int hpMax;
            public int hpFluctuationPlus;
            public int hpFluctuationMinus;

            public int ATFluctuationPlus;
            public int ATFluctuationMinus;
            public List<int> pattern = new List<int>();
            public List<int> value = new List<int>();
        }
        State state;

        [System.Serializable]
        public struct InGameState
        {
            public ReactiveProperty<int> turn;
            public ReactiveProperty<int> hp;
            public ReactiveProperty<int> DF;
            public ReactiveProperty<int> ATWeaken;
            public ReactiveProperty<int> DFWeaken;
            public ReactiveProperty<bool> acted;
        }
        [SerializeField]InGameState gameState;
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
            gameState.acted.Value = false;

            //�Ƃ肠�����e�L�X�g���
            Rough.SetText(ui.turn, gameState.turn.Value);
            Rough.SetText(ui.name, state.name);
            Rough.SetText(ui.hp, gameState.hp.Value);
            Rough.SetText(ui.DF, gameState.DF.Value);
            Rough.SetText(ui.ATWeaken, gameState.ATWeaken.Value);
            Rough.SetText(ui.DFWeaken, gameState.DFWeaken.Value);

            //�G�l�~�[�ԍ��ɑΉ������O���t�B�b�N������
        }


        public static State GetEnemyState(int _id)
        {
            if (_id < 0 || _id >= CacheData.instance.enemyStates.Count)
            {
                Debug.LogError("�f�[�^�O���Q�Ƃ��悤�Ƃ��Ă��܂�");
            }
           return CacheData.instance.enemyStates[_id];
        }
    }
}
