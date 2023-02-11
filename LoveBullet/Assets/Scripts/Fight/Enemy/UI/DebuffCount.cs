using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[System.Serializable]
enum BufferType
{
    Atk_Weak,
    Def_Weak,
    Atk_Buff,
    Armor,
    Stan,
}

namespace Enemy
{
    public class DebuffCount : MonoBehaviour
    {
        [SerializeField] BufferType buffType;
        [SerializeField] Enemy enemy;
        Text text;
        // Start is called before the first frame update
        void Start()
        {
            text = GetComponent<Text>();
            switch (buffType)
            {
                case BufferType.Atk_Weak:
                    enemy.gameState.ATWeaken.Subscribe(x =>
                    {
                        SetText(x.ToString());
                    }).AddTo(this);
                    break;
                case BufferType.Def_Weak:
                    enemy.gameState.DFWeaken.Subscribe(x =>
                    {
                        SetText(x.ToString());
                    }).AddTo(this);
                    break;
                case BufferType.Armor:
                    enemy.gameState.DFBuff.Subscribe(x =>
                    {
                        SetText(x.ToString());
                    }).AddTo(this);
                    break;
                case BufferType.Stan:
                    enemy.gameState.stan.Subscribe(x =>
                    {
                        SetText(x.ToString());
                    }).AddTo(this);
                    break;
                default:
                    break;

            }
        }
        void SetText(string x)
        {
            text.text = x;
        }
        // Update is called once per frame
        void Update()
        {
           // SetText(enemy.gameState.ATWeaken.Value.ToString());
        }
    }
}