using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public class ActivePattern
    {
        public string name;
        public string explanation;

        public int Damage;
        public Buff buff = new Buff();
        
        public int Turn;
        public int Type;
        public int Fluctuation;
        public int SE;
        public int Effect;
        public List<int> value=new List<int>();
    }
}