using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAP : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var player = Player.instance.gameState;
        GetComponent<TextMesh>().text = "AP" + player.AP.ToString() + "/" + player.APMax.ToString();
    }
}
