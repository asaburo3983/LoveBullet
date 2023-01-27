using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

///----------------------------------------------///
/// <summary>
/// オーディオアセット基底クラス
/// </summary>
///----------------------------------------------///

public abstract class AudioAssetScriptableObject : ScriptableObject
{
    [Header("共通項目")]

    [Tooltip("オーディオクリップ"), SerializeField]
    protected AudioClip clip;

    [Tooltip("オーディオミキサーの出力グループ"), SerializeField]
    protected AudioMixerGroup outputGroup;

    public AudioClip Clip => clip;

    public AudioMixerGroup OutputGroup => outputGroup;

    public void SetData(AudioClip _clip,AudioMixerGroup _group)
    {
        clip = _clip;
        outputGroup = _group;
    }
}
