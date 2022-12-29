using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    { 
        public class State
        {
            public int number;
            public int type;
            public int strengeth;

            public string name;
            public string explanation;
            public int hpMax;
            public int hpFluctuationPlus;
            public int hpFluctuationMinus;

            public List<int> pattern = new List<int>();
            public List<int> value = new List<int>();
        }
        State state;

        [System.Serializable]
        public struct InGameState
        {
            public IntReactiveProperty turn;
            public IntReactiveProperty hp;
            public IntReactiveProperty DF;
            public IntReactiveProperty ATWeaken;
            public IntReactiveProperty DFWeaken;
            public int stan;
            [ReadOnly]public int currentIdx;
        }
        public InGameState gameState;
        [System.Serializable]
        public struct UI
        {
            public GameObject turn;
            public GameObject name;
            public GameObject hp;
            public GameObject DF;
            public GameObject ATWeaken;
            public GameObject DFWeaken;
        }
        [SerializeField] UI ui;

        [SerializeField] SpriteRenderer spriteRendere;

        private void Start()
        {
            //死亡処理
            gameState.hp.Where(x => x <= 0).Subscribe(x => {

                var fight = Card.Fight.instance;
                int _id = fight.enemyObjects.IndexOf(this);
                
                // 削除に伴いターゲットのIDを変更する
                if(fight.TargetId == _id) {

                    if(fight.enemyObjects.Count - 1 == _id) {
                        fight.SetTarget(0);
                    }
                }
                else if(fight.TargetId > _id) {
                    fight.SetTarget(fight.TargetId - 1);
                }

                Destroy(gameObject);
            }).AddTo(this);

            // エネミー行動処理
            gameState.turn.Pairwise()
                .Where(x => x.Previous > 0)
                .Where(x => x.Current <= 0)
                .Subscribe(x => {
                    Action();
                }).AddTo(this);

            // 初期ターン数設定
            gameState.turn.Value = CacheData.instance.enemyActivePattern[state.pattern[gameState.currentIdx]].Turn;
        }
        //TODo 仮でテクスチャを設定する
        void SetTexture()
        {
            //リソースのから仮テクスチャを読み込んで入れる
            var path= "Texture/Fight/Enemy/Enemy"+ state.number.ToString();
            spriteRendere.sprite = Resources.Load<Sprite>(path);

            GetComponent<BoxCollider2D>().size = spriteRendere.bounds.size;

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
            gameState.hp.Value = state.hpMax;
            gameState.DF.Value = 0;
            gameState.ATWeaken.Value = 0;
            gameState.DFWeaken.Value = 0;

            //とりあえずテキスト代入
            Rough.SetText(ui.turn, gameState.turn.Value);
            Rough.SetText(ui.name, state.name);
            Rough.SetText(ui.hp, gameState.hp.Value);
            Rough.SetText(ui.DF, gameState.DF.Value);
            Rough.SetText(ui.ATWeaken, gameState.ATWeaken.Value);
            Rough.SetText(ui.DFWeaken, gameState.DFWeaken.Value);

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

            //TODO とりあえず攻撃と防御処理だけ作成
            Player.ReceiveDamage(actiovePattern.AT);//攻撃
            gameState.DF.Value += actiovePattern.DF;//防御

            // 行動順を一つずらす
            gameState.currentIdx++;
            gameState.currentIdx %= state.pattern.Count;

            // 行動までのターン設定
            var act = CacheData.instance.enemyActivePattern[gameState.currentIdx];
            gameState.turn.Value = act.Turn + Random.Range(0,act.Fluctuation);

        }
        public void ReceiveDamage(int _damage)
        {

            // 防御バフ計算
            if(_damage< gameState.DF.Value) {
                gameState.DF.Value -= _damage;
                return;
            }
            else {
                _damage -= gameState.DF.Value;
                gameState.DF.Value = 0;
            }
            gameState.hp.Value -= (_damage - gameState.DF.Value);
        }

        public void ReceiveStan(int _stan)
        {
            gameState.stan += _stan;
        }

        public void ReceiveATWeaken(int _weak)
        {
            gameState.ATWeaken.Value += _weak;
        }

        public void ReceiveDFWeaken(int _weak)
        {
            gameState.DFWeaken.Value += _weak;
        }

        public void ProgressTurn(int _progressTurn)
        {
            if (gameState.stan > 0) {
                gameState.stan--;
            }
            else {
                gameState.turn.Value -= _progressTurn;
            }
        }

        public void ResetDF()
        {
            gameState.DF.Value = 0;
        }

        public void SetTarget()
        {
            Card.Fight.instance.SetTarget(this);
        }

        private void OnDestroy()
        {
            Card.Fight.instance.enemyObjects.Remove(this);
        }
    }
}
