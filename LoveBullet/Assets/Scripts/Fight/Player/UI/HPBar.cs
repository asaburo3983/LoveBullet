using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace FightPlayer
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] RectTransform greenBar;

        float origineSizeX;
        float sizeX;
        Player player;

        // Start is called before the first frame update
        void Start()
        {
            player = Player.instance;
            origineSizeX = greenBar.sizeDelta.x;
            sizeX = origineSizeX * ((float)player.gameState.hp.Value / (float)player.gameState.maxHP.Value);
            greenBar.sizeDelta = new Vector2(sizeX, greenBar.sizeDelta.y);
            player.gameState.hp.Subscribe(x =>
            {
                sizeX = origineSizeX * ((float)x / (float)player.gameState.maxHP.Value);
                greenBar.sizeDelta = new Vector2(sizeX, greenBar.sizeDelta.y);
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}