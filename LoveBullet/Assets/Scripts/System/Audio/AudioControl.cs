using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public enum AudioType
    {
        Master,
        BGM,
        SE
    }

    public sealed class AudioControl : MonoBehaviour
    {
        [Header("�Ǘ����ɒu���C���X�^���X")]

        [Tooltip("�{�����[���Z�b�e�B���O"), SerializeField]
        private AudioVolumeSetting volumeSetting;
        public AudioVolumeSetting VolumeSetting => volumeSetting;

        [Tooltip("BGM"), SerializeField]
        private BGMControl bgmControl;
        public BGMControl BGM => bgmControl;

        [Tooltip("SE"), SerializeField]
        private SEControl seControl;
        public SEControl SE => seControl;

        [Header("�Ǘ��p�X�N���v�^�u���I�u�W�F�N�g")]
        public AudioListScriptableObject audioList;

        public static AudioControl Instance;
        private void Awake()
        {
            Instance = this;
        }
    }

}