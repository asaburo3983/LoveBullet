using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

public class MouseManager : SingletonMonoBehaviour<MouseManager>
{
   
    public ReactiveProperty<Vector2> LeftPushPos =new ReactiveProperty<Vector2>(); 
    public ReactiveProperty<Vector2> RightPushPos =new ReactiveProperty<Vector2>(); 

    private void Awake()
    {
        SingletonCheck(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;

        if (mouse != null)
        {
            // 左クリックが押された時
            if (mouse.leftButton.wasPressedThisFrame)
            {
                LeftPushPos.Value = mouse.position.ReadValue();
            }
            // 右クリックが押された時
            if (mouse.rightButton.wasPressedThisFrame)
            {
                RightPushPos.Value = mouse.position.ReadValue();
            }
        }
    }
}
