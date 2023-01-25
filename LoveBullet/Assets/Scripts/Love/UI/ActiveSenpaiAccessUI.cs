using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class ActiveSenpaiAccessUI : MonoBehaviour
{
    Image im;
    Button bt;
    // Start is called before the first frame update
    void Start()
    {
        im = GetComponent<Image>();
        bt = GetComponent<Button>();

        Love.Player_Love.instance.isUISenpaiActive.Subscribe(x => { SetActive(x); }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        im.enabled= Love.Player_Love.instance.isUISenpaiActive.Value;
        bt.enabled = Love.Player_Love.instance.isUISenpaiActive.Value;
    }
    void SetActive(bool ac)
    {
        im.enabled = ac;
        bt.enabled = ac;
    }
}
