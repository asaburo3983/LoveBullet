using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class TextInLovePointRGB : MonoBehaviour
{

    [SerializeField] ReactiveProperty<int> addPointR = new ReactiveProperty<int>();
    [SerializeField] ReactiveProperty<int> addPointB= new ReactiveProperty<int>();
    [SerializeField] ReactiveProperty<int> addPointG = new ReactiveProperty<int>();

    LovePointManager mana;
    ResultManager result;
    int basePointR;
    int basePointB;
    int basePointG;

    [SerializeField] Text pointR;
    [SerializeField] Text pointB;
    [SerializeField] Text pointG;
    
    [SerializeField] Text addPointRT;
    [SerializeField] Text addPointBT;
    [SerializeField] Text addPointGT;

    // Start is called before the first frame update
    void Start()
    {
        mana = LovePointManager.instance;
        result = ResultManager.instance;

        basePointR = mana.lovePointR.Value;
        basePointG = mana.lovePointG.Value;
        basePointB = mana.lovePointB.Value;

        mana.lovePointR.Subscribe(x => { SetText(x, basePointR, pointR); }).AddTo(this); ;
        mana.lovePointG.Subscribe(x => { SetText(x, basePointG, pointG); }).AddTo(this); ;
        mana.lovePointB.Subscribe(x => { SetText(x, basePointB, pointB); }).AddTo(this); ;

        addPointR.Subscribe(x => { SetAddText(x, addPointRT); }).AddTo(this);
        addPointG.Subscribe(x => { SetAddText(x, addPointGT); }).AddTo(this);
        addPointB.Subscribe(x => { SetAddText(x, addPointBT); }).AddTo(this);
    }

    void SetText(int _point,int _pointBase,Text _Text)
    {
        _Text.text = _point.ToString();
    }
    void SetAddText(int _point,Text _Text)
    {
        _Text.text = "+" + _point.ToString();
    }

    public void RandomAdd()
    {
        var r = result.getLovePointMulti.Value / 3;
        var g = result.getLovePointMulti.Value / 3;
        var b = result.getLovePointMulti.Value / 3;
        
        AddLovePointMulti(-r);
        mana.lovePointR.Value+=r;
        addPointR.Value+=r;
        AddLovePointMulti(-g);
        mana.lovePointG.Value+=g;
        addPointG.Value+=g;
        AddLovePointMulti(-b);
        mana.lovePointB.Value+=b;
        addPointB.Value+=b;
    }
    bool AddLovePointMulti(int num)
    {
        if (num < 0)
        {
            if (result.getLovePointMulti.Value == 0) { return false; }
        }
        result.getLovePointMulti.Value += num;
        return true;
    }
    public void UpPointR()
    {
        if (!AddLovePointMulti(-1)) { return; }
        mana.lovePointR.Value++;
        addPointR.Value++;
    }
    public void UpPointB()
    {
        if (!AddLovePointMulti(-1)) { return; }
        mana.lovePointB.Value++;
        addPointB.Value++;
    }
    public void UpPointG()
    {
        if (!AddLovePointMulti(-1)) { return; }
        mana.lovePointG.Value++;
        addPointG.Value++;
    }
    public void DownPointR()
    {
        if (addPointR.Value == 0) { return; }
        AddLovePointMulti(1);
        mana.lovePointR.Value--;
        addPointR.Value--;
    }
    public void DownPointB()
    {
        if (addPointB.Value == 0) { return; }
        AddLovePointMulti(1);
        mana.lovePointB.Value--;
        addPointB.Value--;
    }
    public void DownPointG()
    {
        if (addPointG.Value == 0) { return; }
        AddLovePointMulti(1);
        mana.lovePointG.Value--;
        addPointG.Value--;
    }

}
