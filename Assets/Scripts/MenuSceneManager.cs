using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuSceneManager : MonoBehaviour
{
    public GameObject settingsPanel;

    public Slider senseSlider;
    public Slider volumeSlider;
    public AudioSource menuSound;

    void Start()
    {
        float senstiveValue = PlayerPrefs.GetFloat("SensitivityValue", 0.5f);
        senseSlider.value = senstiveValue;
    }

    // Update is called once per frame
    void Update()
    {
        MouseSensitivityValue();
        MenuSoundSet();
    }

    public void PlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitButton() 
    {
        Application.Quit();
    }

    public void SettingsButton()
    {
        if(settingsPanel.activeSelf == false)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            settingsPanel.SetActive(false);
        }
    }

    void MouseSensitivityValue()
    {
        PlayerPrefs.SetFloat("SensitivityValue", senseSlider.value);
    }

    void MenuSoundSet()
    {
        menuSound.volume = volumeSlider.value;
    }
}
