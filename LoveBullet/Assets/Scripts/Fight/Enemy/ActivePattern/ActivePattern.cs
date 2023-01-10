using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public class ActivePattern
    {
        public string explanation;
        public int AT;
        public int DF;
        public int ATWeaken;
        public int DFWeaken;
        public int Turn;
        public int Type;
        public int Fluctuation;
        public List<int> value=new List<int>();
    }
}