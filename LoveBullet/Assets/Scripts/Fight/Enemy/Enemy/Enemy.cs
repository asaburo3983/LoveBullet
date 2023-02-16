using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    { 
        [System.Serializable]
        public class State
        {
            public int number;
            public int type;
            public int strengeth;

            public string name;
            public string explanation;
            public int hpMax;
            public int hpFluctuationPlus;

            public List<int> pattern = new List<int>();
            public List<int> value = new List<int>();
        }
        [SerializeField]State state;
        public State enemyState => state;

        public int fieldID;
        [System.Serializable]
        public struct InGameState
        {
            public IntReactiveProperty maxHP;
            public IntReactiveProperty turn;
            public IntReactiveProperty hp;
            public IntReactiveProperty ATBuff;
            public IntReactiveProperty ATBuff_Never;
            public IntReactiveProperty DFBuff;
            public IntReactiveProperty DFBuff_Never;
            public IntReactiveProperty ATWeaken;
            public IntReactiveProperty DFWeaken;
            public IntReactiveProperty stan;
            [ReadOnly]public int currentIdx;
            public ReductionRate Rate;
        }
        public InGameState gameState;
        

        [System.Serializable]
        class TwState
        {
            public float damageTime;
            public float damageShake;
            public float damageDelay;
            public Tween damageTw;

            public float atkTextTime;
            public float atkTime;
            public float atkMove;
        }
        [SerializeField] TwState tw;

        //[SerializeField] Image body;
        [SerializeField] SpriteRenderer sBody;

        [SerializeField] Text actText;
        [SerializeField] FightEnemy.CountDown countDown;

        private void Start()
        {
            if (state == null)
            {
                Debug.LogError("エネミーはイニシャライズされずに生成されました");
                Destroy(this.gameObject);
                return;
            }
            //死亡処理
            gameState.hp.Where(x => x <= 0).Subscribe(x => {

                var fight = FightManager.instance;
                int _id = fight.enemyObjects.IndexOf(this);
                
                // 削除に伴いターゲットのIDを変更する
                if(fight.targetId == _id) {

                    if(fight.enemyObjects.Count - 1 == _id) {
                        fight.SetTarget(0);
                    }
                }
                else if(fight.targetId > _id) {
                    fight.SetTarget(fight.targetId - 1);
                }

                fight.enemyObjects.Remove(this);
                
                transform.Find("UI").gameObject.SetActive(false);

                sBody.DOColor(new Color(1, 1, 1, 0), 1.0f).OnComplete(() => {
                    DOVirtual.DelayedCall(3.0f, () => Destroy(gameObject));
                });


            }).AddTo(this);

            // 初期ターン数設定
            gameState.turn.Value = CacheData.instance.enemyActivePattern[state.pattern[gameState.currentIdx]].Turn;

        }


        //TODo 仮でテクスチャを設定する
        void SetTexture()
        {
            //リソースのから仮テクスチャを読み込んで入れる
            var path= "Texture/Fight/Enemy/Enemy"+ state.number.ToString();
            //body.sprite = Resources.Load<Sprite>(path);
            sBody.sprite = Resources.Load<Sprite>(path);

            //GetComponent<BoxCollider2D>().size = image.bounds.size;

        }
        /// <summary>
        /// ステータス値を代入して
        /// UIなどにデータを代入する
        /// </summary>
        /// <param name="_state"></param>
        public void Initialize(State _state)
        {
            state = _state;
            gameState.turn.Value = 0;
            gameState.maxHP.Value = state.hpMax;
            gameState.hp.Value = state.hpMax;
            gameState.DFBuff.Value = 0;
            gameState.ATWeaken.Value = 0;
            gameState.DFWeaken.Value = 0;

            //エネミー番号に対応したグラフィックを入れる
            SetTexture();
        }

        public static State GetEnemyState(int _id)
        {
            if (_id < 0 || _id >= CacheData.instance.enemyStates.Count)
            {
                Debug.LogError("データ外を参照しようとしています");
            }
           return CacheData.instance.enemyStates[_id];
        }

        /// <summary>
        /// 敵が行動する
        /// </summary>
        public void Action()
        {
            var playerState = Player.instance.gameState;
            var activeId = state.pattern[gameState.currentIdx];
            var actiovePattern = CacheData.instance.enemyActivePattern[activeId];

            int atk = actiovePattern.Damage + gameState.ATBuff.Value + gameState.ATBuff_Never.Value;

            // 攻撃デバフが存在する場合、値の補正を行う
            if (gameState.ATWeaken.Value > 0) {
                atk *= gameState.Rate.AT;
                atk /= 100;
            }
            
            //TODO とりあえず攻撃と防御処理だけ作成
            Player.instance.ReceiveDamage(atk);//攻撃

            // バフ系処理
            gameState.ATBuff.Value = actiovePattern.buff[(int)BuffEnum.Bf_Attack];
            gameState.ATBuff_Never.Value += actiovePattern.buff[(int)BuffEnum.Bf_Attack_Never];

            gameState.DFBuff.Value = Mathf.Clamp(actiovePattern.buff[(int)BuffEnum.Bf_Diffence] > 0 ?
                actiovePattern.buff[(int)BuffEnum.Bf_Diffence] + gameState.DFBuff_Never.Value : 0, 0, 999);
            gameState.DFBuff_Never.Value += actiovePattern.buff[(int)BuffEnum.Bf_Diffence_Never];


            // 回復処理
            gameState.hp.Value += actiovePattern.buff[(int)BuffEnum.Bf_Heal];

            // SE再生
            AudioSystem.AudioControl.Instance.SE.EnemyAttackSePlayOneShot(actiovePattern.SE);

            // 行動順を一つずらす
            gameState.currentIdx++;
            gameState.currentIdx %= state.pattern.Count;

            // 行動までのターン設定
            var act = CacheData.instance.enemyActivePattern[state.pattern[gameState.currentIdx]];
            gameState.turn.Value = act.Turn + Random.Range(0, act.Fluctuation);
        }


        public int ReceiveDamage(int _damage)
        {
            if (_damage <= 0) return 0;

            // 防御デバフ計算  割合増加
            int dmg = (_damage);
            if (gameState.DFWeaken.Value > 0) {
                dmg *= gameState.Rate.DF;
                dmg = (int)((float)dmg/100.0f);
            }

            // 防御バフ計算
            if (dmg < gameState.DFBuff.Value) {
                gameState.DFBuff.Value -= dmg;
                return 0;
            }
            else {
                dmg -= gameState.DFBuff.Value;
                gameState.DFBuff.Value = 0;
            }


            gameState.hp.Value -= dmg;

            DamageAnimation();

            return dmg;
        }

        public void ReceiveStan(int _stan)
        {
            gameState.stan.Value += _stan;
        }

        public void ReceiveATWeaken(int _weak)
        {
            gameState.ATWeaken.Value += _weak;
        }

        public void ReceiveDFWeaken(int _weak)
        {
            gameState.DFWeaken.Value += _weak;
        }

        public bool ProgressTurn(int _progressTurn)
        {
            if (gameState.stan.Value > 0) {
                gameState.stan.Value--;
            }
            else {
                gameState.turn.Value = Mathf.Clamp(gameState.turn.Value - _progressTurn, 0, 9999);
            }

            gameState.ATWeaken.Value = Mathf.Clamp(gameState.ATWeaken.Value - 1, 0, 9999);
            gameState.DFWeaken.Value = Mathf.Clamp(gameState.DFWeaken.Value - 1, 0, 9999);

            // Enemyが行動する場合,trueを返す
            return gameState.turn.Value <= 0;
        }

        public void ResetDF()
        {
            gameState.DFBuff.Value = 0;
        }

        public void SetTarget()
        {
            FightManager.instance.SetTarget(fieldID);
        }

        private void OnDestroy()
        {
            if (tw.damageTw != null) tw.damageTw.Kill();
        }


        public void AttackAnimation()
        {
            if (tw.damageTw != null) {
                tw.damageTw.OnComplete(() => {
                    AtkAnim();
                });
            }
            else {
                AtkAnim();
            }

        }

        void AtkAnim()
        {
            float _delay = 0;
            if (tw.damageTw != null) {
                _delay = tw.damageDelay;
            }

            // TweenAnimationの作成
            Sequence sequence = DOTween.Sequence()
                .Append(DOVirtual.DelayedCall(_delay, () => {
                    actText.text = CacheData.instance.enemyActivePattern[state.pattern[gameState.currentIdx]].name;
                    actText.gameObject.SetActive(true);
                }))
                .Append(transform.DOLocalMoveX(transform.localPosition.x - tw.atkMove, tw.atkTime).SetDelay(tw.atkTextTime).SetLoops(2, LoopType.Yoyo))
                .AppendCallback(() => { Player.instance.ReceiveAnim(); Action(); })
                .OnComplete(() => {
                    actText.gameObject.SetActive(false);
                    FightManager.instance.actEnemy.Remove(this);
                    countDown.Change();
                });
        }



        public void DamageAnimation()
        {
            // 被ダメージのアニメーション
            if (tw.damageTw != null) tw.damageTw.Kill(true);

            tw.damageTw = transform.DOShakePosition(tw.damageTime, tw.damageShake, 30, 1, false, true)
                .SetLoops(2, LoopType.Yoyo).OnComplete(() => tw.damageTw = null);


            transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(new Color(1, 0, 0, 1), tw.damageTime).SetLoops(2, LoopType.Yoyo);

            //sBody.DOColor(Color.red, tw.damageTime / 6f).SetLoops(2, LoopType.Yoyo);
        }
    }
}
