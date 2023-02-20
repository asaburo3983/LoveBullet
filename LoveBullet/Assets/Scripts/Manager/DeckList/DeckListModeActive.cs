using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckListModeActive : MonoBehaviour
{

    [SerializeField]DeckListManager.Mode activeMode;
    void OnEnable()
    {
        if (DeckListManager.instance.mode != activeMode)
        {
            this.gameObject.SetActive(false);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
