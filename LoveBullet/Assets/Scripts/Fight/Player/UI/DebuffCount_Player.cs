using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[System.Serializable]


public class DebuffCount_Player : MonoBehaviour
{
    [SerializeField] BufferType buffType;
    Player player;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;

        text = GetComponent<Text>();
        switch (buffType)
        {
            case BufferType.Atk_Weak:
                player.gameState.ATWeaken.Subscribe(x =>
                {
                    SetText(x.ToString());
                }).AddTo(this);
                break;
            case BufferType.Def_Weak:
                player.gameState.DFWeaken.Subscribe(x =>
                {
                    SetText(x.ToString());
                }).AddTo(this);
                break;
            case BufferType.Armor:
                player.gameState.Def.Subscribe(x =>
                {
                    SetText(x.ToString());
                }).AddTo(this);
                break;
            default:
                break;
        }
    }
    void SetText(string x)
    {
        if (x == "0") { x = null; }//0‚Ì‚Æ‚«‚Í•\Ž¦‚µ‚È‚¢
        text.text = x;
    }
    // Update is called once per frame
    void Update()
    {
    }
}