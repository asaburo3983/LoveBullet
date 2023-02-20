using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmSceneForActive : MonoBehaviour
{

    [SerializeField] string sceneName;
    // Start is called before the first frame update

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            this.gameObject.SetActive(true);
        }
        else
        {

            this.gameObject.SetActive(false);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
