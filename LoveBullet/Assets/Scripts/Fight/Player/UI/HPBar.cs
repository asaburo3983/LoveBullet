using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace FightPlayer
{
    public class HPBar : MonoBehaviour
    {
        Player player;
        [SerializeField] SpriteRendererFillAmount srfa;
        // Start is called before the first frame update
        void Start()
        {
            player = Player.instance;
            player.gameState.hp.Subscribe(x =>
            {
                srfa.FillAmount = ((float)x / (float)player.gameState.maxHP.Value);
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}