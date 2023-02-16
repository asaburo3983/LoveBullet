using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace FightPlayer
{
    public class HPBar : MonoBehaviour
    {
        BandManager bandMana;
        [SerializeField] SpriteRendererFillAmount srfa;
        // Start is called before the first frame update
        void Start()
        {
            bandMana = BandManager.instance;
            bandMana.playerHP.Subscribe(x =>
            {
                srfa.FillAmount = ((float)x / (float)bandMana.playerMaxHP.Value);
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}