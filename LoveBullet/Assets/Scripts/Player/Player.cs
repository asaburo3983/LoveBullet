
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Player : SingletonMonoBehaviour<Player>
{
    [System.Serializable]
    public struct InGameState
    {
        public ReactiveProperty<int> hp;
        public ReactiveProperty<int> AP;
        public ReactiveProperty<int> APMax;
        public ReactiveProperty<int> DF;
        public ReactiveProperty<int> ATWeaken;
        public ReactiveProperty<int> DFWeaken;

        public int reloadAP;
        public int cockingAP;

    }
    public InGameState gameState = new InGameState();
    [System.Serializable]
    public struct UI
    {
        public GameObject hp;
        public GameObject AP;
        public GameObject DF;
        public GameObject ATWeaken;
        public GameObject DFWeaken;
    }
    [SerializeField] UI ui;

    /// <summary>
    /// APÇç≈ëÂílÇ…ñﬂÇ∑
    /// </summary>
    public static void ResetAP()
    {
        instance.gameState.AP = instance.gameState.APMax;
    }
    private void Awake()
    {
        SingletonCheck(this);

    }
    // Start is called before the first frame update
    void Start()
    {
        SingletonCheck(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
