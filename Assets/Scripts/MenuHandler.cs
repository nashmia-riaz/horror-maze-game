using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    delegate void back();
    back backFunction;

    public Slider brightnessSlider, volumeSlider;

    public GameObject menuPanel, optionsPanel;

    public AudioSource musicSource;

    private void Awake()
    {
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
