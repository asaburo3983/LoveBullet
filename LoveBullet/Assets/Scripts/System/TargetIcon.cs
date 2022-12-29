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
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = fight.enemyObjects[fight.TargetId].transform.position;
    }
}
