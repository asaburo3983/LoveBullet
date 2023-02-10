using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class SHealObject : ShopObject
{
    [SerializeField] float healPer;
    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }
    override protected void Update()
    {
        base.Update();
    }
    private void OnDestroy()
    {
        //HP‚Ì‰ñ•œˆ—‚ğ‹Lq
        //var pGameState = Player.instance.gameState;
        //var heal = pGameState.maxHP.Value * healPer / 100.0f;
        //Player.instance.gameState.hp.Value += (int)heal;

    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            touch.Value = true;
        }
    }
    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            touch.Value = false;
        }
    }
}
