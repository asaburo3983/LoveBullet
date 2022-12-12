
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
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

    public class State
    {
        public GENRE genre;
        public TYPE type;
        public string name;
        public string explanation;
        public int AP;
        public int AT;
        public int DF;
        public int ATWeaken;
        public int DFWeaken;
        public List<int> value=new List<int>();
    }
}