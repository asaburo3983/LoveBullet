using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace FightPlayer
{
    public class HPText : MonoBehaviour
    {
        BandManager bandMana;
        [SerializeField] Text hpText;

        // Start is called before the first frame update
        void Start()
        {
            bandMana = BandManager.instance;
            bandMana.playerHP.Subscribe(x =>
            {
                hpText.text = x.ToString() + "/" + bandMana.playerMaxHP.Value.ToString();
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}