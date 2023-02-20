using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWorldUI : MonoBehaviour
{

    [SerializeField] Transform followUI;
    [SerializeField] Vector3 adjustPos;
    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {


        var targetScreenPos = Camera.main.WorldToScreenPoint(followUI.position);
        targetScreenPos = followUI.position;
        rect.position = targetScreenPos+adjustPos;
     
    }
}
