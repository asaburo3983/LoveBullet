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
    /// �z�񂩂�Enum�𐶐�����X�N���v�g
    /// </summary>
    public class EnumCreater
    {

#if UNITY_EDITOR
        public static void CreateEnumCs(string _csName, string _enumName, List<EnumState> _state)
        {
            // �쐬����A�Z�b�g�̃p�X
            var filePath = "Assets/Scripts/Enum/" + _csName + ".cs";

            string code;

            code = "public enum " + _enumName + "{\n";

            foreach (var s in _state) {
                code += "    " + s.name + " = " + s.num.ToString() + ",\n";
            }



            code += "    Max \n}";

            // �A�Z�b�g(.cs)���쐬����
            File.WriteAllText(filePath, code);

            // �ύX���������A�Z�b�g���C���|�[�g����(UnityEditor�̍X�V)
            AssetDatabase.Refresh();
        }
#endif
    }
}