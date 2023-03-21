using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNovel : MonoBehaviour
{
    [SerializeField] bool debug;
    [SerializeField] NovelManager.NovelMode debugNovelMode;
    [SerializeField] public int chapterNum;

    private void Awake()
    {
        if (debug)
        {
            NovelManager.novelMode = debugNovelMode;
            NovelManager.chapterNum = chapterNum;
        }
    }
    // Start is called before the first frame update
    void Start()

    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
