#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
#endif

namespace Utility
{
    [System.Serializable]
    public class EnumState
    {
        public string name;
        public int num;
    }
    /// <summary>
    /// 配列からEnumを生成するスクリプト
    /// </summary>
    public class EnumCreater
    {

#if UNITY_EDITOR
        public static void CreateEnumCs(string _csName, string _enumName, List<EnumState> _state)
        {
            // 作成するアセットのパス
            var filePath = "Assets/Scripts/Enum/" + _csName + ".cs";

            string code;

            code = "public enum " + _enumName + "{\n";

            foreach (var s in _state) {
                code += "    " + s.name + " = " + s.num.ToString() + ",\n";
            }



            code += "    Max \n}";

            // アセット(.cs)を作成する
            File.WriteAllText(filePath, code);

            // 変更があったアセットをインポートする(UnityEditorの更新)
            AssetDatabase.Refresh();
        }
#endif
    }
}