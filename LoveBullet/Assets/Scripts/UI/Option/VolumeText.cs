using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class VolumeText : MonoBehaviour
{
    [SerializeField] AudioSystem.AudioType type;
    // Start is called before the first frame update
    void Start()
    {
        var cont = AudioSystem.AudioControl.Instance;
        if (type == AudioSystem.AudioType.Master) {
            cont.VolumeSetting.MasterVolumeReactive.Subscribe(x => SetText(x));
        }
        if (type == AudioSystem.AudioType.BGM) {
            cont.VolumeSetting.BGMVolumeReactive.Subscribe(x => SetText(x));
        }
        if (type == AudioSystem.AudioType.SE) {
            cont.VolumeSetting.SEVolumeReactive.Subscribe(x => SetText(x));
        }
    }

    void SetText(float _value)
    {
        GetComponent<Text>().text = (_value * 100f).ToString("F0") + "%";
    }
}
