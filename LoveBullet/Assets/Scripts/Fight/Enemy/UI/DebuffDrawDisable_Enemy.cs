using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Enemy
{
    public class DebuffDrawDisable_Enemy : MonoBehaviour
    {
        [SerializeField] BufferType buffType;
        [SerializeField] Enemy enemy;
        // Start is called before the first frame update
        void Start()
        {
            SetDraw(0);
            switch (buffType)
            {
                case BufferType.Atk_Weak:
                    enemy.gameState.ATWeaken.Subscribe(x =>
                    {
                        SetDraw(x);
                    }).AddTo(this);
                    break;
                case BufferType.Def_Weak:
                    enemy.gameState.DFWeaken.Subscribe(x =>
                    {
                        SetDraw(x);
                    }).AddTo(this);
                    break;
                case BufferType.Armor:
                    enemy.gameState.DFBuff.Subscribe(x =>
                    {
                        SetDraw(x);
                    }).AddTo(this);
                    break;
                case BufferType.Stan:
                    enemy.gameState.stan.Subscribe(x =>
                    {
                        SetDraw(x);
                    }).AddTo(this);
                    break;
                default:
                    break;

            }

        }
        void SetDraw(int x)
        {
            var b = false;
            if (x > 0) { b = true; }
            GetComponent<SpriteRenderer>().enabled =b;
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
