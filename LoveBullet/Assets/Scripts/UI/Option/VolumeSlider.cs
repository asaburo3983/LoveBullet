using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class VolumeSlider : MonoBehaviour
{
    AudioSystem.AudioControl cont;
    [SerializeField] Image bar;
    [SerializeField] Canvas canvas;
    Vector2 clickPos;
    float start;
    FloatReactiveProperty pos;

    public AudioSystem.AudioType type;

    // Start is called before the first frame update
    void Start()
    {
        cont = AudioSystem.AudioControl.Instance;
        float _width = bar.rectTransform.rect.width;


        if(type == AudioSystem.AudioType.Master) {
            pos = new FloatReactiveProperty(_width * cont.VolumeSetting.MasterVolume - _width * 0.5f);
            pos.Subscribe(x => {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                cont.VolumeSetting.MasterVolume = (pos.Value + _width * 0.5f) / _width;
            }).AddTo(this);
        }
        else if (type == AudioSystem.AudioType.BGM) {
            pos = new FloatReactiveProperty(_width * cont.VolumeSetting.BGMVolume - _width * 0.5f);
            pos.Subscribe(x => {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                cont.VolumeSetting.BGMVolume = (pos.Value + _width * 0.5f) / _width;
            }).AddTo(this);
        }
        else if(type == AudioSystem.AudioType.SE) {
            pos = new FloatReactiveProperty(_width * cont.VolumeSetting.SEVolume - _width * 0.5f);
            pos.Subscribe(x => {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
                cont.VolumeSetting.SEVolume = (pos.Value + _width * 0.5f) / _width;
            }).AddTo(this);
        }

    }

    public void OnClick()
    {
        start = GetComponent<RectTransform>().anchoredPosition.x;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(),
               Input.mousePosition, canvas.worldCamera, out clickPos);
    }

    public void OnUp()
    {
        start = -10000;
    }

    public void OnDrag()
    {
        if (start < -5000) return;
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(),
               Input.mousePosition, canvas.worldCamera, out mousePos);

        pos.Value = Mathf.Clamp(start + mousePos.x - clickPos.x, -bar.rectTransform.rect.width * 0.5f, bar.rectTransform.rect.width * 0.5f);
        GetComponent<RectTransform>().anchoredPosition = new Vector3(pos.Value, 0, 0);
    }
}
