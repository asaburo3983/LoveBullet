using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace FightEnemy
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] RectTransform greenBar;
        [SerializeField] Text hpText;
        float origineSizeX;
        float sizeX;
        [SerializeField] Enemy.Enemy enemy;

        // Start is called before the first frame update
        void Start()
        {
            origineSizeX = greenBar.sizeDelta.x;
            sizeX = origineSizeX * ((float)enemy.gameState.hp.Value / (float)enemy.gameState.maxHP.Value);
            greenBar.sizeDelta = new Vector2(sizeX, greenBar.sizeDelta.y);
            enemy.gameState.hp.Subscribe(x =>
            {
                hpText.text = x.ToString() + "/" + enemy.gameState.maxHP.Value.ToString();
                sizeX = origineSizeX * ((float)x / (float)enemy.gameState.maxHP.Value);
                greenBar.sizeDelta = new Vector2(sizeX, greenBar.sizeDelta.y);
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}