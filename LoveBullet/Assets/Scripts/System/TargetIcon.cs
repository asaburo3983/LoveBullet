using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIcon : MonoBehaviour
{
    Card.Fight fight;
    // Start is called before the first frame update
    void Start()
    {
        fight = Card.Fight.instance;
        Debug.LogError("ターゲット処理バグってるのでコメントアウト中");
    }

    // Update is called once per frame
    void Update()
    {
        
        //transform.position = fight.enemyObjects[fight.TargetId].transform.position;
    }
}
