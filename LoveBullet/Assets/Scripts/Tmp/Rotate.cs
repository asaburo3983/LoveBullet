using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anything
{
    public class Rotate : MonoBehaviour
    {
        [SerializeField] float rotaXSpeed;
        [SerializeField] float rotaYSpeed;
        [SerializeField] float rotaZSpeed;
        [SerializeField] bool stop = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(stop==false)
                transform.Rotate(new Vector3(rotaXSpeed, rotaYSpeed, rotaZSpeed) * Time.deltaTime);
        }
        public void StopRotate()
        {
            stop = true;
        }
        public void StartRotate()
        {
            stop = false;
        }

    }
}
