using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class DebuffDrawDisable_Player : MonoBehaviour
{
    [SerializeField] BufferType buffType;
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;
        SetDraw(0);
        switch (buffType)
        {
            case BufferType.Atk_Weak:
                player.gameState.ATWeaken.Subscribe(x =>
                {
                    SetDraw(x);
                }).AddTo(this);
                break;
            case BufferType.Def_Weak:
                player.gameState.DFWeaken.Subscribe(x =>
                {
                    SetDraw(x);
                }).AddTo(this);
                break;
            case BufferType.Armor:
                player.gameState.Def.Subscribe(x =>
                {
                    SetDraw(x);
                }).AddTo(this);
                break;
            default:
                break;

        }

    }
    void SetDraw(int x)
    {
        var b = false;
        if (x > 0) { b = true; }
        GetComponent<SpriteRenderer>().enabled = b;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
