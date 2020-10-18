using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    //Public member
    public Text waveText;
    public Text durationText;

    public void Start()
    {
        waveText.text = "Waves: " + PlayerPrefs.GetInt("Wave").ToString();
        durationText.text = "Duration: " + Mathf.Round(PlayerPrefs.GetFloat("Duration") / 60.0f) + " minutes";
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
