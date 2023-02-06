using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [SerializeField] GameObject canvas;

    public void OnActive()
    {
        canvas.SetActive(true);
    }

    public void OffActive()
    {
        canvas.SetActive(false);
    }
}
