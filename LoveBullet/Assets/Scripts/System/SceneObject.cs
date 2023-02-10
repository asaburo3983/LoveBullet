using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneObject
{
    [SerializeField]
    private string m_SceneName;

    public static implicit operator string(SceneObject sceneObject)
    {
        return sceneObject.m_SceneName;
    }

    public static implicit operator SceneObject(string sceneName)
    {
        return new SceneObject() { m_SceneName = sceneName };
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneObject))]
public class SceneObjectEditor : PropertyDrawer
{
    protected SceneAsset GetSceneObject(string sceneObjectName)
    {
        if (string.IsNullOrEmpty(sceneObjectName))
            return null;

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
            if (scene.path.IndexOf(sceneObjectName) != -1)
            {
                return AssetDatabase.LoadAssetAtPath(scene.path, typeof(SceneAsset)) as SceneAsset;
            }
        }

        Debug.Log("シーン [" + sceneObjectName + "] は使用不可。 ビルド設定にシーンを追加してから再度試してください。");
        return null;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SceneAsset sceneObj = GetSceneObject(property.FindPropertyRelative("m_SceneName").stringValue);
        Object newScene = EditorGUI.ObjectField(position, label, sceneObj, typeof(SceneAsset), false);
        if (newScene == null)
        {
            SerializedProperty prop = property.FindPropertyRelative("m_SceneName");
            prop.stringValue = "";
        }
        else
        {
            if (newScene.name != property.FindPropertyRelative("m_SceneName").stringValue)
            {
                SceneAsset scnObj = GetSceneObject(newScene.name);
                if (scnObj == null)
                {
                    Debug.LogWarning("シーン " + newScene.name + " は使用不可。 ビルド設定にシーンを追加してから再度試してください。");
                }
                else
                {
                    SerializedProperty prop = property.FindPropertyRelative("m_SceneName");
                    prop.stringValue = newScene.name;
                }
            }
        }
    }
}
#endif
