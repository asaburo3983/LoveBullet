using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class LovePointManager : SingletonMonoBehaviour<LovePointManager>
{
    public enum LovePointType
    {
        Red = 0,
        Green,
        Blue,

        Max
    }

    [SerializeField] List<IntReactiveProperty> lovePoint;

    public IObservable<int> LovePointChanged(LovePointType _type)
    {
        return lovePoint[(int)_type];
    }

    // �l�̎擾
    public int LovePointR => lovePoint[(int)LovePointType.Red].Value;
    public int LovePointG => lovePoint[(int)LovePointType.Green].Value;
    public int LovePointB => lovePoint[(int)LovePointType.Blue].Value;

    // �|�C���g�̎g�p
    public bool UseLovePoint(LovePointType _type,int _useValue)
    {
        if (_useValue < lovePoint[(int)_type].Value) return false;

        lovePoint[(int)_type].Value -= _useValue;
        return true;
    }

    // �|�C���g�̉��Z
    public void AddLovePoint(LovePointType _type,int _addValue)
    {
        lovePoint[(int)_type].Value += _addValue;
    }

    private void Awake()
    {
        if (SingletonCheck(this, true))
        {

        }
    }   
}
