using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
namespace FightEnemy
{
    public class CountDown : MonoBehaviour
    {
        [SerializeField] Enemy.Enemy enemy;
        [SerializeField] Text countDownText;
        // Start is called before the first frame update
        void Start()
        {
            enemy.gameState.turn.Subscribe(x =>
            {
                countDownText.text = x.ToString();
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}