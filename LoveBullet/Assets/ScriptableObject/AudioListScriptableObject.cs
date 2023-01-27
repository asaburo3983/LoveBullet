using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SEData
{
    public string key = string.Empty;
    public SEAssetScriptableObject se;
}

[System.Serializable]
public class BGMData
{
    public string key = string.Empty;
    public BGMAssetScriptableObject bgm;
}

[CreateAssetMenu(fileName = "Assets/Resources/ScriptableObject/Audio/SO_Master", menuName = "ScriptableObjects/Audio/Master")]
public class AudioListScriptableObject : ScriptableObject
{
     public List<Utility.EnumCreater.EnumState> actionName = new List<Utility.EnumCreater.EnumState>();
     public List<SEData> SE_List = new List<SEData>();
     public List<BGMData> BGM_List = new List<BGMData>();
     public int enumNumber = 0;

    public bool ContainsAction(int _num)
    {
        foreach (var act in actionName) {
            if (act.num == _num) return true;
        }
        return false;
    }
    public bool ContainsAction(string _key)
    {
        foreach (var act in actionName) {
            if (act.name == _key) return true;
        }
        return false;
    }

    public bool ContainsAction(string _key, int _num)
    {
        foreach (var act in actionName) {
            if (act.name == _key) return true;
            if (act.num == _num) return true;

        }
        return false;
    }



    public bool ContainsSE(string _key)
    {
        foreach (var data in SE_List) {
            if (data.key == _key) return true;
        }
        return false;
    }

    public bool ContainsSE(SEAssetScriptableObject _se)
    {
        foreach (var data in SE_List) {
            if (data.se == _se) return true;
        }
        return false;
    }

    public void RemoveSEKey(string _str)
    {
        foreach (var data in SE_List) {
            if (data.key == _str) data.key = string.Empty;
        }
    }

    public SEAssetScriptableObject GetSE(string _key)
    {
        foreach (var data in SE_List) {
            if (data.key == _key) return data.se;
        }

        Debug.Log("‘¶İ‚µ‚Ä‚¢‚È‚¢Key‚ªŒÄ‚Ño‚³‚ê‚Ü‚µ‚½");
        return null;
    }

    public bool ContainsBGM(string _key)
    {
        foreach (var data in BGM_List) {
            if (data.key == _key) return true;
        }
        return false;
    }

    public BGMAssetScriptableObject GetBGM(string _key)
    {
        foreach (var data in BGM_List) {
            if (data.key == _key) return data.bgm;
        }

        Debug.Log("‘¶İ‚µ‚Ä‚¢‚È‚¢Key‚ªŒÄ‚Ño‚³‚ê‚Ü‚µ‚½");
        return null;
    }

    //public Dictionary<string, SEAssetScriptableObject> SE_List = new Dictionary<string, SEAssetScriptableObject>();
    //public Dictionary<string, BGMAssetScriptableObject> BGM_List = new Dictionary<string, BGMAssetScriptableObject>();
}