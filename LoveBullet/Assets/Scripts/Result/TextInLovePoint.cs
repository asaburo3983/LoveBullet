using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class TextInLovePoint : MonoBehaviour
{
    [SerializeField] string headText = "Žæ“¾Loveƒ|ƒCƒ“ƒg ";
    Text text;
    ResultManager result;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        result = ResultManager.instance;
        result.getLovePoint.Subscribe(x => { SetText(); }).AddTo(this);
    }

    void SetText()
    {
        text.text = headText + result.getLovePoint.Value.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
