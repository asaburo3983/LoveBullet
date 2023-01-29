using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/ScriptableObject/Audio/SO_Master", menuName = "ScriptableObjects/Audio/Master")]
public class AudioListScriptableObject : ScriptableObject
{
    public List<Utility.EnumState> actionName = new List<Utility.EnumState>();
    public List<SEAssetScriptableObject> SE_List = new List<SEAssetScriptableObject>();
    public List<BGMAssetScriptableObject> BGM_List = new List<BGMAssetScriptableObject>();

    public SEAssetScriptableObject GetSE(string _key)
    {
        foreach (var data in SE_List) {
            if (data.name == _key) return data;
        }

        Debug.Log("‘¶İ‚µ‚Ä‚¢‚È‚¢Key‚ªŒÄ‚Ño‚³‚ê‚Ü‚µ‚½");
        return null;
    }

}