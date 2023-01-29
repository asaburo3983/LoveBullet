
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    public class Card :MonoBehaviour
    {
        public enum GENRE
        {
            Red = 0,
            Max
        }
        public enum TYPE
        {
            Attack,
            Deffense,

            Max
        }
        public enum ATTACK_TYPE
        {
            solo,
            all
        }

        [System.Serializable]
        public class State
        {
            public int id;
            public GENRE genre;
            public TYPE type;
            public string name;
            public int rank;
            public string explanation;

            public int AP;
            public int Damage;

            public Buff buff = new Buff();

            public int Cocking;     // �R�b�L���O��
            public int Reload;      // ���������[�h��
            public int SelfDamage;  // ����
            public int MultiAttack; // �U����
            public bool Scrap;      // �p��
            public bool Whole;      // �S�̍U��
            public bool Random;     // �����_���U��

            public int number;
            public int SE;
            public int Effect;
            public List<int> value = new List<int>();
        }
        protected State state;

        [System.Serializable]
        public struct UI
        {
            public GameObject name;
            public GameObject explanation;
            public GameObject AP;
            public GameObject AT;
            public GameObject DF;
            public GameObject ATWeaken;
            public GameObject DFWeaken;
        }
        public UI ui;



        /// <summary>
        /// �X�e�[�^�X�l��������
        /// UI�ȂǂɃf�[�^��������
        /// </summary>
        /// <param name="_state"></param>
        public void Initialize(State _state)
        {
            state = _state;

            //�Ƃ肠�����e�L�X�g���
            Rough.SetText(ui.name, state.name);
            Rough.SetText(ui.explanation,state.explanation);
            Rough.SetText(ui.AP, state.AP);
            Rough.SetText(ui.AT, state.Damage);
            Rough.SetText(ui.DF, state.buff.DF);
            
            //Rough.SetText(ui.ATWeaken, state.ATWeaken);
            //Rough.SetText(ui.DFWeaken, state.DFWeaken);

        }

    }
}