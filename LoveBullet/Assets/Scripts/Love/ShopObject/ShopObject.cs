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
    BandManager bandMana;
    virtual protected void Start()
    {
        bandMana = BandManager.instance;
        touch.Value = false;
        //�v���C���[��UI�؂�ւ�
        touch.Subscribe(x => { ActiveUI(x); }).AddTo(this);
    }
    virtual protected void Update()
    {

        if (touch.Value == true && InputSystem.instance.WasPressThisFlame("Player", "Fire")&&IsGetItem())
        {
            touch.Value = false;
            Destroy(this.gameObject);
        }
    }
    /// <summary>
    /// �A�C�e�����D���x�Ŏ擾�ł��邩�̊m�F�Ƃł���ꍇ�͍D���x������������
    /// </summary>
    /// <returns></returns>
    protected bool IsGetItem()
    {
        switch (favorability)
        {
            case Favorability.Red:
                if (amount <= bandMana.feelRed.Value)
                {
                    bandMana.feelRed.Value -= amount;
                    return true;
                }
                return false;
                break;
            case Favorability.Blue:
                if (amount <= bandMana.feelBlue.Value)
                {
                    bandMana.feelBlue.Value -= amount;
                    return true;
                }
                return false;
                break;
            case Favorability.Green:
                if (amount <= bandMana.feelGreen.Value)
                {
                    bandMana.feelGreen.Value -= amount;
                    return true;
                }
                return false;
                break;
        }

        return false;
    }
    protected void ActiveUI(bool enable)
    {
        Love.PlayerLove.instance.shopAccessUI.SetActive(enable);
    }



}
