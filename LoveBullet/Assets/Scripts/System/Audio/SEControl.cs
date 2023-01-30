using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

namespace AudioSystem
{

    public sealed class SEControl : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField, Tooltip("オーディオソースの準備数"), Range(1, 70)]
        private int audioSourceCapacity = 30;

        private List<AudioSource> audioSourceList = new List<AudioSource>();

        private int listIdx = 0;

        [SerializeField, Tooltip("Mixer")]
        private AudioMixer mixer;

        void Awake()
        {
            for (int i = 0; i < audioSourceCapacity; i++)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSourceList.Add(audioSource);
            }
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="_asset"> アセット </param>
        public AudioSource Play(string _path)
        {
            //再生処理
            var audioSource = audioSourceList[listIdx];
            audioSource.PlaySE(_path);

            //インデックスを更新
            UpdateIdx();

            return audioSource;
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="_act"></param>
        public AudioSource PlaySeOneShot(SEList _act)
        {
            //再生処理
            var audioSource = audioSourceList[listIdx];
            audioSource.PlaySE(_act.ToString());

            //インデックスを更新
            UpdateIdx();

            return audioSource;
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="_act"></param>
        public AudioSource CardSePlayOneShot(int id)
        {
            //再生処理
            var audioSource = audioSourceList[listIdx];
            audioSource.PlayCardSE(id);

            //インデックスを更新
            UpdateIdx();

            return audioSource;
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="_act"></param>
        public AudioSource EnemyAttackSePlayOneShot(int id)
        {
            //再生処理
            var audioSource = audioSourceList[listIdx];
            audioSource.PlayEnemyAttackSE(id);

            //インデックスを更新
            UpdateIdx();

            return audioSource;
        }



        /// <summary>
        /// インデックスを更新する
        /// </summary>
        private void UpdateIdx()
        {
            int _cnt = 0;

            while (true)
            {
                listIdx++;

                if (listIdx >= audioSourceList.Count)
                {
                    listIdx = 0;
                }

                if (!audioSourceList[listIdx].isPlaying)
                {
                    break;
                }

                _cnt++;

                if(_cnt >= audioSourceList.Count)
                {
                    Debug.LogWarning("SE再生用AudioSourceの空きがありません。capacityを見直してください");
                    break;
                }
            }
        }

        /// <summary>
        /// シーンが切り替わった時に呼ばれる関数
        /// </summary>
        /// <param name="_prev"> 前のシーン </param>
        /// <param name="_next"> 次のシーン </param>
        private void OnActiveSceneChanged(Scene _prev,Scene _next)
        {
            AudioControl.Instance.SE.StopAll();
        }

        /// <summary>
        /// 全てのSEの再生を停止する
        /// </summary>
        private void StopAll()
        {
            foreach(var _audio in audioSourceList)
            {
                _audio.loop = false;
                _audio.Stop();
            }
        }

        /// <summary>
        /// パラメータを変更する
        /// </summary>
        /// <param name="_key"> キー </param>
        /// <param name="_value"> 値 </param>
        public void SetFloat(string _key,float _value)
        {
            mixer.SetFloat(_key, _value);
        }
    }

}