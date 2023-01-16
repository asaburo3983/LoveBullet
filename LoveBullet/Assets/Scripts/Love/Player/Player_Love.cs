using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;

namespace Love
{
    public class Player_Love : SingletonMonoBehaviour<Player_Love>
    {

        MouseManager mouse;

        public Tween moveTW;


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
        }


    }
}