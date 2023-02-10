using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Love {
    public class Player_LoveEvent : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //タグに応じたUIの表示を行う
            if (other.CompareTag("Senpai"))
            {
                PlayerLove.instance.isUISenpaiActive.Value = true;
            }
            if (other.CompareTag("Obj"))
            {
                PlayerLove.instance.isUIObjActive.Value = true;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            //タグに応じたUIの表示を行う
            if (other.CompareTag("Senpai"))
            {
                PlayerLove.instance.isUISenpaiActive.Value = false;
            }
            if (other.CompareTag("Obj"))
            {
                PlayerLove.instance.isUIObjActive.Value = false;
            }
        }

    }
}