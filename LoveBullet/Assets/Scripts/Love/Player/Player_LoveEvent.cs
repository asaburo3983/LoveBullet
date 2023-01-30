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
            //�^�O�ɉ�����UI�̕\�����s��
            if (other.CompareTag("Senpai"))
            {
                Player_Love.instance.isUISenpaiActive.Value = true;
            }
            if (other.CompareTag("Obj"))
            {
                Player_Love.instance.isUIObjActive.Value = true;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            //�^�O�ɉ�����UI�̕\�����s��
            if (other.CompareTag("Senpai"))
            {
                Player_Love.instance.isUISenpaiActive.Value = false;
            }
            if (other.CompareTag("Obj"))
            {
                Player_Love.instance.isUIObjActive.Value = false;
            }
        }

    }
}