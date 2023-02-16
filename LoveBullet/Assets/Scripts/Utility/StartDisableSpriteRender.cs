using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartDisableSpriteRender : MonoBehaviour
{
    enum Mode
    {
        SpriteRenderer,
        Image,
    }
    [SerializeField]Mode mode;
    // Start is called before the first frame update
    void Start()
    {
        if (mode == Mode.SpriteRenderer)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (mode == Mode.Image)
        {
            GetComponent<Image>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
