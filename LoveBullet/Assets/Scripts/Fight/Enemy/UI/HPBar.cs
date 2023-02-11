using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace FightEnemy
{
    public class HPBar : MonoBehaviour
    {

        [SerializeField] Enemy.Enemy enemy;
        [SerializeField] SpriteRendererFillAmount srfa;

        // Start is called before the first frame update
        void Start()
        {
            srfa.FillAmount = (float)enemy.gameState.hp.Value / (float)enemy.gameState.maxHP.Value;
            //HP変更時にバーのサイズを変える
            enemy.gameState.hp.Subscribe(x =>
            {
                srfa.FillAmount = (float)enemy.gameState.hp.Value / (float)enemy.gameState.maxHP.Value;
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}