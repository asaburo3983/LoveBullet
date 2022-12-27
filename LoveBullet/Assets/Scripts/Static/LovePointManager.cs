using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class LovePointManager : SingletonMonoBehaviour<LovePointManager>
{

    public ReactiveProperty<int> lovePointR = new ReactiveProperty<int>();
    public ReactiveProperty<int> lovePointG = new ReactiveProperty<int>();
    public ReactiveProperty<int> lovePointB = new ReactiveProperty<int>();

    private void Awake()
    {
        if (SingletonCheck(this, true))
        {

        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
