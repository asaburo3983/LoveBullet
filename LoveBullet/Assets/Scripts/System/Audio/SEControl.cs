using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

namespace AudioSystem
{

    public sealed class SEControl : MonoBehaviour
    {
        [Header("�ݒ�")]
        [SerializeField, Tooltip("�I�[�f�B�I�\�[�X�̏�����"), Range(1, 70)]
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
        /// SE���Đ�����
        /// </summary>
        /// <param name="_asset"> �A�Z�b�g </param>
        public AudioSource Play(string _path)
        {
            //�Đ�����
            var audioSource = audioSourceList[listIdx];
            audioSource.PlaySE(_path);

            //�C���f�b�N�X���X�V
            UpdateIdx();

            return audioSource;
        }

        /// <summary>
        /// SE���Đ�����
        /// </summary>
        /// <param name="_act"></param>
        public AudioSource PlaySeOneShot(SEList _act)
        {
            //�Đ�����
            var audioSource = audioSourceList[listIdx];
            audioSource.PlaySE(_act.ToString());

            //�C���f�b�N�X���X�V
            UpdateIdx();

            return audioSource;
        }

        /// <summary>
        /// SE���Đ�����
        /// </summary>
        /// <param name="_act"></param>
        public AudioSource CardSePlayOneShot(int id)
        {
            //�Đ�����
            var audioSource = audioSourceList[listIdx];
            audioSource.PlayCardSE(id);

            //�C���f�b�N�X���X�V
            UpdateIdx();

            return audioSource;
        }

        /// <summary>
        /// SE���Đ�����
        /// </summary>
        /// <param name="_act"></param>
        public AudioSource EnemyAttackSePlayOneShot(int id)
        {
            //�Đ�����
            var audioSource = audioSourceList[listIdx];
            audioSource.PlayEnemyAttackSE(id);

            //�C���f�b�N�X���X�V
            UpdateIdx();

            return audioSource;
        }



        /// <summary>
        /// �C���f�b�N�X���X�V����
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
                    Debug.LogWarning("SE�Đ��pAudioSource�̋󂫂�����܂���Bcapacity���������Ă�������");
                    break;
                }
            }
        }

        /// <summary>
        /// �V�[�����؂�ւ�������ɌĂ΂��֐�
        /// </summary>
        /// <param name="_prev"> �O�̃V�[�� </param>
        /// <param name="_next"> ���̃V�[�� </param>
        private void OnActiveSceneChanged(Scene _prev,Scene _next)
        {
            AudioControl.Instance.SE.StopAll();
        }

        /// <summary>
        /// �S�Ă�SE�̍Đ����~����
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
        /// �p�����[�^��ύX����
        /// </summary>
        /// <param name="_key"> �L�[ </param>
        /// <param name="_value"> �l </param>
        public void SetFloat(string _key,float _value)
        {
            mixer.SetFloat(_key, _value);
        }
    }

}