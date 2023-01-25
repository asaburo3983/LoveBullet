using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextLayer : MonoBehaviour
{

    public int layerNum;

    // Use this for initialization
    void Start()
    {
        GetComponent<MeshRenderer>().sortingOrder = layerNum;
    }

    // Update is called once per frame
    void Update()
    {
    }
}