using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class ActiveSenpaiAccessUI : MonoBehaviour
{
    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        Love.PlayerLove.instance.isUISenpaiActive.Subscribe(x => { SetActive(x); }).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
    }
    void SetActive(bool ac)
    {
        sr.enabled = ac;
    }
}
