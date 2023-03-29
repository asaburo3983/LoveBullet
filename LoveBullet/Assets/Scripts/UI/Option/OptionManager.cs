using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class OptionManager : SingletonMonoBehaviour<OptionManager>
{
    public float masterVolume;
    public float bgmVolume;
    public float seVolume;

    public int fightSpeedNum;
    public int textSpeedNum;
    [SerializeField] List<float> textSpeedList = new List<float>();

    public GameObject optionCanvas;
    [SerializeField] Slider masterVolSlider;
    [SerializeField] Slider seVolSlider;
    [SerializeField] Slider bgmVolSlider;

    [SerializeField] GameObject checkFight;
    [SerializeField] GameObject checkText;

    [SerializeField] List<Transform> fightButton;
    [SerializeField] List<Transform> textButton;
    CanvasGroup canvasGroup;
    [SerializeField] float canvasFeadSpeed;
    

    private void Awake()
    {
        SingletonCheck(this, true);
        masterVolume = 0.5f;
        bgmVolume = 0.5f;
        seVolume = 0.5f;
        SetSlider(true);

        fightSpeedNum = 3;
        textSpeedNum = 3;
        canvasGroup=optionCanvas.GetComponent<CanvasGroup>();

        
    }
    void Start()
    {
        DisableCanvas();
        //データを先んじて入れておく
        textSpeedNum = 3;
        SetChangeData();
    }
    private void Update()
    {
        SetSlider(false);
    }
    public void ActiveCanvas()
    {
        if (optionCanvas.active)
        {
            DisableCanvas();
        }
        else
        {
            EnableCanvas();

        }
    }
    public void EnableCanvas()
    {
        //カンバスをフェードで表示する
        optionCanvas.SetActive(true);

        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 1.0f, canvasFeadSpeed);
    }
    public void DisableCanvas()
    {
        //カンバスをフェードで非表示する
        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 0.0f, canvasFeadSpeed).OnComplete(()=> optionCanvas.SetActive(false));
        SetChangeData();
    }

    public void SetFightSpeed(int sp)
    {
        fightSpeedNum = sp;
        checkFight.transform.position = fightButton[sp - 1].position;
    }
    public void SetTextSpeed(int sp)
    {
        textSpeedNum = sp;
        checkText.transform.position = textButton[sp - 1].position;
    }
    void SetSlider(bool sliderToValue)
    {
        if (sliderToValue == false)
        {
            masterVolume = masterVolSlider.value;
            bgmVolume = bgmVolSlider.value;
            seVolume = seVolSlider.value;
            
        }
        else
        {
            masterVolSlider.value = masterVolume;
            bgmVolSlider.value= bgmVolume;
            seVolSlider.value=seVolume;
        }

    }
    void SetChangeData()
    {
        Debug.LogError("オプションの値が適応されていません　テキストスピードと戦闘スピード OptionManger");
        //音量の値を適応させる
        AudioSystem.AudioControl.Instance.VolumeSetting.MasterVolume = masterVolume;
        AudioSystem.AudioControl.Instance.VolumeSetting.BGMVolume = bgmVolume;
        AudioSystem.AudioControl.Instance.VolumeSetting.SEVolume = seVolume;

        //テキストスピードを設定する
        NovelManager.textSpeed = textSpeedList[textSpeedNum - 1];
    }

}
