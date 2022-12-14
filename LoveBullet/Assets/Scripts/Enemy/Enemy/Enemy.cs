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
            public int type;
            public int strengeth;

            public string name;
            public string explanation;
            public int hpMax;
            public int hpFluctuationPlus;
            public int hpFluctuationMinus;

            public int ATFluctuationPlus;
            public int ATFluctuationMinus;
            public List<int> pattern = new List<int>();
            public List<int> value = new List<int>();
        }
        State state;

        [System.Serializable]
        public struct InGameState
        {
            public ReactiveProperty<int> turn;
            public ReactiveProperty<int> hp;
            public ReactiveProperty<int> DF;
            public ReactiveProperty<int> ATWeaken;
            public ReactiveProperty<int> DFWeaken;
            public ReactiveProperty<bool> acted;
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


        private void Start()
        {
            //死亡処理
            gameState.hp.Where(x => x <= 0).Subscribe(x =>
            {
                Destroy(gameObject);//削除処理だけ入れておく
            }).AddTo(this);
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
            gameState.acted.Value = false;

            //とりあえずテキスト代入
            Rough.SetText(ui.turn, gameState.turn.Value);
            Rough.SetText(ui.name, state.name);
            Rough.SetText(ui.hp, gameState.hp.Value);
            Rough.SetText(ui.DF, gameState.DF.Value);
            Rough.SetText(ui.ATWeaken, gameState.ATWeaken.Value);
            Rough.SetText(ui.DFWeaken, gameState.DFWeaken.Value);

            //エネミー番号に対応したグラフィックを入れる
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
            var activeId = state.pattern[gameState.turn.Value];
            var actiovePattern = CacheData.instance.enemyActivePattern[activeId];

            //TODO とりあえず攻撃と防御処理だけ作成
            Player.ReceiveDamage(actiovePattern.AT);//攻撃
            gameState.DF.Value += actiovePattern.DF;//防御

            gameState.turn.Value = (gameState.turn.Value + 1) % state.pattern.Count;//敵内部ターン増加

            gameState.acted.Value = true;//行動終了

        }
        public void ReceiveDamage(int _damage)
        {
            gameState.hp.Value -= (_damage - gameState.DF.Value);

        }
        public void ResetDF()
        {
            gameState.DF.Value = 0;
        }
    }
}
