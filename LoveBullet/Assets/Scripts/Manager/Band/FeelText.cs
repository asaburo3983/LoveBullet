using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

    public class FeelText : MonoBehaviour
    {
    enum FeelMode
    {
        unstable,
        red,blue,green
    }
        BandManager bandMana;
    [SerializeField] FeelMode mode;
    [SerializeField] Text text;

    // Start is called before the first frame update
    void Start()
    {
        bandMana = BandManager.instance;
        switch (mode)
        {
            case FeelMode.unstable:
                bandMana.unstable.Subscribe(x =>
                {
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            case FeelMode.red:

                bandMana.feelRed.Subscribe(x =>
                {
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            case FeelMode.blue:

                bandMana.feelBlue.Subscribe(x =>
                {
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            case FeelMode.green:

                bandMana.feelGreen.Subscribe(x =>
                {
                    text.text = x.ToString();
                }).AddTo(this);
                break;
        }

    }

        // Update is called once per frame
        void Update()
        {

        }
    }