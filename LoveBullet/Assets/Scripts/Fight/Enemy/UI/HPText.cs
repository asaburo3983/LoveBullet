using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace Enemy
{
    public class HPText : MonoBehaviour
    {
        [SerializeField] Enemy enemy;
        [SerializeField] Text hpText;

        // Start is called before the first frame update
        void Start()
        {
            enemy.gameState.hp.Subscribe(x =>
            {
                hpText.text = x.ToString() + "/" + enemy.gameState.maxHP.Value.ToString();
            }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}