using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    /// -----------------------------------------------///
    /// <summary>
    /// AudioSource�g���N���X
    /// </summary>
    /// -----------------------------------------------///
    public static class AudioSourceExtensions
    {
        /// <summary>
        /// SE�A�Z�b�g���Đ�����
        /// </summary>
        /// <param name="_audioSource"> this </param>
        /// <param name="_audioAsset"> �Đ�����I�[�f�B�I�A�Z�b�g </param>
        public static void PlaySE(this AudioSource _audioSource, string _audioPath, bool _loop = false)
        {
            var list = AudioControl.Instance.audioList;

            var _se = list.GetSE(_audioPath);

            if (_se == null) {
                Debug.LogError("SE Play Error");
                return;
            }

            _audioSource.loop = _loop;
            _audioSource.Play(_se);

        }

        public static void PlayCardSE(this AudioSource _audioSource, int cardSeID, bool _loop = false)
        {
            var list = AudioControl.Instance.audioList;

            int number = (int)System.Enum.ToObject(typeof(Card_SE), cardSeID);


            if (list.SE_List.Count < number) {
                Debug.LogError("SE Play Error");
                return;
            }

            _audioSource.loop = _loop;
            _audioSource.Play(list.SE_List[number]);

        }

        public static void PlayEnemyAttackSE(this AudioSource _audioSource, int eaSeID, bool _loop = false)
        {
            var list = AudioControl.Instance.audioList;

            int number = (int)System.Enum.ToObject(typeof(EnemyAttack_SE), eaSeID);


            if (list.SE_List.Count < number) {
                Debug.LogError("SE Play Error");
                return;
            }

            _audioSource.loop = _loop;
            _audioSource.Play(list.SE_List[number]);

        }



        /// <summary>
        /// SE�A�Z�b�g���Đ�����
        /// </summary>
        /// <param name="_audioSource"> this </param>
        /// <param name="_audioAsset"> �Đ�����I�[�f�B�I�A�Z�b�g </param>
        public static void PlaySE(this AudioSource _audioSource, SEAssetScriptableObject _audioAsset, float _pitch)
        {
            _audioSource.Play(_audioAsset, _pitch);
        }

        /// <summary>
        /// SE�A�Z�b�g���iPlayOneShot���ɃV�X�e�����Łj�Đ�����
        /// </summary>
        /// <param name="_audioSource"></param>
        /// <param name="_audioAsset"></param>
        /// <returns> �Đ������s���Ă���AudioSource </returns>
        public static AudioSource PlaySEOneShot(this AudioSource _audioSource,string _path)
        {
            return AudioControl.Instance.SE.Play(_path);
        }

        /// <summary>
        /// BGM�A�Z�b�g���Đ�����
        /// </summary>
        /// <param name="_audioSource"> this </param>
        /// <param name="_audioAsset"> �Đ�����I�[�f�B�I�A�Z�b�g </param>
        /// <param name="_loop"> ���[�v�̗L���itrue�F����@false�F�Ȃ��j </param>
        public static void PlayBGM(this AudioSource _audioSource, BGMAssetScriptableObject _audioAsset, bool _loop = true)
        {
            _audioSource.loop = _loop;
            _audioSource.Play(_audioAsset);
        }

        /// <summary>
        /// �I�[�f�B�I�A�Z�b�g���Đ�����
        /// </summary>
        /// <param name="_audioSource"> this </param>
        /// <param name="_audioAsset"> �Đ�����I�[�f�B�I�A�Z�b�g </param>
        private static void Play(this AudioSource _audioSource, AudioAssetScriptableObject _audioAsset, float _pitch = 1f)
        {
            _audioSource.clip = _audioAsset.Clip;
            _audioSource.pitch = _pitch;
            _audioSource.outputAudioMixerGroup = _audioAsset.OutputGroup;
            _audioSource.Play();
        }
    }

}
