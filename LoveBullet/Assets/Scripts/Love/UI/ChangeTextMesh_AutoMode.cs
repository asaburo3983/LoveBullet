using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTextMesh_AutoMode : MonoBehaviour
{
    TextMesh textMesh;
    [SerializeField] string text1;
    [SerializeField] string text2;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        ChangeText();
    }
    public void ChangeText()
    {
        if (NovelManager.instance.autoMode)
        {
            textMesh.text = text1;
        }
        else
        {
            textMesh.text = text2;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
