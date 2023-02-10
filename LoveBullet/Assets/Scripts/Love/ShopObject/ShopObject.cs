using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ShopObject : MonoBehaviour
{
    protected enum Favorability
    {
        Red,
        Blue,
        Green,
        MAX
    }
    protected ReactiveProperty<bool> touch = new ReactiveProperty<bool>();
    [SerializeField] protected Favorability favorability;
    [SerializeField] protected int amount;
    virtual protected void Start()
    {
        touch.Value = false;
        //ƒvƒŒƒCƒ„[‚ÌUIØ‚è‘Ö‚¦
        touch.Subscribe(x => { ActiveUI(x); }).AddTo(this);
    }
    virtual protected void Update()
    {
        if (touch.Value == true && InputSystem.instance.WasPressThisFlame("Player", "Fire"))
        {
            touch.Value = false;
            Destroy(this.gameObject);
        }
    }
    protected void ActiveUI(bool enable)
    {
        Love.PlayerLove.instance.shopAccessUI.SetActive(enable);
    }



}
