using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
public class NovelCameraMove : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float camMoveSpeed = 1;
    ReactiveProperty<bool> IsCamPosRight = new ReactiveProperty<bool>();
    [SerializeField] float camLeftPosX;
    [SerializeField] float camRightPosX;

    float playerPosX;
    Tween tw;
    // Start is called before the first frame update
    void Start()
    {
        
        IsCamPosRight.Subscribe(x => CamMove(x)).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        playerPosX = Love.PlayerLove.instance.transform.position.x;
        if (playerPosX > this.transform.position.x)
        {
            IsCamPosRight.Value = true;
        }
        if (playerPosX < this.transform.position.x)
        {
            IsCamPosRight.Value = false;
        }

    }
    void CamMove(bool right)
    {
        if (tw != null)
            tw.Kill();

        if (right)
            tw = cam.transform.DOMoveX(camRightPosX, camMoveSpeed).OnComplete(()=> tw = null);
        else
            tw=cam.transform.DOMoveX(camLeftPosX, camMoveSpeed).OnComplete(() => tw = null); ;
    }
}
