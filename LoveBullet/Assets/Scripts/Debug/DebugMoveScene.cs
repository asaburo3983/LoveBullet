using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DebugMoveScene : MonoBehaviour
{
     [SerializeField]  int fightFloor;
     [SerializeField]  NovelManager.NovelMode loveMode;
    [SerializeField] int loveChapter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveFight()
    {
        SceneManager.LoadScene("Fight");

    }
    public void MoveLove()
    {
        SceneManager.LoadScene("Love");
        NovelManager.instance.novelMode = loveMode;
        NovelManager.instance.chapterNum = loveChapter;
    
    }
    public void MoveEnd()
    {
        SceneManager.LoadScene("End");
    }

}
