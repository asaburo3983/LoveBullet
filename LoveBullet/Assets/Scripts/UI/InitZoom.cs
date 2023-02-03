using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitZoom : MonoBehaviour
{
    [SerializeField] GameObject zoom;
    [SerializeField] Card.Card card;
    public void InitCard()
    {
        var obj = Instantiate(zoom, transform.parent.parent);
        obj.transform.position = new Vector3(960, 540);

        obj.GetComponent<ZoomCard>().Initialize(card.STATE);
    }
}
