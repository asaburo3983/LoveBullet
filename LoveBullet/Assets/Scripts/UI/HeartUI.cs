using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class HeartUI : MonoBehaviour
{
    [SerializeField] List<Text> value;

    [SerializeField] float scaleSize = 1.2f;
    [SerializeField] float scaleTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {

        for(int i = 0; i < (int)LovePointManager.LovePointType.Max; i++) {
            LovePointManager.instance.LovePointChanged((LovePointManager.LovePointType)i).Subscribe(x => {
                value[i].text = x.ToString();
                value[i].transform.parent.DOScale(scaleSize, scaleTime).SetLoops(2, LoopType.Yoyo);
            }).AddTo(this);
        }
    }
}
