using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using System.Media;

public class SoundSOEditWindow :EditorWindow
{
    [MenuItem("Editor/Sound/CreateScriptableObject")]
    static void CreateSoundObject()
    {

        var _master = Resources.Load<AudioListScriptableObject>("ScriptableObject/Sound/ListManager");

        var data = Resources.LoadAll<AudioClip>("Sound");

        var seMix = Resources.Load<AudioMixerGroup>("AudioMixer/AM_SE");
        var bgmMix = Resources.Load<AudioMixerGroup>("AudioMixer/AM_BGM");

        List<Utility.EnumState> state = new List<Utility.EnumState>();
        int id = 0;

        foreach (var _clip in data) {
            string _name = _clip.name;
            string _path = AssetDatabase.GetAssetPath(_clip);

            if (_path.Contains("SE")) {
                if (!System.IO.File.Exists("Assets/Resources/ScriptableObject/Sound/SE/" + _name + ".asset")) {

                    var obj = ScriptableObject.CreateInstance<SEAssetScriptableObject>();
                    AssetDatabase.CreateAsset(obj, "Assets/Resources/ScriptableObject/Sound/SE/" + _name + ".asset");

                    obj.SetData(_clip, seMix);


                    SEData sd = new SEData();
                    sd.se = obj;

                    _master.SE_List.Add(sd);

                    EditorUtility.SetDirty(obj);
                }

                Utility.EnumState _state = new Utility.EnumState();
                _state.num = id;
                _state.name = _name;
                state.Add(_state);

                id++;
            }
            else {
                if (!System.IO.File.Exists("Assets/Resources/ScriptableObject/Sound/BGM/" + _name + ".asset")) {
                    var obj = ScriptableObject.CreateInstance<BGMAssetScriptableObject>();
                    AssetDatabase.CreateAsset(obj, "Assets/Resources/ScriptableObject/Sound/BGM/" + _name + ".asset");

                    obj.SetData(_clip, bgmMix);

                    BGMData bd = new BGMData();
                    bd.bgm = obj;

                    _master.BGM_List.Add(bd);

                    EditorUtility.SetDirty(obj);
                }
            }

        }

        EditorUtility.SetDirty(_master);
        AssetDatabase.SaveAssets();
        Utility.EnumCreater.CreateEnumCs("SEList", "SE_List", state);

        Debug.Log("AudioScriptableObject Create");
    }

    [MenuItem("Editor/Sound/EnemySEList")]
    static void EnemySEList()
    {

    }
    [MenuItem("Editor/Sound/CardSEList")]
    static void CardSEList()
    {

    }


    void OnPlay(AudioClip clip)
    {
        var soundPlayer = new SoundPlayer(AssetDatabase.GetAssetPath(clip));

        //âπåπÇÉçÅ[Éh
        soundPlayer.Load();

        //çƒê∂
        soundPlayer.Play();
    }
}
