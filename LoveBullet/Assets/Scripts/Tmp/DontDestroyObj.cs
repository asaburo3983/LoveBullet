using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObj : SingletonMonoBehaviour<DontDestroyObj>
{
    private static bool created = false;
    // Start is called before the first frame update
    private void Awake()
    {
        SingletonCheck(this, true);
        return;
        if (created == false)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
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
