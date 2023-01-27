using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    /// -----------------------------------------------///
    /// <summary>
    /// AudioSource拡張クラス
    /// </summary>
    /// -----------------------------------------------///
    public static class AudioSourceExtensions
    {
        /// <summary>
        /// SEアセットを再生する
        /// </summary>
        /// <param name="_audioSource"> this </param>
        /// <param name="_audioAsset"> 再生するオーディオアセット </param>
        public static void PlaySE(this AudioSource _audioSource, string _audioPath, bool _loop = false)
        {
            var list = AudioControl.Instance.audioList;

            if (!AudioControl.Instance.audioList.ContainsAction(_audioPath)) {
                Debug.LogError("存在しないアクション名です　; " + _audioPath);
                return;
            }

            if (list.ContainsSE(_audioPath)) {
                _audioSource.loop = _loop;
                _audioSource.Play(list.GetSE(_audioPath));
            }
            else {
                Debug.LogError("現在使用されていないアクション名です　; " + _audioPath);
            }
        }

        /// <summary>
        /// SEアセットを再生する
        /// </summary>
        /// <param name="_audioSource"> this </param>
        /// <param name="_audioAsset"> 再生するオーディオアセット </param>
        public static void PlaySE(this AudioSource _audioSource, SEAssetScriptableObject _audioAsset, float _pitch)
        {
            _audioSource.Play(_audioAsset, _pitch);
        }

        /// <summary>
        /// SEアセットを（PlayOneShot風にシステム側で）再生する
        /// </summary>
        /// <param name="_audioSource"></param>
        /// <param name="_audioAsset"></param>
        /// <returns> 再生を実行しているAudioSource </returns>
        public static AudioSource PlaySEOneShot(this AudioSource _audioSource,string _path)
        {
            return AudioControl.Instance.SE.Play(_path);
        }

        /// <summary>
        /// BGMアセットを再生する
        /// </summary>
        /// <param name="_audioSource"> this </param>
        /// <param name="_audioAsset"> 再生するオーディオアセット </param>
        /// <param name="_loop"> ループの有無（true：あり　false：なし） </param>
        public static void PlayBGM(this AudioSource _audioSource, BGMAssetScriptableObject _audioAsset, bool _loop = true)
        {
            _audioSource.loop = _loop;
            _audioSource.Play(_audioAsset);
        }

        /// <summary>
        /// オーディオアセットを再生する
        /// </summary>
        /// <param name="_audioSource"> this </param>
        /// <param name="_audioAsset"> 再生するオーディオアセット </param>
        private static void Play(this AudioSource _audioSource, AudioAssetScriptableObject _audioAsset, float _pitch = 1f)
        {
            _audioSource.clip = _audioAsset.Clip;
            _audioSource.pitch = _pitch;
            _audioSource.outputAudioMixerGroup = _audioAsset.OutputGroup;
            _audioSource.Play();
        }
    }

}
