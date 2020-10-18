using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class HowToPlay : MonoBehaviour
{
    public GameObject canvas1;
    public GameObject canvas2;
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Next()
    {
        canvas1.SetActive(false);
        canvas2.SetActive(true);
    }

    public void Previous()
    {
        canvas1.SetActive(true);
        canvas2.SetActive(false);
    }

}
