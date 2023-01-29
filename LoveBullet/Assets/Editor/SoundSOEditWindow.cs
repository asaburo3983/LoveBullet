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

        foreach (var _clip in data) {
            string _name = _clip.name;
            string _path = AssetDatabase.GetAssetPath(_clip);

            if (_path.Contains("SE")) {
                if (!System.IO.File.Exists("Assets/Resources/ScriptableObject/Sound/SE/" + _name + ".asset")) {

                    var obj = ScriptableObject.CreateInstance<SEAssetScriptableObject>();
                    AssetDatabase.CreateAsset(obj, "Assets/Resources/ScriptableObject/Sound/SE/" + _name + ".asset");

                    obj.SetData(_clip, seMix);
                    EditorUtility.SetDirty(obj);
                }
            }
            else {
                if (!System.IO.File.Exists("Assets/Resources/ScriptableObject/Sound/BGM/" + _name + ".asset")) {
                    var obj = ScriptableObject.CreateInstance<BGMAssetScriptableObject>();
                    AssetDatabase.CreateAsset(obj, "Assets/Resources/ScriptableObject/Sound/BGM/" + _name + ".asset");

                    obj.SetData(_clip, bgmMix);
                    EditorUtility.SetDirty(obj);
                }
            }
        }


        // リストの初期化
        _master.SE_List.Clear();
        _master.BGM_List.Clear();

        // SE一覧のEnum作成
        List<Utility.EnumState> state = new List<Utility.EnumState>();
        int id = 0;

        // 全てのアセットをロードする
        var allSe = Resources.LoadAll<SEAssetScriptableObject>("ScriptableObject/Sound/SE/");
        var allBgm = Resources.LoadAll<BGMAssetScriptableObject>("ScriptableObject/Sound/BGM/");

        // リストに入れる
        foreach (var _se in allSe) {
            Utility.EnumState _state = new Utility.EnumState();
            _state.num = id;
            _state.name = _se.name;
            state.Add(_state);

            id++;

            _master.SE_List.Add(_se);
        }
        foreach (var _bgm in allBgm) {
            _master.BGM_List.Add(_bgm);
        }

        // ファイルの更新を反映させる
        EditorUtility.SetDirty(_master);

        // カードSE・エネミーSEのEnumを作成する
        CardSEList();
        EnemySEList();


        AssetDatabase.SaveAssets();
        Utility.EnumCreater.CreateEnumCs("EnumSEList", "SEList", state);

        Debug.Log("AudioScriptableObject Create");
    }

    static void CardSEList()
    {
        //CreateSoundObject();

        var _master = Resources.Load<AudioListScriptableObject>("ScriptableObject/Sound/ListManager");

        var databasePath = Application.streamingAssetsPath + "/Database/";

        var db = new SQLiteUnity.SQLite("CardSE.db", null, databasePath);

        var cmd = "SELECT * FROM CardSE";
        var table = db.ExecuteQuery(cmd);


        List<Utility.EnumState> state = new List<Utility.EnumState>();

        foreach (var row in table.Rows) {
            for (int i = 0; i < _master.SE_List.Count; i++) {
                if (_master.SE_List[i].name == (string)row["FileName"]) {
                    Utility.EnumState _state = new Utility.EnumState();

                    _state.name = _master.SE_List[i].name;
                    _state.num = i;
                    state.Add(_state);

                    break;
                }
            }
        }

        Utility.EnumCreater.CreateEnumCs("EnumCardSE", "Card_SE", state);

        EditorUtility.SetDirty(_master);
        AssetDatabase.SaveAssets();


    }

    static void EnemySEList()
    {
        //CreateSoundObject();

        var _master = Resources.Load<AudioListScriptableObject>("ScriptableObject/Sound/ListManager");

        var databasePath = Application.streamingAssetsPath + "/Database/";

        var db = new SQLiteUnity.SQLite("EnemyAttackSE.db", null, databasePath);

        var cmd = "SELECT * FROM EnemyAttackSE";
        var table = db.ExecuteQuery(cmd);


        List<Utility.EnumState> state = new List<Utility.EnumState>();

        foreach (var row in table.Rows) {
            for(int i = 0; i < _master.SE_List.Count; i++) {
                if(_master.SE_List[i].name == (string)row["FileName"]) {
                    Utility.EnumState _state = new Utility.EnumState();

                    _state.name = _master.SE_List[i].name;
                    _state.num = i;
                    state.Add(_state);

                    break;
                }                
            }
        }

        Utility.EnumCreater.CreateEnumCs("EnumEnemyAttackSE", "EnemyAttack_SE", state);

        EditorUtility.SetDirty(_master);
        AssetDatabase.SaveAssets();

    }

    static public void OnPlay(AudioClip clip)
    {
        var soundPlayer = new SoundPlayer(AssetDatabase.GetAssetPath(clip));

        //音源をロード
        soundPlayer.Load();

        //再生
        soundPlayer.Play();
    }
}
