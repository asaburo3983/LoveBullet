using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace FightEnemy
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] Transform greenBar;
        float origineSizeX;
        float sizeX;
        [SerializeField] Enemy.Enemy enemy;

        // Start is called before the first frame update
        void Start()
        {
            origineSizeX = greenBar.localScale.x;

            sizeX = origineSizeX * ((float)enemy.gameState.hp.Value / (float)enemy.gameState.maxHP.Value);
            greenBar.localScale = new Vector3(sizeX, greenBar.localScale.y);

            //HP変更時にバーのサイズを変える
            enemy.gameState.hp.Subscribe(x =>
            {
                sizeX = origineSizeX * ((float)x / (float)enemy.gameState.maxHP.Value);
                greenBar.localScale = new Vector3(sizeX, greenBar.localScale.y);
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}