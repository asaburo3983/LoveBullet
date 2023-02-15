using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class BandManager : SingletonMonoBehaviour<BandManager>
{
    [SerializeField] Canvas optionCanvas;
    [SerializeField] Canvas deckListCanvas;

    public ReactiveProperty<int> playerHP = new ReactiveProperty<int>();
    public ReactiveProperty<int> unstable = new ReactiveProperty<int>();
    public ReactiveProperty<int> feelRed = new ReactiveProperty<int>();
    public ReactiveProperty<int> feelBlue = new ReactiveProperty<int>();
    public ReactiveProperty<int> feelGreen = new ReactiveProperty<int>();

    private void Awake()
    {
        SingletonCheck(this, true);
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }


}
