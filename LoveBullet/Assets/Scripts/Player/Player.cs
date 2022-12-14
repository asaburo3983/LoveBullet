
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
    /// APを最大値に戻す
    /// </summary>
    public static void ResetAP()
    {
        instance.gameState.AP.Value = instance.gameState.APMax.Value;
    }
    public static void PlusAP(int _AP)
    {
        instance.gameState.AP.Value += _AP;
    }
    public static void MinusAP(int _AP)
    {
        instance.gameState.AP.Value -= _AP;
    }
    /// <summary>
    /// DF管理
    /// </summary>
    public static void ResetDF()
    {
        instance.gameState.DF.Value = 0;
    }
    public static void PlusDF(int _value)
    {
        instance.gameState.DF.Value += _value;
    }
    public static void MinusDF(int _value)
    {
        instance.gameState.DF.Value -= _value;
    }
    /// <summary>
    /// プレイヤーにダメージを与える処理
    /// </summary>
    /// <param name="_damage"></param>
    public static void ReceiveDamage(int _damage)
    {
        var state = instance.gameState;
        state.hp.Value -= (_damage - state.DF.Value);

        if (state.hp.Value <= 0)
        {
            //死亡処理 TODO
        }
    }

    private void Awake()
    {
        SingletonCheck(this);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
