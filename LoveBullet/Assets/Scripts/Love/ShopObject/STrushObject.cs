using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class STrushObject : ShopObject
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
        //�J�[�h�p���������쐬

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