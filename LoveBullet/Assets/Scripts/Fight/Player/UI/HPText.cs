using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace FightPlayer
{
    public class HPText : MonoBehaviour
    {
        Player player;
        [SerializeField] Text hpText;

        // Start is called before the first frame update
        void Start()
        {
            player = Player.instance;
            player.gameState.hp.Subscribe(x =>
            {
                hpText.text = x.ToString() + "/" + player.gameState.maxHP.Value.ToString();
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}