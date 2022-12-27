using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rough
{
    public static void SetText(GameObject _ui, int _value)
    {
        if (_ui.TryGetComponent<TextMesh>(out TextMesh comp))
        {
            comp.text = _value.ToString();
        }
    }
    public static void SetText(GameObject _ui, string _value)
    {
        if (_ui.TryGetComponent<TextMesh>(out TextMesh comp))
        {
            comp.text = _value;
        }
    }
}
