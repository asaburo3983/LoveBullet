using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rough
{
    public static void SetText(GameObject _ui, int _value)
    {
        if (_ui.TryGetComponent<TextMesh>(out TextMesh comp))
        {
            comp.text = _value.ToString();
        }
        //Canvas‚Ì•û‚ÌText‚É“ü‚ê‚é
        if (_ui.TryGetComponent<Text>(out Text comp2))
        {
            comp2.text = _value.ToString();
        }
    }
    public static void SetText(GameObject _ui, string _value)
    {
        if (_ui.TryGetComponent<TextMesh>(out TextMesh comp))
        {
            comp.text = _value;
        }
        //Canvas‚Ì•û‚ÌText‚É“ü‚ê‚é
        if (_ui.TryGetComponent<Text>(out Text comp2))
        {
            comp2.text = _value;
        }
    }

}
