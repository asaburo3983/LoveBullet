
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            public string name;
            public int id;
            public GENRE genre;
            public TYPE type;
            public int rank;
            public string explanation;

            public int AP;
            public int Damage;

            public int[] buff = new int[(int)BuffEnum.Max];

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
        public State STATE => state;

        [System.Serializable]
        public struct UI
        {
            public GameObject name;
            public GameObject explanation;
            public GameObject AP;
            public GameObject AT;
            public GameObject DF;
        }
        public UI ui;

        [SerializeField] Image frame;
        [SerializeField] Image illust;

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
            Rough.SetText(ui.DF, state.buff[(int)BuffEnum.Bf_Diffence]);

            //Rough.SetText(ui.ATWeaken, state.ATWeaken);
            //Rough.SetText(ui.DFWeaken, state.DFWeaken);


            SetTexture();
        }
        void SetTexture()
        {
            string framePath = "none";
            if (state.Damage > 0)
            {
                framePath = "Texture/Fight/UI/Card/Frame/" + "CardFrame_Attack";

            }
            else if (state.buff[(int)BuffEnum.Bf_Diffence] > 0)
            {
                framePath = "Texture/Fight/UI/Card/Frame/" + "CardFrame_Diffence";
            }
            else
            {
                framePath = "Texture/Fight/UI/Card/Frame/" + "CardFrame_Special";
            }
            frame.sprite = Resources.Load<Sprite>(framePath);

            //���\�[�X�̂��牼�e�N�X�`����ǂݍ���œ����
            var path = "Texture/Fight/UI/Card/Illust/" + state.id.ToString();
            var tex = Resources.Load<Sprite>(path);
            if (tex != null)
            {
                illust.sprite = Resources.Load<Sprite>(path);
            }

        }
    }
}