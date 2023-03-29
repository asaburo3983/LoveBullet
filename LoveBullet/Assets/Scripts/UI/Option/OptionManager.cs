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
        //�f�[�^���񂶂ē���Ă���
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
        //�J���o�X���t�F�[�h�ŕ\������
        optionCanvas.SetActive(true);

        DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 1.0f, canvasFeadSpeed);
    }
    public void DisableCanvas()
    {
        //�J���o�X���t�F�[�h�Ŕ�\������
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
        Debug.LogError("�I�v�V�����̒l���K������Ă��܂���@�e�L�X�g�X�s�[�h�Ɛ퓬�X�s�[�h OptionManger");
        //���ʂ̒l��K��������
        AudioSystem.AudioControl.Instance.VolumeSetting.MasterVolume = masterVolume;
        AudioSystem.AudioControl.Instance.VolumeSetting.BGMVolume = bgmVolume;
        AudioSystem.AudioControl.Instance.VolumeSetting.SEVolume = seVolume;

        //�e�L�X�g�X�s�[�h��ݒ肷��
        NovelManager.textSpeed = textSpeedList[textSpeedNum - 1];
    }

}
