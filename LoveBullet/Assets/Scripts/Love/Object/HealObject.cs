using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class HealObject : BaseDropObject
{
    [SerializeField]float healPer;
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
        var bandManager = BandManager.instance;
        //HP�̉񕜏������L�q
        bandManager.HealHP((int)(bandManager.playerMaxHP.Value * healPer / 100.0f));

        //�G�t�F�N�g�Ɖ���炷
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
