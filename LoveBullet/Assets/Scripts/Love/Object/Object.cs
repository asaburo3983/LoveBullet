using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Object : MonoBehaviour
{
    protected ReactiveProperty<bool> touch = new ReactiveProperty<bool>();
    virtual protected void Start()
    {
        touch.Value = false;
        //プレイヤーのUI切り替え
        touch.Subscribe(x => { ActiveUI(x); }).AddTo(this);
    }
    virtual protected void Update()
    {
        if (touch.Value==true&&InputSystem.instance.WasPressThisFlame("Player", "Fire"))
        {
            touch.Value = false;
            Destroy(this.gameObject);
        }
    }
    protected void ActiveUI(bool enable)
    {
        Love.PlayerLove.instance.objectAccessUI.SetActive(enable);
    }



}
