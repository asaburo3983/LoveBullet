using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuffStateCreate
{
    [MenuItem("Editor/BuffStateCreate")]
    static void Create()
    {
        var databasePath = Application.streamingAssetsPath + "/Database/";

        var db = new SQLiteUnity.SQLite("Buff.db", null, databasePath);

        var cmd = "SELECT * FROM Buff";
        var table = db.ExecuteQuery(cmd);


        List<Utility.EnumState> state = new List<Utility.EnumState>();

        foreach (var row in table.Rows) {
            Utility.EnumState _state = new Utility.EnumState();
            _state.name = (string)row["EnumName"];
            _state.num = (int)row["Number"];

            state.Add(_state);
        }

        Utility.EnumCreater.CreateEnumCs("BuffEnum", "BuffEnum", state);      
    }
}
