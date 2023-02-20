using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindHealObject : BaseDropObject
{
    [SerializeField] int heal;
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
        //•sˆÀ’è“x‰ñ•œˆ—
        BandManager.instance.HealMind(heal);
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
