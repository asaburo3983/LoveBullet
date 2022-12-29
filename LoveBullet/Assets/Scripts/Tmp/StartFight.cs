using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFight : MonoBehaviour
{
    [SerializeField] GameObject resultCanvas;
    // Start is called before the first frame update
    void Start()
    {
        Card.Fight.instance.StartFight();
        var resultMana = ResultManager.instance;
        resultMana.resultCanvas.SetActive(false);
        resultMana.resultCanvas.GetComponent<CanvasGroup>().alpha = 0;
        resultMana.result.Value = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
