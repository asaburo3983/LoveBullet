using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Love
{
    public class PlayerMove : MonoBehaviour
    {
        InputSystem input;
        public float moveSpeed;
        Rigidbody2D rb;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            input = InputSystem.instance;
        }

        // Update is called once per frame
        void Update()
        {
            Move();
        
        }
        /// <summary>
        /// ƒvƒŒƒCƒ„[‚ÌˆÚ“®Œn
        /// </summary>
        private void Move()
        {
            //ˆÚ“®ˆ—
            if (input.WasPressed("Player", "Move"))
            {
                var vec = input.GetValue("Player", "Move");

                rb.AddForce(vec * moveSpeed*Time.deltaTime);
            }
            //’â~ˆ—
            if (input.WasReleasedThisFlame("Player", "Move"))
            {

                rb.velocity = Vector2.zero;
            }

        }
    }
}