using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UniRx;

public sealed class AudioVolumeSetting : MonoBehaviour
{
    [Header("AudioMixer")]
    [Tooltip("マスター"), SerializeField]
    private AudioMixer masterAM;

    [SerializeField]
    private FloatReactiveProperty masterVolume = new FloatReactiveProperty(1.0f);   //マスターボリューム値

    [SerializeField]
    private FloatReactiveProperty bgmVolume = new FloatReactiveProperty(1.0f);      //BGMのボリューム値
    
    [SerializeField]
    private FloatReactiveProperty seVolume = new FloatReactiveProperty(1.0f);       //SEのボリューム値    

    private void Start()
    {
        masterVolume
            .Subscribe(_value => {
                masterAM.SetFloat("BGM_Volume", ConvertVolumeToDb(_value * masterVolume.Value));
                masterAM.SetFloat("SE_Volume", ConvertVolumeToDb(_value * masterVolume.Value));
            }).AddTo(this);

        bgmVolume
            .Subscribe(_value => masterAM.SetFloat("BGM_Volume", ConvertVolumeToDb(_value * masterVolume.Value)))
            .AddTo(this);

        seVolume
            .Subscribe(_value => masterAM.SetFloat("SE_Volume", ConvertVolumeToDb(_value * masterVolume.Value)))
            .AddTo(this);
    }

    /// <summary>
    /// マスターボリューム値
    /// </summary>
    public float MasterVolume {
        get { return masterVolume.Value; }
        set { masterVolume.Value = Mathf.Clamp01(value); }
    }
    public FloatReactiveProperty MasterVolumeReactive => masterVolume;

    /// <summary>
    /// BGMのボリューム値
    /// </summary>
    public float BGMVolume
    {
        get { return bgmVolume.Value; }
        set { bgmVolume.Value = Mathf.Clamp01(value); }
    }
    public FloatReactiveProperty BGMVolumeReactive => bgmVolume;
    

    /// <summary>
    /// SEのボリューム値
    /// </summary>
    public float SEVolume
    {
        get { return seVolume.Value; }
        set { seVolume.Value = Mathf.Clamp01(value); }
    }
    public FloatReactiveProperty SEVolumeReactive => seVolume;

    /// <summary>
    /// ボリューム値をデシベル値に変換する
    /// </summary>
    /// <param name="volume"> ボリューム値 </param>
    /// <returns> デシベル値 </returns>
    private static float ConvertVolumeToDb(float volume)
    {
        return Mathf.Clamp(Mathf.Log10(Mathf.Clamp(volume, 0f, 1f)) * 20f, -80f, 0f);
    }

}
