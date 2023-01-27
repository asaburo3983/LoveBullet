using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using System.Media;
using System.Linq;

public class SoundSOEditWindow :EditorWindow
{

    int index,index2;
    private readonly string[] tab = { "SE", "BGM" };
    string add;
    Vector2 scroll;

    private void Awake()
    {
        index = 0;
        index2 = 0;
        add = "";
        scroll = Vector2.zero;
    }


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


    [MenuItem("Editor/Sound/Window")]
    public static void CreateWindow()
    {
        // 生成
        var window = GetWindow<SoundSOEditWindow>("サウンド設定");
        window.minSize = new Vector2(520, 420);
        window.maxSize = new Vector2(520, 420);
    }


    private void OnGUI()
    {
        GUILayout.Space(10);

        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
            var _num = GUILayout.Toolbar(index, tab, new GUIStyle(EditorStyles.toolbarButton), GUI.ToolbarButtonSize.FitToContents);
            if (index != _num) {
                index = _num;
            }
        }

        var _master = Resources.Load<AudioListScriptableObject>("ScriptableObject/Sound/ListManager");

        if(index == 0) {
            var _se = _master.SE_List;
            List<string> _str = new List<string>();

            foreach(var l in _master.actionName) {
                _str.Add(l.name);
            }

            _str.Add(string.Empty);
            _str.Add("未選択");

            {
                EditorGUILayout.Space(10);
                index2 = EditorGUILayout.Popup(new GUIContent("アクション名"), index2, _str.ToArray());
                add = EditorGUILayout.TextField("action名",add);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("追加")) {
                    if (add != "" && !_master.ContainsAction(add,_master.enumNumber)) {

                        Utility.EnumState _state = new Utility.EnumState();

                        _state.name = add;
                        _state.num = _master.enumNumber;

                        _master.actionName.Add(_state);
                        add = "";
                        _master.enumNumber++;

                        Utility.EnumCreater.CreateEnumCs("DefineSEAction", "SEAction", _master.actionName);

                        EditorUtility.SetDirty(_master);
                        AssetDatabase.SaveAssets();
                    }
                }
                if (GUILayout.Button("削除")) {
                    if(index2 !=_str.Count - 1) {
                        _master.actionName.RemoveAt(index2);

                        Utility.EnumCreater.CreateEnumCs("DefineSEAction", "SEAction", _master.actionName);
                        index2 = 0;
                    }
                }
                GUILayout.EndHorizontal();
            }


            GUILayout.Space(20);

            // ここから下スクロール
            scroll = EditorGUILayout.BeginScrollView(scroll);

            foreach(var _data in _se) {

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_data.se.name);
                int num = _str.Count - 1;

                if (_str.Contains(_data.key)) {
                    num = _str.IndexOf(_data.key);
                }

                var hoge = EditorGUILayout.Popup(new GUIContent("シーンデータ"), num, _str.ToArray());
                if (hoge != num) {

                    _master.RemoveSEKey(_str[hoge]);
                    _data.key = _str[hoge];

                    EditorUtility.SetDirty(_master);
                    AssetDatabase.SaveAssets();

                    num = hoge;
                }

                if (GUILayout.Button("Play")) {
                    OnPlay(_data.se.Clip);
                }

                GUILayout.EndHorizontal();
            }

            /*
            foreach (var _se in _so) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_se.name);
                int num = _str.Count - 1;

                if (_master.ContainsSE(_se)) {
                    var _key = _master.SE_List.FirstOrDefault(c => c.Value == _se);
                    num = _str.IndexOf(_key.Key);

                    if(num == -1) {
                        num = _str.Count - 1;
                    }
                }
                var hoge = EditorGUILayout.Popup(new GUIContent("シーンデータ"), num, _str.ToArray());
                if (hoge != num) {
                    _master.SE_List[_str[hoge]] = _se;
                    if (num != _str.Count - 1) {
                        _master.SE_List.Remove(_str[num]);

                        EditorUtility.SetDirty(_master);
                        AssetDatabase.SaveAssets();

                    }
                    num = hoge;
                }

                if (GUILayout.Button("Play")) {
                    OnPlay(_se.Clip);
                }

                GUILayout.EndHorizontal();
            }
            */
            EditorGUILayout.EndScrollView();
        }
        else {

        }
    }


    void OnPlay(AudioClip clip)
    {
        var soundPlayer = new SoundPlayer(AssetDatabase.GetAssetPath(clip));

        //音源をロード
        soundPlayer.Load();

        //再生
        soundPlayer.Play();
    }
}
