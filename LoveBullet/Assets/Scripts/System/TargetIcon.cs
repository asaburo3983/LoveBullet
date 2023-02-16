using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

public class TargetIcon : MonoBehaviour
{
    FightManager fight;
    ReactiveProperty<Vector3> pos = new ReactiveProperty<Vector3>();
    [SerializeField] float moveSpeed;
    Tween tw;

    // Start is called before the first frame update
    void Start()
    {
        fight = FightManager.instance;
        pos.Subscribe(x => Move(x)).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (fight.targetId >= 0 && fight.enemyObjects.Count > 0)
        {
            pos.Value = fight.enemyObjects[fight.targetId].transform.position;
            //transform.position = pos;
        }
    }
    private void Move(Vector3 _pos)
    {
        if (tw != null) { tw.Kill(true); return; }
        tw = transform.DOMove(_pos, moveSpeed).OnComplete(() => tw = null);


    }
}
