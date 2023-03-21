using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReturnIdle()
    {
        GetComponent<Animator>().SetInteger("AnimeNum", 0);
    }
}
