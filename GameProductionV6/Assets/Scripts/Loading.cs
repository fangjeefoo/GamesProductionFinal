using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{    void Start()
    {
        Invoke("LoadScene", 4.0f);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
