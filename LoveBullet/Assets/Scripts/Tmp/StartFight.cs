using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Card.Fight.instance.StartFight();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
