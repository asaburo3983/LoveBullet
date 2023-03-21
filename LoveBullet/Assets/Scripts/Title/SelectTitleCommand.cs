using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
public class SelectTitleCommand : MonoBehaviour
{
    [SerializeField] float moveX;
    [SerializeField] float speed;

    [SerializeField] List<GameObject> startShotSign = new List<GameObject>();
    [SerializeField] List<float> shotSignTime=new List<float>();

    [SerializeField] GameObject fadeImage;
    [SerializeField] float screenEffectTime;
    [SerializeField] float fadeTime;

    [SerializeField] float sceneMoveTime;

    float originePosX;

    bool sceneMoving;
    void Start()
    {
        originePosX = transform.position.x;
    }

    public void OnEnterMove()
    {

        transform.DOMoveX(moveX, speed);
    }
    public void OnExitMove()
    {
        transform.DOMoveX(originePosX, speed);
    }
    //�I����
    public void OnEnterStart()
    {

        if (sceneMoving) { return; }
        sceneMoving = true;

        NovelManager.novelMode = NovelManager.NovelMode.Novel;
        NovelManager.chapterNum = 0;

        //���o�I����V�[���ړ�
        //��ʂɒe����\��
        float screenEffectTiming = shotSignTime.Sum() + screenEffectTime;
        float sceneMoveTiming = screenEffectTiming + sceneMoveTime;
        Sequence sequence = DOTween.Sequence().OnStart(() => { })
        .Append(DOVirtual.DelayedCall(shotSignTime[0], () => { startShotSign[0].SetActive(true); }))
        .Append(DOVirtual.DelayedCall(shotSignTime[0] + shotSignTime[1], () => { startShotSign[1].SetActive(true); }))
        .Append(DOVirtual.DelayedCall(shotSignTime[0] + shotSignTime[1] + shotSignTime[2], () => { startShotSign[2].SetActive(true); }))
        .Append(DOVirtual.DelayedCall(shotSignTime.Sum(), () => { startShotSign[3].SetActive(true); }))
        .Append(DOVirtual.DelayedCall(screenEffectTiming, () => { fadeImage.SetActive(true); fadeImage.GetComponent<Image>().DOFade(1, fadeTime); }))
        .Append(DOVirtual.DelayedCall(sceneMoveTiming, () => { SceneManager.LoadScene("Novel"); }));






        }
    public void OnEnterContinue()
    {
        if (sceneMoving) { return; }
        sceneMoving = true;

        //�Z�[�u�f�[�^�����[�h���ăQ�[���ɍs��
    }
    public void OnEnterOption()
    {
        if (sceneMoving) { return; }
        
        OptionManager.instance.EnableCanvas();
    }
    public void OnEnterEnd()
    {
        if (sceneMoving) { return; }
        sceneMoving = true;

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
    #else
        Application.Quit();//�Q�[���v���C�I��
    #endif
    }

}
