using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DelayDestroyEffect : MonoBehaviour
{
    SpriteRenderer sr;
    [SerializeField] float fadeTime;
    // Start is called before the first frame update
    void Start()
    {
        //フェードして削除
        sr = GetComponent<SpriteRenderer>();
        sr.DOFade(0, fadeTime).OnComplete(() => { Destroy(this.gameObject); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
