using UnityEngine;
using System.Collections;

public class TextLayer : MonoBehaviour
{

    public string LayerName;
    public int SortingOrder;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //���C���[�̖��O
        this.GetComponent<MeshRenderer>().sortingLayerName = LayerName;
        //Order in Layer�̐��l
        this.GetComponent<MeshRenderer>().sortingOrder = SortingOrder;
    }
}