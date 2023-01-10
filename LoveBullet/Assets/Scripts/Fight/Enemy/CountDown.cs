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
        [SerializeField] Image actIcon;
        [SerializeField] Text damageText;
        [SerializeField] List<Sprite> type;
        // Start is called before the first frame update
        void Start()
        {
            enemy.gameState.turn.Subscribe(x => {
                countDownText.text = x.ToString();
            }).AddTo(this);

            Change();
        }

        public void Change()
        {
            var pattrn = CacheData.instance.enemyActivePattern[enemy.enemyState.pattern[enemy.gameState.currentIdx]];
            int _type = pattrn.Type;

            actIcon.sprite = type[_type];

            switch (_type) {
                case 0:
                    damageText.enabled = true;
                    int damage = (pattrn.AT + enemy.gameState.ATBuff.Value);

                    // 攻撃デバフ計算
                    if (enemy.gameState.ATWeaken.Value > 0) {
                        damage *= enemy.gameState.Rate.AT;
                        damage /= 100;
                    }
                    // プレイヤー防御デバフ計算
                    if (Player.instance.gameState.DFWeaken.Value > 0) {
                        damage *= Player.instance.Rate.DF;
                        damage /= 100;
                    }
                    damageText.text = damage.ToString();
                    break;
                case 1:
                    damageText.enabled = true;
                    damageText.text = pattrn.DF.ToString();
                    break;
                case 2:
                    damageText.enabled = false;
                    break;
                default:
                    break;
            }
        }
    }
}