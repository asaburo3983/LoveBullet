using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandManager : MonoBehaviour
{
    [SerializeField] GameObject deckList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InstantateDeckList()
    {
        Instantiate(deckList);
    }
}
