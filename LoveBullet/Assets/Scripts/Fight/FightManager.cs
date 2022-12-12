using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : SingletonMonoBehaviour<FightManager> {


    int floor;
    List<Enemy.Enemy.State> enemys; 
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartTurn()
    {
        AdventEnemys();
    }
    /// <summary>
    /// 階層に応じた敵を出現させる
    /// </summary>
    void AdventEnemys()
    {
        //敵の出現条件について
        //とりあえず指定パターンで出限させる（後にDB化して指定パターンの中でもランダムで出現させるなどを行う）
    }
    void EndTurn()
    {

    }
}
