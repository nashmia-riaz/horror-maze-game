using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This script takes care of all the UI events in main menu.
/// It contains functions for each of the button clicks.
/// </summary>
public class MenuHandler : MonoBehaviour
{
    delegate void back();
    back backFunction;

    public Slider brightnessSlider, volumeSlider;

    public GameObject menuPanel, optionsPanel;

    public AudioSource musicSource;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1;
        float gamma = PlayerPrefs.GetFloat("brightness", 0);
        RenderSettings.ambientLight = new Color(gamma, gamma, gamma, 1);
        brightnessSlider.value = gamma;
        float volume = PlayerPrefs.GetFloat("volume", 1);
        volumeSlider.value = volume;

    }
    public void OnClickStart()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickOptions()
    {
        menuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        backFunction = () => {
            menuPanel.SetActive(true);
            optionsPanel.SetActive(false);
        };
    }

    public void OnClickBack()
    {
        backFunction();
    }

    public void OnChangeBrightness()
    {
        float gamma = brightnessSlider.value;
        PlayerPrefs.SetFloat("brightness", gamma);
        RenderSettings.ambientLight = new Color(gamma, gamma, gamma, 1);
        Debug.Log(gamma);
    }

    public void OnChangeVolume()
    {
        float volume = volumeSlider.value;
        PlayerPrefs.SetFloat("volume", volume);
        musicSource.volume = volume;
    }
}
