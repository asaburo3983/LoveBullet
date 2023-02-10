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

        public bool move = true;

        public GameObject fightAccessUI;

        public GameObject shopAccessUI;

        public GameObject objectAccessUI;

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
            

        }

    }
}