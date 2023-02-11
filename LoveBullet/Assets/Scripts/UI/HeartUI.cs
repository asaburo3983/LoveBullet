using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class HeartUI : MonoBehaviour
{
    [SerializeField] Text red;
    [SerializeField] Text green;
    [SerializeField] Text blue;

    [SerializeField] float scaleSize = 1.2f;
    [SerializeField] float scaleTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        LovePointManager.instance.lovePointR.Subscribe(value => {
            red.text = value.ToString();
            red.transform.parent.DOScale(scaleSize, scaleTime).SetLoops(2, LoopType.Yoyo);
        }).AddTo(this);
        LovePointManager.instance.lovePointG.Subscribe(value => {
            green.text = value.ToString();
            green.transform.parent.DOScale(scaleSize, scaleTime).SetLoops(2, LoopType.Yoyo);
        }).AddTo(this);
        LovePointManager.instance.lovePointB.Subscribe(value => {
            blue.text = value.ToString();
            blue.transform.parent.DOScale(scaleSize, scaleTime).SetLoops(2, LoopType.Yoyo);
        }).AddTo(this);
    }
}
