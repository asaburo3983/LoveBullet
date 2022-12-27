using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TextInLovePointMulti : MonoBehaviour
{

    Text text;
    ResultManager result;
    int basePoint;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        result = ResultManager.instance;
        basePoint = result.getLovePointMulti.Value;
        result.getLovePointMulti.Subscribe(x => { SetText(); }).AddTo(this);
    }

    void SetText()
    {
        text.text = result.getLovePointMulti.Value.ToString()+"/"+ basePoint.ToString();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
