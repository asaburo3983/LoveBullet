
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
        [System.Serializable]
        public class State
        {
            public int id;
            public GENRE genre;
            public TYPE type;
            public string name;
            public string explanation;
            public int AP;
            public int AT;
            public int DF;
            public int ATWeaken;
            public int DFWeaken;
            public List<int> value = new List<int>();
        }
        State state;

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
        [SerializeField] UI ui;



        /// <summary>
        /// ステータス値を代入して
        /// UIなどにデータを代入する
        /// </summary>
        /// <param name="_state"></param>
        public void Initialize(State _state)
        {
            state = _state;

            //とりあえずテキスト代入
            Rough.SetText(ui.name, state.name);
            Rough.SetText(ui.explanation,state.explanation);
            Rough.SetText(ui.AP, state.AP);
            Rough.SetText(ui.AT, state.AT);
            Rough.SetText(ui.DF, state.DF);
            //Rough.SetText(ui.ATWeaken, state.ATWeaken);
            //Rough.SetText(ui.DFWeaken, state.DFWeaken);

        }

    }
}