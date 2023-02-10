using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Cinematic Effect 005 : シーン遷移
public class CE_005 : CE_Base
{
    [SerializeField, Header("ロードするシーン")]
    private SceneObject sceneToLoad;

    // billy memo: シーン遷移の演出後日入れるかも

    private void OnEnable()
    {
        if (CinematicsManager.Instance != null)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
