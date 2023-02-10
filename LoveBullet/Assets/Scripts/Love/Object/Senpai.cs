using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Senpai : MonoBehaviour
{
    protected ReactiveProperty<bool> touch = new ReactiveProperty<bool>();
    void Start()
    {
        //ノベルモードのときはスクリプトを作動しないようにする
        if (NovelManager.instance.novelMode == NovelManager.NovelMode.Novel)
        {
            Destroy(gameObject);
        }
        touch.Value = false;
        //プレイヤーのUI切り替え
        touch.Subscribe(x => { ActiveUI(x); }).AddTo(this);
    }
    virtual protected void Update()
    {
        if (touch.Value == true && InputSystem.instance.WasPressThisFlame("Player", "Fire"))
        {
            touch.Value = false;
            //演出終了後シーン移動を行う
        }
    }
    protected void ActiveUI(bool enable)
    {
        Love.PlayerLove.instance.fightAccessUI.SetActive(enable);
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
