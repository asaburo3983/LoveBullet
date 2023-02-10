using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Cinematic Effect 005 : �V�[���J��
public class CE_005 : CE_Base
{
    [SerializeField, Header("���[�h����V�[��")]
    private SceneObject sceneToLoad;

    // billy memo: �V�[���J�ڂ̉��o�������邩��

    private void OnEnable()
    {
        if (CinematicsManager.Instance != null)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
