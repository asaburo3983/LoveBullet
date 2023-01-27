using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using DG.Tweening;

namespace AudioSystem
{
    [System.Serializable]
    struct BGMState
    {
        public BGMAssetScriptableObject bgm;
        public List<string> value;
    }


    [RequireComponent(typeof(AudioSource))]
    public sealed class BGMControl : MonoBehaviour
    {
        private IntReactiveProperty listIdxRP = new IntReactiveProperty();  //再生するBGMのIdx
        private AudioSource audioSource;                                    //オーディオソース

        [Header("設定")]
        [Tooltip("フェードアウトに用いるデフォルト時間"), SerializeField]
        private float fadeOutSeconds = 1f;

        [Tooltip("シーン別BGMリスト"), SerializeField]
        private List<BGMState> bgmSourceList;

        private Tween fadeTween = null;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            //インデックス更新時、自動で再生処理を行うように登録
            listIdxRP
                .Subscribe(_value =>
                {
                    audioSource.volume = 1f;
                    Play(_value);
                })
                .AddTo(this);

            //シーン名を検索キーに変換
            string _keyString = ConvertSceneNameToKeyString(SceneManager.GetActiveScene().name);

            //対応BGMを検索
            SwitchIdx(_keyString);

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        /// <summary>
        /// シーン切り替え時に呼び出す処理
        /// </summary>
        /// <param name="_prevScene"> 前のシーン </param>
        /// <param name="_nextScene"> 次のシーン </param>
        private static void OnActiveSceneChanged(Scene _prevScene, Scene _nextScene)
        {
            var _bgm = AudioControl.Instance.BGM;
            if (!_bgm) {
                return;
            }

            if (_bgm.fadeTween != null) {
                _bgm.fadeTween.Kill();
            }

            //シーン名を検索キーに変換
            string _keyString = _bgm.ConvertSceneNameToKeyString(_nextScene.name);

            //対応BGMを検索
            _bgm.SwitchIdx(_keyString);

            //ボリュームをリセット
            _bgm.audioSource.volume = 1f;

            //ボス戦の時、ボリュームを既定の値に設定
            _bgm.BossStageVolumeSet(_nextScene.name);
        }

        /// <summary>
        /// 音量をフェードさせる（既定秒数）
        /// </summary>
        /// <param name="_volume"> 目標のボリューム </param>
        public Tween FadeVolume(float _volume = 0f)
        {
            //フェードが実行中ならキルする
            if (fadeTween != null)
            {
                fadeTween.Kill();
            }

            fadeTween = DOTween.To(() => audioSource.volume, (x) => audioSource.volume = x, _volume, fadeOutSeconds)
                               .OnComplete(() => fadeTween = null);

            return fadeTween;
        }

        /// <summary>
        /// 音量をフェードアウトさせる（指定秒数）
        /// </summary>
        /// <param name="_seconds"> 完了までの秒数 </param>
        /// <param name="_volume"> 目標のボリューム </param>
        public Tween FadeVolume(float _seconds, float _volume)
        {
            //フェードが実行中ならキルする
            if (fadeTween != null)
            {
                fadeTween.Kill();
            }

            fadeTween = DOTween.To(() => audioSource.volume, (x) => audioSource.volume = x, _volume, _seconds)
                               .OnComplete(() => fadeTween = null);

            return fadeTween;
        }

        /// <summary>
        /// 文字列に対応するアセットを検索し、見つかればインデックスを更新する
        /// </summary>
        /// <param name="_keyValue"> キーとなる文字列 </param>
        public void SwitchIdx(string _keyValue)
        {
            for (int i = 0; i < bgmSourceList.Count; i++)
            {
                foreach (var st in bgmSourceList[i].value) {
                    //見つかったら値を更新して関数終了
                    if (st == _keyValue) {
                        listIdxRP.Value = i;
                        return;
                    }
                }
            }

            listIdxRP.Value = -1;
            Debug.Log("シーンに対応するBGMAssetが見つかりませんでした\nBGMの再生を停止しました");
        }

        /// <summary>
        /// シーン名を検索キーに変換する
        /// </summary>
        /// <param name="_sceneName"> シーン名 </param>
        /// <returns> 検索キー </returns>
        private string ConvertSceneNameToKeyString(string _sceneName)
        {
            string _keyString = _sceneName;

            //メインゲームのシーンかどうか判別
            if (_keyString.Substring(0, 5) == "Stage" &&
                _keyString.Length == 8)
            {
                //「Stage○」まで抜き出す
                _keyString = _keyString.Substring(0, 6);
            }

            return _keyString;
        }

        /// <summary>
        /// ボス戦の時、ボリュームを既定の値に設定
        /// </summary>
        /// <param name="_sceneName"> シーン名 </param>
        private void BossStageVolumeSet(string _sceneName)
        {
            //ボスステージのシーンかどうか判別
            if (_sceneName.Substring(0, 5) == "Stage" &&
                _sceneName.Substring(7, 1) == "6" &&
                _sceneName.Length == 8)
            {
                //音量を０に
                FadeVolume(0f);
            }
        }

        /// <summary>
        /// BGMの切り替えと再生
        /// </summary>
        private void Play(int _idx)
        {
            audioSource.Stop();

            if (_idx >= 0)
            {
                var bgmAsset = bgmSourceList[_idx];

                //再生開始
                audioSource.time = 0f;
                audioSource.PlayBGM(bgmAsset.bgm);
            }
        }

    }
}