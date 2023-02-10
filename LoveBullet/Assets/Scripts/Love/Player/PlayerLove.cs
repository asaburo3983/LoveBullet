using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;

namespace Love
{
    public class PlayerLove : SingletonMonoBehaviour<PlayerLove>
    {

        MouseManager mouse;

        public Tween moveTW;

        public ReactiveProperty<bool> isUISenpaiActive;
        public ReactiveProperty<bool> isUIObjActive;

        public bool move = true;
        private void Awake()
        {
            SingletonCheck(this);

           
        }
            // Start is called before the first frame update
        void Start()
        {
            mouse = MouseManager.instance;
        }

        // Update is called once per frame
        void Update()
        {
            //Move();
            InputEvent();
        }
        void InputEvent()
        {
            
            if (Love.PlayerLove.instance.isUISenpaiActive.Value)
            {
                NovelManager.instance.NovelStart();
            }
            else if (Love.PlayerLove.instance.isUIObjActive.Value)
            {

            }

        }

    }
}