using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLayer : MonoBehaviour
{
    [SerializeField]string layerName;
    [SerializeField] int orderNum;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().sortingLayerName = layerName;
        GetComponent<Renderer>().sortingOrder = orderNum;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
