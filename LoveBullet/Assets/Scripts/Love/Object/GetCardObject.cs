using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class GetCardObject : Object
{
    [SerializeField]int rank;
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
        //�����Ă����ꂽ�����N���烉���_���ɃJ�[�h���f�b�L�ɓ���鏈���ǉ�
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
