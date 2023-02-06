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
        [Header("管理下に置くインスタンス")]

        [Tooltip("ボリュームセッティング"), SerializeField]
        private AudioVolumeSetting volumeSetting;
        public AudioVolumeSetting VolumeSetting => volumeSetting;

        [Tooltip("BGM"), SerializeField]
        private BGMControl bgmControl;
        public BGMControl BGM => bgmControl;

        [Tooltip("SE"), SerializeField]
        private SEControl seControl;
        public SEControl SE => seControl;

        [Header("管理用スクリプタブルオブジェクト")]
        public AudioListScriptableObject audioList;

        public static AudioControl Instance;
        private void Awake()
        {
            Instance = this;
        }
    }

}