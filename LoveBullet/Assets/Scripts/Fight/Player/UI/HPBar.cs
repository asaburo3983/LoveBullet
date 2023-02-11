using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace FightPlayer
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] Transform greenBar;

        float origineSizeX;
        float sizeX;
        Player player;

        // Start is called before the first frame update
        void Start()
        {
            player = Player.instance;
            origineSizeX = greenBar.localScale.x;
            sizeX = origineSizeX * ((float)player.gameState.hp.Value / (float)player.gameState.maxHP.Value);
            greenBar.localScale = new Vector3(sizeX, greenBar.localScale.y);
            player.gameState.hp.Subscribe(x =>
            {
                sizeX = origineSizeX * ((float)x / (float)player.gameState.maxHP.Value);
                greenBar.localScale = new Vector2(sizeX, greenBar.localScale.y);
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}