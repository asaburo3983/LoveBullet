using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        struct InGameState
        {
            int turn;

            int hp;
            int DF;
            int ATWeaken;
            int DFWeaken;
        }
        InGameState gameState;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
