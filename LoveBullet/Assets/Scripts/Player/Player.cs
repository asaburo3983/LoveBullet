using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Player : SingletonMonoBehaviour<Player>
{
    [System.Serializable]
    public struct ReductionRate
    {
        [Header("0% ~ 100%"),Header("�f�o�t���̔{��")]
        [Range(0, 100)] public int AT;
        [Header("100% ~ 200%"), Range(100, 200)] public int DF;
    }
    [SerializeField] ReductionRate rate;
    public ReductionRate Rate => rate;

    [System.Serializable]
    public struct InGameState
    {
        public IntReactiveProperty hp;
        public IntReactiveProperty DF;
        public IntReactiveProperty ATWeaken;
        public IntReactiveProperty DFWeaken;

        public int reloadAP;
        public int cockingAP;

        public int freeCocking;
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
    /// DF�Ǘ�
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
    /// �v���C���[�Ƀ_���[�W��^���鏈��
    /// </summary>
    /// <param name="_damage"></param>
    public static void ReceiveDamage(int _damage)
    {
        var state = instance.gameState;
        state.hp.Value -= (_damage - state.DF.Value);

        if (state.hp.Value <= 0)
        {
            //���S���� TODO
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
